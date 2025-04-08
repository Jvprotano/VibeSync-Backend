using Microsoft.Extensions.DependencyInjection;
using VibeSync.Application.Contracts.Authentication;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.UseCases;
using VibeSync.Infrastructure.Authentication;
using VibeSync.Infrastructure.Repositories;

namespace VibeSync.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddUseCases();
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateSpaceUseCase>();
        services.AddScoped<GetSpaceByPublicTokenUseCase>();
        services.AddScoped<GetSpaceByAdminTokenUseCase>();
        services.AddScoped<SearchSongUseCase>();
        services.AddScoped<SuggestSongToSpaceUseCase>();
        services.AddScoped<GetSuggestionsUseCase>();
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<GetSpacesByUserIdUseCase>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISpaceRepository, SpaceRepository>();
        services.AddScoped<ISuggestionRepository, SuggestionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
