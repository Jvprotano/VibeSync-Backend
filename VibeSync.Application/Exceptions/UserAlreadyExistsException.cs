using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Application.Exceptions;

public class UserAlreadyExistsException(string email) : Exception($"User with email {email} is already registered.");