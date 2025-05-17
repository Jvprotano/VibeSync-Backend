using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VibeSync.Application.Contracts.Services;

namespace VibeSync.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_config["Smtp:Host"])
        {
            Port = int.Parse(_config["Smtp:Port"] ?? throw new InvalidOperationException("Smtp:Port configuration is missing")),
            Credentials = new NetworkCredential(
                _config["Smtp:Username"],
                _config["Smtp:Password"]
            ),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["Smtp:From"] ?? throw new InvalidOperationException("Smtp:From configuration is missing")),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}