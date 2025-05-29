using Microsoft.Extensions.Options;
using QRCoder;
using VibeSync.Application.Helpers;

namespace VibeSync.Infrastructure.Helpers;

public static class QrCodeExtension
{
    public static string GenerateQrCode(string uri)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new Base64QRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        return qrCodeImage;
    }
}