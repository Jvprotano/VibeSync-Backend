using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Helpers;
using VibeSync.Infrastructure.Context;
using VibeSync.Infrastructure.Extensions;
using VibeSync.Infrastructure.Helpers;
using VibeSync.Infrastructure.Repositories;
using VibeSync.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\n\nExample: 'Bearer eyJhb...'"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // evita tolerância extra no tempo de expiração
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<YouTubeSettings>(builder.Configuration.GetSection("YoutubeSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection(FrontendSettings.SectionName));

builder.Services.AddHttpClient<ISongIntegrationRepository, SongIntegrationRepository>();
builder.Services.AddSingleton<SuggestionRateLimiter>();

builder.Services.AddScoped<IEmailSender, AuthEmailService>();

builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddApplicationInsights(
        configureTelemetryConfiguration: (config) =>
            config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
        configureApplicationInsightsLoggerOptions: (options) => { }
    );
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("App started at: " + DateTime.UtcNow);

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
        .WithHeaders("Authorization", "Content-Type")
        .WithExposedHeaders("Content-Disposition"));
}
else
{
    var frontendUrl = app.Configuration.GetSection(FrontendSettings.SectionName)
        .Get<FrontendSettings>()?.BaseUrl
        ?? throw new InvalidOperationException("Frontend BaseUrl não configurado");

    logger.LogInformation("Configurando CORS para origem: {FrontendUrl}", frontendUrl);

    app.UseCors(builder => builder
        .WithOrigins(frontendUrl)
        .AllowCredentials()
        .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
        .WithHeaders("Authorization", "Content-Type", "Accept")
        .WithExposedHeaders("Content-Disposition"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();