using PublicCommon;
using PublicCommon.Common;

namespace Shared.DTOs;

#pragma warning disable 
public class CardDetailDto : CardDetail, IEquatable<CardDetailDto>  //: TownCardProduct
    {
    //public new List<OpenCloseTiming>? TimingsToday { get; set; }
    //public new List<OpenCloseTimingsOfDay>? TimingsUsual { get; set; }
    public new Que? Queue { get; set; }

    public CardDetailDto()
        {
        Image1 = new ImageInfoBase64Url();
        Image2 = new ImageInfoBase64Url();
        Image3 = new ImageInfoBase64Url();
        Image4 = new ImageInfoBase64Url();
        Image5 = new ImageInfoBase64Url();
        Image6 = new ImageInfoBase64Url();
        }

    public string FileName(string? actualName) => $"{ConstantsTown.CardDetailImagesPrefix_Keyword}_{Id}_{actualName ?? ""}";

    public void FileNamesUpdate()
        {
        if (ImageInfoBase64Url.IsBase64(Image1)) Image1.FileName = FileName(Image1.FileName);
        if (ImageInfoBase64Url.IsBase64(Image2)) Image2.FileName = FileName(Image2.FileName);

        if (ImageInfoBase64Url.IsBase64(Image3)) Image1.FileName = FileName(Image3.FileName);
        if (ImageInfoBase64Url.IsBase64(Image4)) Image2.FileName = FileName(Image4.FileName);
        if (ImageInfoBase64Url.IsBase64(Image5)) Image1.FileName = FileName(Image5.FileName);
        if (ImageInfoBase64Url.IsBase64(Image6)) Image2.FileName = FileName(Image6.FileName);
        }
    /*public List<ImageInfo> ToUploadImages()
        {
        List<ImageInfo> list = new List<ImageInfo>();
        FileNamesUpdate();
        if (ImageInfoBase64Url.IsBase64(Image1)) list.Add(new ImageInfo(Image1, nameof(Image1)));
        if (ImageInfoBase64Url.IsBase64(Image2)) list.Add(new ImageInfo(Image2, nameof(Image2)));
        if (ImageInfoBase64Url.IsBase64(Image3)) list.Add(new ImageInfo(Image3, nameof(Image3)));
        if (ImageInfoBase64Url.IsBase64(Image4)) list.Add(new ImageInfo(Image4, nameof(Image4)));
        if (ImageInfoBase64Url.IsBase64(Image5)) list.Add(new ImageInfo(Image5, nameof(Image5)));
        if (ImageInfoBase64Url.IsBase64(Image6)) list.Add(new ImageInfo(Image6, nameof(Image6)));
        return list;
        }*/
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

    public new ImageInfoBase64Url? Image1 { get; set; }

    public new ImageInfoBase64Url? Image2 { get; set; }
    public new ImageInfoBase64Url? Image3 { get; set; }
    public new ImageInfoBase64Url? Image4 { get; set; }
    public new ImageInfoBase64Url? Image5 { get; set; }
    public new ImageInfoBase64Url? Image6 { get; set; }
    public new List<ImageInfoBase64Url>? MoreImages { get; set; }
    //"storageaccounc.om/1.jpg,storageaccounc.om/2.png,storageaccounc.om/3.jpeg,google.com/4.jpeg"

    //  public new TownCardDto? Card_Verified { get; set; }

    public static bool IsNullOrDefault(CardDetailDto? detail) => detail == null ? true :
        IsNotImageUrlNorBase64(detail) && string.IsNullOrEmpty(detail.DetailDescription)
        && string.IsNullOrEmpty(detail.TimingsToday) && string.IsNullOrEmpty(detail.TimingsToday);
    //&& string.IsNullOrEmpty(detail.Queue)

    public static bool IsNewBase64Valid(CardDetailDto? detail) => detail != null &&
      (ImageInfoBase64Url.IsBase64(detail?.Image1) || ImageInfoBase64Url.IsBase64(detail?.Image2)
        || ImageInfoBase64Url.IsBase64(detail?.Image3) || ImageInfoBase64Url.IsBase64(detail?.Image4)
        || ImageInfoBase64Url.IsBase64(detail?.Image5) || ImageInfoBase64Url.IsBase64(detail?.Image6));

    public static bool IsNotImageUrlNorBase64(CardDetailDto? detail) => detail == null ? true :
       ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image1) &&
        ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image2) &&
        ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image3) &&
        ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image4) &&
        ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image5) &&
        ImageInfoBase64Url.IsNotImageUrlNorBase64(detail.Image6);
    public void ResetImages()
        {
        Image1 = null;
        Image2 = null;
        Image3 = null;
        Image4 = null;
        Image5 = null;
        Image6 = null;
        }
    //IEquatable<CardProduct> implementation
    public bool Equals(CardDetailDto? otherProduct)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (otherProduct == null) return false; // Not the same type
        return Id == otherProduct.Id && EqualImages(otherProduct)
            && DetailDescription == otherProduct.DetailDescription
            && TimingsToday == otherProduct.TimingsToday
            && TimingsUsual == otherProduct.TimingsUsual;
        //&& Queue == otherProduct.Queue;
        }

    public bool EqualImages(CardDetailDto? otherProduct)//compares without id
        {//usage bool isEqual1 = person1.EqualImages(person2);
        if (otherProduct == null) return false; // Not the same type

        //IdCardBrand == otherCard.IdCardBrand //here wont check for id
        return Image1 == otherProduct.Image1 && Image2 == otherProduct.Image2 // Compare properties
            && Image3 == otherProduct.Image3 && Image4 == otherProduct.Image4
            && Image5 == otherProduct.Image5 && Image6 == otherProduct.Image6;
        }
    public static (bool updateRequired, bool addRequired) UpdateExistingDbEntity(CardDetailDto? existingEntity, CardDetailDto? newUiModifiedEntity)
        {
        if (existingEntity == null && newUiModifiedEntity == null || newUiModifiedEntity == null ||
            existingEntity != null && existingEntity.Equals(newUiModifiedEntity))
            return (updateRequired: false, addRequired: false);
        if (existingEntity == null)
            {
            existingEntity ??= newUiModifiedEntity;//existing null so overwrite
            return (updateRequired: false, addRequired: true);//required to Add
            }
        else
            {
            existingEntity.IsOpenNow = newUiModifiedEntity.IsOpenNow;
            existingEntity.TimingsToday = newUiModifiedEntity.TimingsToday;
            existingEntity.TimingsUsual = newUiModifiedEntity.TimingsUsual;
            existingEntity.Queue = newUiModifiedEntity.Queue;
            existingEntity.DetailDescription = newUiModifiedEntity.DetailDescription;
            existingEntity.MoreImages = newUiModifiedEntity.MoreImages;

            existingEntity.Image1 = newUiModifiedEntity.Image1;
            existingEntity.Image2 = newUiModifiedEntity.Image2;
            existingEntity.Image3 = newUiModifiedEntity.Image3;
            existingEntity.Image4 = newUiModifiedEntity.Image4;
            existingEntity.Image5 = newUiModifiedEntity.Image5;
            existingEntity.Image6 = newUiModifiedEntity.Image6;

            existingEntity.YouTubeVideoLink = newUiModifiedEntity.YouTubeVideoLink;
            existingEntity.ExternalImageLink = newUiModifiedEntity.ExternalImageLink;
            return (updateRequired: true, addRequired: false);
            }
        }

    public static (bool updateRequired, bool addRequired) UpdateExistingDbEntity(CardDetail? existingEntity, CardDetailDto? newUiModifiedEntity)
        {
        if (existingEntity == null && newUiModifiedEntity == null || newUiModifiedEntity == null ||
            existingEntity != null && existingEntity.Equals(newUiModifiedEntity))
            return (updateRequired: false, addRequired: false);
        if (existingEntity == null)
            {
            existingEntity ??= newUiModifiedEntity;//existing null so overwrite
            return (updateRequired: false, addRequired: true);//required to Add
            }
        else
            {
            existingEntity.IsOpenNow = newUiModifiedEntity.IsOpenNow;
            //existingEntity.TimingsToday = OpenCloseTiming.SerializeTimings(newUiModifiedEntity.TimingsToday);//todo add is default check
            //existingEntity.TimingsUsual = OpenCloseTimingsOfDay.SerializeTimingsUsual(newUiModifiedEntity.TimingsUsual);
            existingEntity.Queue = newUiModifiedEntity.Queue.Serialize();//todo  need to verify
            existingEntity.DetailDescription = newUiModifiedEntity.DetailDescription;
            existingEntity.IsOpenNow = newUiModifiedEntity.IsOpenNow;
            existingEntity.TimingsToday = newUiModifiedEntity.TimingsToday;
            existingEntity.TimingsUsual = newUiModifiedEntity.TimingsUsual;
            //existingEntity.Queue = newUiModifiedEntity.Queue;
            //existingEntity.MoreImages = newUiModifiedEntity.MoreImages;

            //existingEntity.Image1 = newUiModifiedEntity.Image1;
            //existingEntity.Image2 = newUiModifiedEntity.Image2;
            //existingEntity.Image3 = newUiModifiedEntity.Image3;
            //existingEntity.Image4 = newUiModifiedEntity.Image4;
            //existingEntity.Image5 = newUiModifiedEntity.Image5;
            //existingEntity.Image6 = newUiModifiedEntity.Image6;

            existingEntity.YouTubeVideoLink = newUiModifiedEntity.YouTubeVideoLink;
            existingEntity.ExternalImageLink = newUiModifiedEntity.ExternalImageLink;
            return (updateRequired: true, addRequired: false);
            }
        }
    }

#pragma warning restore 
