using BASE.Common;

namespace SHARED.DTOs;

/// <summary>
/// combination of TownCard & TownVerifiedCard
/// </summary>
public class CardDto : Card//, IEquatable<iCardDto>
    {
    public new TownDto? Town { get; set; }

    public CardDto()
        {
        //if (ImagesAsCsv.HasText())
        //    {
        //    Images = [.. ImagesAsCsv!.Split(',').Where(c => c.HasText()).Select(x => new ImageInfoBase64Url(url: x))];
        //    }
        }

    public bool IsEditable { get; set; }

    //public  string ImagesAsCsv { get; set; }
    //  public List<ImageInfoBase64Url>? Images { get; set; }
    public new CardData? CardData { get; set; }

    public new CardDetailDto? CardDetail { get; set; }
    //public new iCardDto? VerifiedCard { get; set; }//for draftcard case
    //public iCardDto? DraftCard { get; set; }//for verified card case

    //public int VerifiedCount { get; set; } //by equal or above grade people
    //public int RejectedCount { get; set; } //by equal or above grade people
    public new ICollection<CardApproval>? ApprovedPeerCardIds { get; set; }

    public bool IsItVerified() => IsVerified == true || IsAdminVerified || IsPeerVerified;

    public string ApproveStatus()
        {
        if (IsItVerified())
            {
            return IsAdminVerified ? "Admin IsVerifiedEntryExists" : IsPeerVerified ? "Peer IsVerifiedEntryExists" : "IsVerifiedEntryExists";
            }
        else if (IsGoogleVerified == true)
            return "Google Verified";
        else
            return "Not Verified Yet";
        }

    public string CreatorModifierOwnerForAdminView(Guid? userId)
        {//only for admin
        string res = "";
        if (IdOwner != Guid.Empty)
            {
            res += "Owner-" + (IdOwner == userId ? "Me" : IdOwner);
            }
        else
            {
            if (CreatedBy != Guid.Empty && CreatedBy != IdOwner)
                {
                res = "Creator-" + (CreatedBy == userId ? "Me" : CreatedBy.ToString());
                }
            if (LastModifiedBy != null && LastModifiedBy != Guid.Empty && LastModifiedBy != IdOwner)
                {
                res += "LastModifier-" + (LastModifiedBy == userId ? "Me" : LastModifiedBy);
                }
            }

        return res;
        }

    //public ICollection<TownToVerifiedCard>? TownsOfVerifiedCard { get; set; } = new List<TownToVerifiedCard>();
    //public new ICollection<TownToDraftCard>? TownsOfDraftCard { get; set; } = new List<TownToDraftCard>();
    public List<int>? TownsOfVerifiedCard { get; set; } = new List<int>();

    public new ImageInfoBase64Url? Image1 { get; set; } //= new();

    public new ImageInfoBase64Url? Image2 { get; set; } //= new();

    // public new TownCardDto? Card_Verified { get; set; } = new();

    public void NullifyPrivateData()
        {
        CreatedBy = Guid.Empty;
        IdOwner = Guid.Empty;
        LastModifiedBy = null;
        if (CardData != null)
            {
            CardData.CreatedBy = Guid.Empty;
            CardData.LastModifiedBy = null;
            }
        if (CardDetail != null)
            {
            CardDetail.CreatedBy = Guid.Empty;
            CardDetail.LastModifiedBy = null;
            }
        }
    }
