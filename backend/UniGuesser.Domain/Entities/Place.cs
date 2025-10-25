using System.ComponentModel.DataAnnotations;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Domain.Entities;

public class Place
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ImageUrl { get; set; }
    public string Alt { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public ImageType ImageType { get; set; }
    public bool InQueue { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Guid? AuthorId { get; set; }
    public virtual User? AuthorPlace { get; set; }


}