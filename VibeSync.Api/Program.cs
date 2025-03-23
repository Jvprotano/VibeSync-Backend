using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Helpers;
using VibeSync.Infrastructure.Context;
using VibeSync.Infrastructure.Extensions;
using VibeSync.Infrastructure.Helpers;
using VibeSync.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<YouTubeSettings>(builder.Configuration.GetSection("YoutubeSettings"));

builder.Services.AddHttpClient<ISongIntegrationRepository, SongIntegrationRepository>();

builder.Services.AddSingleton<SuggestionRateLimiter>();

builder.Services.AddApplicationServices();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.MapIdentityApi<ApplicationUser>();

app.Run();