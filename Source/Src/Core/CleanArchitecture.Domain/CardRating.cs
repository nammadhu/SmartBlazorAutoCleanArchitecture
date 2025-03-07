using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class CardRating : AuditableBaseEntity
{
    //[Key]//also ref of userid but its different db
    [Display(Name = "ByUserId")]
    public Guid UserId { get; set; }

    //IdCard+UserId
    [Required]
    public int IdCARD { get; set; } //bhadravathi,kadur,bidar

    [ForeignKey(nameof(IdCARD))]
    public virtual Card? iCard { get; set; }

    public bool Liked { get; set; }

    //either like or rating anyone might be enough //later
    public int Rating { get; set; }

    public string? Message { get; set; }
}