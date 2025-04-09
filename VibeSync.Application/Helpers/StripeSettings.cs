namespace VibeSync.Application.Helpers;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}