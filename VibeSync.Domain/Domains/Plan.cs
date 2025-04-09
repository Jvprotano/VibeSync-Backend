namespace VibeSync.Domain.Domains;

public class Plan
{
    public Plan(Guid id, string name, int maxSpaces, decimal price, string? stripePriceId = null)
    {
        Id = id;
        Name = name;
        MaxSpaces = maxSpaces;
        Price = price;
        StripePriceId = stripePriceId;
        CreatedAt = DateTime.Now;
    }

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int MaxSpaces { get; private set; }
    public decimal Price { get; private set; }
    public string? StripePriceId { get; private set; } = string.Empty;
    public bool IsFree => Price == 0;
}