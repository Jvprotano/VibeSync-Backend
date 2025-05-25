namespace VibeSync.Domain.Models;

public class Space : BaseEntity
{
    public Space(string name, Guid userId, DateTime eventDate)
    {
        Name = name;
        UserId = userId;
        AdminToken = Guid.NewGuid();
        PublicToken = Guid.NewGuid();
        EventDate = eventDate;
    }

    public string Name { get; private set; } = string.Empty;
    public Guid PublicToken { get; private set; }
    public Guid AdminToken { get; private set; }
    public string QrCode { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime EventDate { get; private set; }
    public IList<Suggestion>? Suggestions { get; private set; } = [];

    public void SetQrCode(string qrCode)
    {
        QrCode = qrCode;
    }
}
