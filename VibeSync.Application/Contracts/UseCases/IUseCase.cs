namespace VibeSync.Application.Contracts.UseCases;

public interface IUseCase<TRequest, TResponse>
{
    Task<TResponse> Execute(TRequest request);
}