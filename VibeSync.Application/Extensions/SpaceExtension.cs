using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Extensions;

public static class SpaceExtension
{
    public static Space AsDomain(this CreateSpaceRequest self, Guid userId)
    {
        return new Space(
            self.Name,
            userId,
            self.EventDate
        );
    }

    public static SpaceResponse AsResponseModel(this Space self)
    {
        return new SpaceResponse(
            self.AdminToken,
            self.PublicToken,
            self.Name,
            self.QrCode,
            self.EventDate,
            self.Suggestions?.Count() ?? 0
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