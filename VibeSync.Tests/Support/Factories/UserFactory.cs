using VibeSync.Application.Requests;

namespace VibeSync.Tests.Support.Factories;

public static class UserFactory
{
    public static RegisterRequest CreateValidRegisterRequest()
        => new("teste@teste.com", "SenhaTeste123@", "Full Name Teste");
}