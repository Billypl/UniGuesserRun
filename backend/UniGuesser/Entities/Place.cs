using PartyGame.Entities;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.PlaceModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Place
{
    [Key]
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ImageUrl { get; set; }
    public string Alt { get; set; }
    public string DifficultyLevel { get; set; }
    public bool InQueue { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public int? AuthorId { get; set; }
    public virtual User? AuthorPlace { get; set; }


}
