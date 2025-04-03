using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Application.Exceptions;

public class UserAlreadyRegisteredException(string email) : BadRequestException($"User with email {email} is already registered.");