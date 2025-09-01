using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Helpers;
using VibeSync.Domain.Domains;

namespace VibeSync.Infrastructure.Services;

public class AuthEmailService : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly FrontendSettings _frontendSettings;
    private readonly ILogger<AuthEmailService> _logger;


    public AuthEmailService(
        IConfiguration config,
        IOptions<FrontendSettings> frontendSettingsOptions,
        ILogger<AuthEmailService> logger)
    {
        _frontendSettings = frontendSettingsOptions.Value; // Obtém o objeto de configurações
        _logger = logger;
        _config = config;
    }


    public async Task SendConfirmationEmailAsync(User user, string token)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("User email cannot be empty.", nameof(user.Email));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

        var encodedToken = HttpUtility.UrlEncode(token);
        var confirmationLink = $"{_frontendSettings.BaseUrl}/email-confirmation?userId={user.Id}&token={encodedToken}";

        var emailSubject = "Confirme seu e-mail no VibeSync";
        var emailBody = $@"
                Olá {(string.IsNullOrWhiteSpace(user.FullName) ? "Usuário" : user.FullName)},<br/><br/>
                Obrigado por se registrar no VibeSync!<br/>
                Por favor, clique no link abaixo para confirmar seu endereço de e-mail:<br/>
                <a href=""{confirmationLink}"">Confirmar meu E-mail</a><br/><br/>
                Se você não conseguir clicar no link, copie e cole a seguinte URL no seu navegador:<br/>
                {confirmationLink}<br/><br/>
                Se você não se registrou no VibeSync, por favor, ignore este e-mail.<br/><br/>
                Atenciosamente,<br/>
                Equipe VibeSync";

        _logger.LogInformation("Attempting to send confirmation email to {Email} with link: {Link}", user.Email, confirmationLink);
        await SendEmailAsync(user.Email, emailSubject, emailBody);
        _logger.LogInformation("Confirmation email sent successfully to {Email}", user.Email);
    }

    public async Task SendPasswordResetEmailAsync(User user, string token)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("User email cannot be empty.", nameof(user.Email));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

        _logger.LogInformation("Preparing to send password reset email to {Email}; {Token}", user.Email, token);

        var encodedToken = HttpUtility.UrlEncode(token);
        var resetLink = $"{_frontendSettings.BaseUrl}/reset-password?email={HttpUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var emailSubject = "Redefinição de Senha - VibeSync";
        var emailBody = $@"
                Olá {(string.IsNullOrWhiteSpace(user.FullName) ? "Usuário" : user.FullName)},<br/><br/>
                Recebemos uma solicitação para redefinir sua senha no VibeSync.<br/>
                Por favor, clique no link abaixo para criar uma nova senha:<br/>
                <a href=""{resetLink}"">Redefinir minha senha</a><br/><br/>
                Se você não solicitou a redefinição de senha, por favor, ignore este e-mail.<br/>
                Este link expirará em 24 horas por questões de segurança.<br/><br/>
                Se você não conseguir clicar no link, copie e cole a seguinte URL no seu navegador:<br/>
                {resetLink}<br/><br/>
                Atenciosamente,<br/>
                Equipe VibeSync";

        _logger.LogInformation("Attempting to send password reset email to {Email} with link: {Link}", user.Email, resetLink);
        await SendEmailAsync(user.Email, emailSubject, emailBody);
        _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
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