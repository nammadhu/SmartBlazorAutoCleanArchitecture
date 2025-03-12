namespace CleanArchitecture.Domain;

public class _SocialConnections : AuditableBaseEntity
    {//nondbentity
    //below are non sensitive informations
    [Display(Name = "Mobile Number")]
    [RegularExpression(@"^(\+91|0\d{10}|\d{10})$", ErrorMessage = "Invalid mobile number format.")]
    public string? MobileNumber { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Display(Name = "Google Map Address Link")]
    public string? GoogleMapAddressUrl { get; set; }

    //we can make these as like json & store if more links needed
    [Display(Name = "Google Profile Link")]
    public string? GoogleProfileUrl { get; set; }

    [Display(Name = "FaceBook Link")]
    public string? FaceBookUrl { get; set; }

    [Display(Name = "YouTube Link")]
    public string? YouTubeUrl { get; set; }

    [Display(Name = "Instagram Link")]
    public string? InstagramUrl { get; set; }

    [Display(Name = "Twitter Link")]
    public string? TwitterUrl { get; set; }

    [Display(Name = "Other Reference Link")]
    public string? OtherReferenceUrl { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double? Longitude { get; set; }
    }