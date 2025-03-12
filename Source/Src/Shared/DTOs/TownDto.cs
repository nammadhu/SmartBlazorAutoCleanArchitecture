using PublicCommon.Common;

namespace Shared.DTOs;

//todo change all these later
public class TownDto : Town
    {
    public TownDto()
        {
        }

    public TownDto(string title) : base(title)
        {
        Name = title;
        }

    //public DateTime? ServerSideFromDbLoadedTime { get; set; }
    //public DateTime? CacheLoadedTime { get; set; }
    public new List<CardDto>? VerifiedCards { get; set; }

    public new List<CardDto>? DraftCards { get; set; }

    public new ImageInfoBase64Url? Image1 { get; set; } //= new();
    public new ImageInfoBase64Url? Image2 { get; set; } //= new();
    public new ImageInfoBase64Url? Image3 { get; set; }
    public new ImageInfoBase64Url? Image4 { get; set; }
    public new ImageInfoBase64Url? Image5 { get; set; }
    public new ImageInfoBase64Url? Image6 { get; set; }
    public new List<ImageInfoBase64Url>? MoreImages { get; set; }

    public string FileName(string? actualName) => $"{ConstantsTown.TownMainImagesPrefix}_{Id}_{actualName ?? ""}";

    public void FileNamesUpdate()
        {
        if (Image1 != null && ImageInfoBase64Url.IsBase64(Image1)) Image1.FileName = FileName(Image1.FileName);
        if (Image2 != null && ImageInfoBase64Url.IsBase64(Image2)) Image2.FileName = FileName(Image2.FileName);

        if (Image3 != null && ImageInfoBase64Url.IsBase64(Image3)) Image3.FileName = FileName(Image3.FileName);
        if (Image4 != null && ImageInfoBase64Url.IsBase64(Image4)) Image4.FileName = FileName(Image4.FileName);
        if (Image5 != null && ImageInfoBase64Url.IsBase64(Image5)) Image5.FileName = FileName(Image5.FileName);
        if (Image6 != null && ImageInfoBase64Url.IsBase64(Image6)) Image6.FileName = FileName(Image6.FileName);
        }

    public (List<ImageInfo> listToUpload, List<ImageInfo> listToDelete) GetUploadAndDeleteImages(CardDetail? existing)
        {
        List<ImageInfo> listToUpload = new List<ImageInfo>();
        List<ImageInfo> listToDelete = new List<ImageInfo>();
        FileNamesUpdate();
        ImageInfoBase64Url.LoadUploadDelete(Image1, nameof(Image1), existing?.Image1, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image2, nameof(Image2), existing?.Image2, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image3, nameof(Image3), existing?.Image3, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image4, nameof(Image4), existing?.Image4, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image5, nameof(Image5), existing?.Image5, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image6, nameof(Image6), existing?.Image6, listToUpload, listToDelete);
        return new(listToUpload, listToDelete);
        }

    public static bool IsNotImageUrlNorBase64(TownDto? town) => town == null ||
        ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image1) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image2) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image3) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image4) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image5) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image6);

    public void NullifyPrivateData()
        {
        CreatedBy = Guid.Empty;
        LastModifiedBy = null;
        }
    }
