using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Domain.Exceptions;

public sealed class SpaceNotFoundException(Guid guid) : NotFoundException("Space not found: " + guid);