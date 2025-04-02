using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Extensions;

public static class SpaceExtension
{
    public static Space AsDomain(this CreateSpaceRequest space, string userId)
    {
        return new Space(
            space.Name,
            userId
        );
    }

    public static SpaceResponse AsResponseModel(this Space space)
    {
        return new SpaceResponse(
            space.AdminToken,
            space.PublicToken,
            space.Name,
            $"https://www.google.com/search?q={space.Id}",
            space.QrCode
        );
    }

    public static GetPublicSpaceResponse AsPublicResponseModel(this Space space)
    {
        return new GetPublicSpaceResponse(
            space.PublicToken,
            space.Name
        );
    }
}