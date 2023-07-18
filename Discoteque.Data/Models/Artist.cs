using System.Runtime.CompilerServices;

namespace Discoteque.Data.Models;

public class Artist : BaseEntity<int>
{
    public Artist()
    {
        // Generate a random integer for the Id property
        Random random = new Random();
        Id = random.Next(1000,10000);
    }
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public bool IsOnTour{ get; set; }
}