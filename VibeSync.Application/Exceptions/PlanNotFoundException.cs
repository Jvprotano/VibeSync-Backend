using VibeSync.Application.Exceptions.Base;

namespace VibeSync.Application.Exceptions;

public sealed class PlanNotFoundException(string mensagem) : NotFoundException(mensagem);