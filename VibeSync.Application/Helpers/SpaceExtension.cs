using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Helpers;

public static class SpaceExtension
{
    public static Space AsModel(this CreateSpaceRequest space)
    {
        return new Space(
            space.Name,
            space.Expiration
        );
    }

    public static SpaceResponse AsDomain(this Space space)
    {
        return new SpaceResponse(
            space.Id,
            space.Name,
            $"https://www.google.com/search?q={space.Id}",
            space.QrCode
        );
    }
}