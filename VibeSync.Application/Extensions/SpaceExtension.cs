using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Extensions;

public static class SpaceExtension
{
    public static Space AsModel(this CreateSpaceRequest space, string userId)
    {
        return new Space(
            space.Name,
            userId
        );
    }

    public static SpaceResponse AsDomain(this Space space)
    {
        return new SpaceResponse(
            space.AdminToken,
            space.PublicToken,
            space.Name,
            $"https://www.google.com/search?q={space.Id}",
            space.QrCode
        );
    }

    public static GetPublicSpaceResponse AsPublicDomain(this Space space)
    {
        return new GetPublicSpaceResponse(
            space.PublicToken,
            space.Name
        );
    }
}