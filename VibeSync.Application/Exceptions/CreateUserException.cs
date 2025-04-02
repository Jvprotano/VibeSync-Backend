using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Application.Exceptions;

public class CreateUserException(string? message = null) : BadRequestException(message ?? "Error creating user");