namespace VibeSync.Application.Contracts.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
