using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    Task SendConfirmationEmailAsync(User user, string token);
}
