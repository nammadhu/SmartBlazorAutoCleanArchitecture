using FluentValidation;
using PublicCommon.Common;
using Shared.DTOs;
using Shared.Features.Cards.Commands;

namespace Shared.Features.Towns.Commands;

public class CU_TownCommand : Town, IRequest<BaseResult<TownDto>>
    {
    //  public new ICollection<CardDto> CardVerifiedItems { get; set; } = new List<CardDto>();

    //should not be loaded on town page,instead only for creator appearance selection
    //    public new ICollection<CardDto> Cards { get; set; } = new List<CardDto>();

    public new ImageInfoBase64Url? Image1 { get; set; } = new();
    public new ImageInfoBase64Url? Image2 { get; set; } //= new();
    public new ImageInfoBase64Url? Image3 { get; set; }
    public new ImageInfoBase64Url? Image4 { get; set; }
    public new ImageInfoBase64Url? Image5 { get; set; }
    public new ImageInfoBase64Url? Image6 { get; set; }
    public new List<ImageInfoBase64Url>? MoreImages { get; set; }

    //    public static bool IsNullOrDefaultImagesUrls(Town? card) => (card == null ||
    //(!ImageInfoBase64Url.IsUrl(card.Image1) && !ImageInfoBase64Url.IsUrl(card.Image2) &&
    //        !ImageInfoBase64Url.IsUrl(card.Image3) && !ImageInfoBase64Url.IsUrl(card.Image4) &&
    //        !ImageInfoBase64Url.IsUrl(card.Image5) && !ImageInfoBase64Url.IsUrl(card.Image6)
    //        ));

    public static bool IsNotImageUrlNorBase64(CU_TownCommand? town) => town == null ||
        ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image1) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image2) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image3) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image4) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image5) &&
         ImageInfoBase64Url.IsNotImageUrlNorBase64(town.Image6);

    public static bool IsNewBase64Valid(CU_TownCommand? townCommand) => townCommand != null &&
    (ImageInfoBase64Url.IsBase64(townCommand.Image1) || ImageInfoBase64Url.IsBase64(townCommand.Image2)
    || ImageInfoBase64Url.IsBase64(townCommand.Image3) || ImageInfoBase64Url.IsBase64(townCommand.Image4)
    || ImageInfoBase64Url.IsBase64(townCommand.Image5) || ImageInfoBase64Url.IsBase64(townCommand.Image6));

    public static bool IsNullOrDefault(CU_TownCommand? town) => town == null ? true :
   IsNotImageUrlNorBase64(town) && string.IsNullOrEmpty(town.DetailDescription);

    private const string keyword = "TI";

    public string FileName(string? actualName) => $"{keyword}_{Id}_{actualName ?? ""}";

    public void FileNamesUpdate()
        {
        if (Image1 != null && ImageInfoBase64Url.IsBase64(Image1)) Image1.FileName = FileName(Image1.FileName);
        if (Image2 != null && ImageInfoBase64Url.IsBase64(Image2)) Image2.FileName = FileName(Image2.FileName);
        if (Image3 != null && ImageInfoBase64Url.IsBase64(Image3)) Image3.FileName = FileName(Image3.FileName);
        if (Image4 != null && ImageInfoBase64Url.IsBase64(Image4)) Image4.FileName = FileName(Image4.FileName);
        if (Image5 != null && ImageInfoBase64Url.IsBase64(Image5)) Image5.FileName = FileName(Image5.FileName);
        if (Image6 != null && ImageInfoBase64Url.IsBase64(Image6)) Image6.FileName = FileName(Image6.FileName);
        }

    public List<ImageInfo> ToUploadImages()
        {
        List<ImageInfo> list = new List<ImageInfo>();
        FileNamesUpdate();
        if (Image1 != null && ImageInfoBase64Url.IsBase64(Image1)) list.Add(new ImageInfo(Image1, nameof(Image1)));
        if (Image2 != null && ImageInfoBase64Url.IsBase64(Image2)) list.Add(new ImageInfo(Image2, nameof(Image2)));
        if (Image3 != null && ImageInfoBase64Url.IsBase64(Image3)) list.Add(new ImageInfo(Image3, nameof(Image3)));
        if (Image4 != null && ImageInfoBase64Url.IsBase64(Image4)) list.Add(new ImageInfo(Image4, nameof(Image4)));
        if (Image5 != null && ImageInfoBase64Url.IsBase64(Image5)) list.Add(new ImageInfo(Image5, nameof(Image5)));
        if (Image6 != null && ImageInfoBase64Url.IsBase64(Image6)) list.Add(new ImageInfo(Image6, nameof(Image6)));
        return list;
        }

    public (List<ImageInfo> listToUpload, List<ImageInfo> listToDelete) GetUploadAndDeleteImages(Town? existing)
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

    public void ClientToServerDataExclusion()
        {
        if (Image1 != null) Image1 = Image1.BeforePostingCorrection();
        if (Image2 != null) Image2 = Image2.BeforePostingCorrection();
        if (Image3 != null) Image3 = Image3.BeforePostingCorrection();
        if (Image4 != null) Image4 = Image4.BeforePostingCorrection();
        if (Image5 != null) Image5 = Image5.BeforePostingCorrection();
        if (Image6 != null) Image6 = Image6.BeforePostingCorrection();
        }

    public static bool EqualImages(CU_CardCommand? source, CU_CardCommand? other)//compares without id
        {//usage bool isEqual1 = person1.EqualImages(person2);
        if (source == null && other == null) return true;
        if (source == null || other == null) return false;

        //IdCardBrand == otherCard.IdCardBrand //here wont check for id
        return ImageInfoBase64Url.Equals(source.Image1, other?.Image1)
            && ImageInfoBase64Url.Equals(source.Image2, other?.Image2); // Compare properties
        }

    public bool Equals(CU_CardCommand? other)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (other == null) return false; // Not the same type
        return Equals(this, other);//&& CardData.EqualsKeyInfo(this.CardData, other.CardData);
        }

    public static bool Equals(CU_CardCommand? source, CU_CardCommand? other)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (source == null && other == null) return true;
        if (source == null || other == null) return false;

        return Card.Equals(source, other) && source.Id == other.Id
            && CardData.Equals(source.CardData, other.CardData)
            && EqualImages(source, other)
            && CardApproval.Equals(source.ApprovedPeerCardIds, other.SelectedApprovalCards);
        //image1,2,isforverified,draft
        }

    //mapping in towndto
    //private class Mapping : AutoMapper.Profile
    //    {
    //    public Mapping()
    //        {
    //        CreateMap<CreateUpdateTownCommand, Town>().ReverseMap();
    //        }
    //    }
    }

public class CreateUpdateTownCommandValidator : AbstractValidator<CU_TownCommand>
    {
    public CreateUpdateTownCommandValidator()//(ITranslator translator)
        {
        RuleFor(p => p.Title)
            .NotNull();
        //.WithName(p => translator[nameof(p.Name)]);
        //RuleFor(p => p.ShortName)
        //    .NotNull();
        //.WithName(p => translator[nameof(p.ShortName)]);
        }
    }
