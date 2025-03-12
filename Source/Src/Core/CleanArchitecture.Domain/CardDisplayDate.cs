using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class CardDisplayDate : AuditableBaseEntity
    {
    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    [Key]
    public override int Id { get; set; }

    public DateOnly Date { get; set; }

    //todo had to add more columsn like end date or more ranges

    // Navigation property to the Card_Verified
    [ForeignKey(nameof(Id))]
    public virtual Card_DraftChanges? VerifiedCard { get; set; }
    }