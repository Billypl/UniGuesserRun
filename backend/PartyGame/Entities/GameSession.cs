using PartyGame.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class GameSession
{
    [Key]
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public virtual List<Round> Rounds { get; set; }


    public DateTime ExpirationDate { get; set; }
    public int ActualRoundNumber { get; set; }
    
    public double GameScore { get; set; }
    public string Difficulty { get; set; }

    public int? UserId { get; set; }
    public virtual User? Player { get; set; }

    public bool IsFinished { get; set; } = false;
}