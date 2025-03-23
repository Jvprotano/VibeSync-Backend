namespace VibeSync.Domain.Models;

public class Space : BaseEntity
{
    public Space(string name, string userId)
    {
        Name = name;
        UserId = userId;
        ExpirationDate = CreatedAt.AddDays(7);
        AdminToken = Guid.NewGuid();
        PublicToken = Guid.NewGuid();
    }

    public string Name { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    public Guid PublicToken { get; private set; }
    public Guid AdminToken { get; private set; }
    public string QrCode { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    
    public void SetQrCode(string qrCode)
    {
        QrCode = qrCode;
    }
}
