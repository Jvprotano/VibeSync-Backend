namespace VibeSync.Domain.Models;

public class Space : BaseEntity
{
    protected Space() { }
    public Space(string name, DateTime? expirationDate)
    {
        Name = name;
        ExpirationDate = expirationDate ?? this.CreatedAt.AddDays(7);
    }
    public string Name { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    public string AdminToken { get; private set; } = Guid.NewGuid().ToString();
    public string QrCode { get; private set; } = string.Empty;

    public void SetQrCode(string qrCode)
    {
        QrCode = qrCode;
    }
}
