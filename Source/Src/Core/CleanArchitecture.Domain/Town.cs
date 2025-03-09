using PublicCommon;
using System.ComponentModel;

namespace CleanArchitecture.Domain;

//db entity
public class Town : _SocialConnections, IEquatable<Town>
    {
    [Key]
    public override int Id { get; set; }

    public Town()
        {
        }

    public Town(string title)
        {
        Title = title;
        }

    public string Title { get; set; }// = default;
    public string? SubTitle { get; set; }
    public string? Description { get; set; }

    [DefaultValue(false)]
    public bool IsDisabled { get; set; } = false;

    public string? District { get; set; } //= "Shimoga";//later move to other table called districts & refer here only id
    public string? State { get; set; } //= "Karnataka";//later move to other table called states & refer here only id
    public string? UrlName1 { get; set; }//bhadravathi.com
    public string? UrlName2 { get; set; }//bdvt.in

    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
    public string? Image3 { get; set; }
    public string? Image4 { get; set; }
    public string? Image5 { get; set; }
    public string? Image6 { get; set; }
    public string? MoreImages { get; set; }//json format

    [Display(Name = "YouTube Video Link")]
    public string? YouTubeVideoLink { get; set; }

    //any external Url's links of direct image only
    [Display(Name = "External Image Link")]
    public string? ExternalImageLink { get; set; }

    public int? Population { get; set; }
    public int? VoteCount { get; set; }
    public string? PinCodePostal { get; set; }
    public string? AlternateNames { get; set; }//csv

    [MaxLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
    public string? DetailDescription { get; set; }

    //may be this might makes heavy
    public virtual List<Card>? DraftCards { get; set; } = [];

    public virtual List<Card_DraftChanges>? VerifiedCards { get; set; } = [];
    public virtual List<Card_AdditionalTown>? VerifiedCardsAdditional { get; set; } = [];

    public bool Equals(Town? otherDetails)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
        if (otherDetails == null) return false; // Not the same type
        return Equals(this, otherDetails);
        }

    public static bool Equals(Town? source, Town? other)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
        if (source == null && other == null) return true; // Not the same type
        if (source == null || other == null) return false;
        //id card compare on derived
        return
            StringExtensions.Equals(source.Title, other.Title) &&
            StringExtensions.Equals(source.SubTitle, other.SubTitle) &&
            StringExtensions.Equals(source.Description, other.Description) &&
            source.IsDisabled == other.IsDisabled &&

            StringExtensions.Equals(source.State, other.State) &&
            StringExtensions.Equals(source.District, other.District) &&
            StringExtensions.Equals(source.State, other.State) &&
            StringExtensions.Equals(source.UrlName1, other.UrlName1) &&
            StringExtensions.Equals(source.UrlName2, other.UrlName2) &&
            StringExtensions.Equals(source.MobileNumber, other.MobileNumber) &&
            StringExtensions.Equals(source.Email, other.Email) &&
            StringExtensions.Equals(source.GoogleMapAddressUrl, other.GoogleMapAddressUrl) &&
            StringExtensions.Equals(source.GoogleProfileUrl, other.GoogleProfileUrl) &&
            StringExtensions.Equals(source.FaceBookUrl, other.FaceBookUrl) &&
            StringExtensions.Equals(source.YouTubeUrl, other.YouTubeUrl) &&
            StringExtensions.Equals(source.InstagramUrl, other.InstagramUrl) &&
            StringExtensions.Equals(source.TwitterUrl, other.TwitterUrl) &&
            StringExtensions.Equals(source.OtherReferenceUrl, other.OtherReferenceUrl) &&
            StringExtensions.Equals(source.DetailDescription, other.DetailDescription) &&
            EqualImages(source, other);
        }

    public virtual bool EqualImages(Town? other)//compares without id
        {//usage bool isEqual1 = person1.EqualImages(person2);
        if (other == null) return false; // Not the same type

        //IdCardBrand == otherCard.IdCardBrand //here wont check for id
        return EqualImages(this, other); // Compare properties
        }

    public static bool EqualImages(Town source, Town? other)//compares without id
        {
        if (other == null) return false; // Not the same type

        //IdCardBrand == otherCard.IdCardBrand //here wont check for id
        return
            StringExtensions.Equals(source.Image1, other.Image1) &&
            StringExtensions.Equals(source.Image2, other.Image2) &&
            StringExtensions.Equals(source.Image3, other.Image3) &&
            StringExtensions.Equals(source.Image4, other.Image4) &&
            StringExtensions.Equals(source.Image5, other.Image5) &&
            StringExtensions.Equals(source.Image6, other.Image6) &&
            StringExtensions.Equals(source.MoreImages, other.MoreImages);
        }

    //may be this is also not required
    public DateTime? LastCardUpdateTime { get; set; }

    public static bool IsNotImageUrls(Town? town) => (town == null ||
       (!ImageInfoBase64Url.IsUrl(town.Image1) && !ImageInfoBase64Url.IsUrl(town.Image2)
       && !ImageInfoBase64Url.IsUrl(town.Image3) && !ImageInfoBase64Url.IsUrl(town.Image4)
       && !ImageInfoBase64Url.IsUrl(town.Image5) && !ImageInfoBase64Url.IsUrl(town.Image6)));

    //remove below
    //public int? IdTownCard { get; set; }//only id,no relationship...so had to fetch separately & attach if needed
    //public virtual Card_Draft? TownDraftCard { get; set; }
    //public virtual Card_Verified? TownVerifiedCard { get; set; }
    }