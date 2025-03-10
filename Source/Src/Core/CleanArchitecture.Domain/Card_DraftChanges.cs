using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class Card_DraftChanges : _CardBase, IEquatable<Card_DraftChanges>
    {
    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] // Disable identity column]//this is not working use fluent on ApplicationDbContext
    //[Key]
    //public int IdCard { get; set; }
    //no direct operation here by creator,instead all goes to card
    public Card_DraftChanges()
        {
        IdOwner = CreatedBy;  //LastModifiedBy ?? CreatedBy;
        }

    public Card_DraftChanges(int typeId, string title) : this()
        {
        IdCardType = typeId;
        Name = title;
        }

    [Key]
    public override int Id { get; set; }

    //[ForeignKey(nameof(IdCard))]//dont mention here
    public virtual Card? iCard { get; set; }

    [NotMapped]
    public new bool? IsVerified { get; set; }//this is for edit dialog purpos

    public bool EqualImages(Card_DraftChanges? other)//wont compare id
        {
        return _CardBase.EqualImages(this, other);
        }

    //this wont check CardData or CardDetails
    public bool Equals(Card_DraftChanges? other)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
        if (other == null) return false; // Not the same type
        return Id == other.Id && _CardBase.Equals(this, other);
        }

    public static bool Equals(Card_DraftChanges? verified, Card? draft)//compares including id
        {
        return _CardBase.Equals(verified, draft) && draft?.Id == verified?.Id;
        }

    public virtual ICollection<CardApproval>? CardApprovals { get; set; }
    }
