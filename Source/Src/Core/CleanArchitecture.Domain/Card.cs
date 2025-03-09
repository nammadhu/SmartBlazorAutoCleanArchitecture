using PublicCommon;

namespace CleanArchitecture.Domain;

//each called iCard , internet card of any user or business entity
//dbentity, once verified then will be moving the verified entity to Card_Verified table
public class Card : _CardBase
    {
    public Card()
        {
        IdOwner = CreatedBy;
        VerifiedCardDisplayDates = [];//new HashSet<TownVerifiedCardSelectedDate>();
        }

    public Card(int typeId, string title) : this()
        {
        IdCardType = typeId;
        Title = title;
        }

    public Card(CardType type, string title) : this()
        {
        Type = type;
        Title = title;
        }

    public Card(CardType type, string title, string subtitle) : this(type, title)
        {
        SubTitle = subtitle;
        }

    public Card(CardType type, int id, string title, string subtitle) : this(type, title, subtitle)
        {
        //this should be removed later,as id is from db or from screen its 0/null only
        Id = id;
        }

    [Key]
    public override int Id { get; set; }

    public new bool? IsVerified { get; set; }//this is for edit dialog purpose
    public bool IsAdminVerified { get; set; }
    public bool IsPeerVerified { get; set; }
    public virtual ICollection<int>? ApprovedPeerCardIds { get; set; } //doesnt contain admin ids

    //ApprovedCards either save as string or mapping table
    //public virtual ICollection<CardApproval>? CardApprovals { get; set; }//remove
    public bool IsGoogleVerified { get; set; }

    public bool IsDraftExists { get; set; }//keep draft  only if some change otherwise clear draft table entry

    //public bool IsSameAsVerified { get; set; }//toRemove
    //once verified make it as true. then if any change on Draft table then change it to false
    //null:notYet,true:yes,false:rejected
    //public bool IsVerifiedEntryExists { get; set; }//toRemove //either admin or peerverified

    //[ForeignKey(nameof(VerifiedCardId))]//dont mention this

    public bool IsClaimed { get; set; } = false;

    public virtual Card_DraftChanges? DraftChanges { get; set; }

    public virtual CardData? CardData { get; set; }

    public virtual CardDetail? CardDetail { get; set; }//options so virtual
                                                       //public virtual int Id { get; set; }

    public virtual ICollection<CardRating>? CardRatings { get; set; }

    //This requires on every Verified/Reject needs to update these
    //public int VerifiedPeerCount { get; set; } //by equal or above grade people
    //public int RejectedPeerCount { get; set; } //by equal or above grade people

    //TownItemLikeDisLike
    public int LikeCount { get; set; }//by public anyone

    public int DisLikeCount { get; set; }//by public anyone

    //town2verified card lets add apart from card base town only in case of extra mapping,not for all. as its unnecessary.

    public virtual ICollection<Card_AdditionalTown>? AdditionalTownsOfVerifiedCard { get; set; } = new List<Card_AdditionalTown>();

    public bool IsActiveSubscriber { get; set; }//this had to be changed everyday based on selection
    public virtual ICollection<CardDisplayDate> VerifiedCardDisplayDates { get; set; }

    public void AddDate(CardDisplayDate date)
        {
        // The HashSet will handle uniqueness and performance efficiently
        VerifiedCardDisplayDates.Add(date);
        }

    public static bool Equals(Card? source, Card? other)//compares including id
        {
        var baseCompare = _CardBase.Equals(source, other);
        if (baseCompare)//then check remaining comparison
            {
            if (source == null && other == null) return true;
            else if (source == null || other == null) return false;
            return source.IsAdminVerified == other.IsAdminVerified &&
                ListExtensions.AreListsEqualIgnoringOrder(source.ApprovedPeerCardIds?.ToList(), other.ApprovedPeerCardIds?.ToList())
                && source.LikeCount == other.LikeCount && source.DisLikeCount == other.DisLikeCount;
            }
        else return false;
        }
    }

//[NotMapped]
//public int? IdTownFirst { get; set; }

/* , IEquatable<Card_Draft>
    //this wont check CardData or CardDetails
    public bool Equals(Card_Draft? other)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
        if (other == null) return false; // Not the same type
        return IdCard == other.IdCard && Equals(this, other);
        }

    //this wont check CardData or CardDetails
    public static bool Equals(Card_Draft? source, Card_Draft? other)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
        var baseCompare = CardBase.Equals(source, other);

        if (baseCompare)//then check remaining comparison
            {
            if (source == null || other == null) return true;
            return source.IsVerifiedEntryExists == other.IsVerifiedEntryExists &&
                source.IsSameAsVerified == other.IsSameAsVerified;
            }
        else return false;
        }
    */
/*
    public bool EqualImages(Card_Draft? other)//compares including id
        {
        return CardBase.EqualImages(this, other);
        }

    //this wont check CardData or CardDetails
    public static bool Equals(Card_Draft? draft, Card_Verified? verified)//compares including id
        {
        return CardBase.Equals(verified, draft) && draft?.IdCard == verified?.IdCARD;
        }

    */
