using PublicCommon;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain;

public class CardApproval : AuditableBaseEntity, IEquatable<CardApproval>
    {
    [Key]
    public override int Id { get; set; }

    //here admin entry will be as townidcard +admin id

    //Step1 user will choose approver
    //IdCard+IdCardOfApprover will be filled while selecting
    //step2 user will approve then will mark userif of particulr card owner,later can be tracked easily by which user and card owner all details

    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    [Required]
    public int IdCard { get; set; } //draftcardid for which approval taking

    [ForeignKey(nameof(IdCard))]
    public virtual Card? iCard { get; set; }

    //cant use required,bcz either this or TownId will be stored here
    // public int? IdCardOfApprover { get; set; }
    private int? _idCardOfApprover;

    public int? IdCardOfApprover
        {
        get => _idCardOfApprover;
        set
            {
            _idCardOfApprover = value == 0 ? null : value;
            }
        }

    public int? IdTown { get; set; }

    [NotMapped]
    public string? Title { get; set; } = ConstantString.TownAdmin;//only for UI purpose

    [NotMapped]
    public bool IsSelected { get; set; }

    [ForeignKey(nameof(IdCardOfApprover))]
    public virtual Card_DraftChanges? ApproverCard { get; set; }

    //when creator requested Verified is null then either he can approve or reject
    public bool? IsVerified { get; set; }

    //thw one who is approving his userId
    public Guid? UserId { get; set; } = null;//can be admin or any target verified card owner with right

    public string? Message { get; set; }

    //public bool Equals(CardApproval? x, CardApproval? y)
    //    {
    //    if (ReferenceEquals(x, y)) return true;
    //    if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
    //    //x == null && y == null) ||(x != null && y != null &&
    //    return (x.IdCard == y.IdCard && x.IdTown == y.IdTown && x.IdCardOfApprover == y.IdCardOfApprover);
    //    }
    public override bool Equals(object? obj)
        {
        return Equals(obj as CardApproval);
        }

    public bool Equals(CardApproval? target)
        {
        if (ReferenceEquals(this, target)) return true;
        if (ReferenceEquals(target, null)) return false;
        return IdCard == target.IdCard && IdTown == target.IdTown && IdCardOfApprover == target.IdCardOfApprover && IsVerified == target.IsVerified && Message == target.Message;
        }

    public static bool Equals(List<CardApproval>? source, List<CardApproval>? target)
        {
        if (source == null || target == null)
            return source == target; // Both null or both not null

        if (source.Count != target.Count)
            return false; // Different number of elements
                          // Compare each element using the Equals method
        for (int i = 0; i < source.Count; i++)
            if (!source[i].Equals(target[i]))
                return false;

        return true;
        }

    public override int GetHashCode()
        {
        unchecked
            {
            var hashCode = IdCard.GetHashCode();
            hashCode = (hashCode * 397) ^ (IdTown != null ? IdTown.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (IdCardOfApprover != null ? IdCardOfApprover.GetHashCode() : 0);
            return hashCode;
            }
        }

    public void SetTitleAndNullifyNestedCards()
        {
        Title = ApproverCard?.Title ?? "";
        iCard = null;
        ApproverCard = null;
        }
    }

public class CardApprovalComparer : IEqualityComparer<CardApproval>
    {
    public bool Equals(CardApproval? x, CardApproval? y)
        {
        return x?.Equals(y) ?? y is null;
        }

    public int GetHashCode(CardApproval obj)
        {
        return obj.GetHashCode();
        }
    }

public static class CardApprovalExtensions
    {
    public static bool AreListsEqual(List<CardApproval>? list1, List<CardApproval>? list2, IEqualityComparer<CardApproval> comparer)
        {
        if ((list1 == null || list1.Count == 0) && (list2 == null || list2.Count == 0)) return true;

        if (list1 != null && list2 != null && list1!.Count != list2!.Count)
            return false;

        var set1 = new HashSet<CardApproval>(list1 ?? [], comparer);
        var set2 = new HashSet<CardApproval>(list2 ?? [], comparer);

        return set1.SetEquals(set2);
        }
    }
