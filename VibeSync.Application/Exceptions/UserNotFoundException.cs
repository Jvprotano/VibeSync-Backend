using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Application.Exceptions;

public class UserNotFoundException() : NotFoundException("User not found");