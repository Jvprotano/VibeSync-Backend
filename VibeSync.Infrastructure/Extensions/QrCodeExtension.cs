using QRCoder;

namespace VibeSync.Infrastructure.Helpers;

public static class QrCodeExtension
{
    public static string GenerateQrCode(string text)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode($"https://vibe-sync-frontend.vercel.app/{text}", QRCodeGenerator.ECCLevel.Q);
        var qrCode = new Base64QRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        return qrCodeImage;
    }
}