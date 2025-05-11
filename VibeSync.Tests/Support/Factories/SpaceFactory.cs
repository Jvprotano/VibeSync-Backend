using VibeSync.Domain.Models;

namespace VibeSync.Tests.Support.Factories;

public static class SpaceFactory
{
    public static List<Space> Generate(int? count = null, Guid? userId = null)
    {
        if (count == null)
        {
            Random random = new();
            count = random.Next(1, 100);
        }
        var spaces = new List<Space>();

        for (int i = 0; i < count; i++)
        {
            var space = new Space($"Space {i + 1}", userId ?? Guid.NewGuid(), DateTime.UtcNow.AddDays(1));
            spaces.Add(space);
        }

        return spaces;
    }
}