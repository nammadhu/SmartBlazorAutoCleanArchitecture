using FluentValidation;
using PublicCommon;
using PublicCommon.Common;
using Shared.DTOs;

namespace Shared.Features.Cards.Commands;

public class CU_CardCommand : Card, IRequest<BaseResult<iCardDto>>, IEquatable<CU_CardCommand>
    {
    //on create card+data
    //on update only card
    public Guid Operator { get; set; }

    public new int IdTown { get; set; }
    public new TownDto? Town { get; set; }

    public bool IsForVerifiedCard { get; set; } = false;
    public new bool? IsVerified { get; }
    public new bool? IsAdminVerified { get; }
    public new bool? IsPeerVerified { get; }
    public new List<CardApproval>? ApprovedPeerCardIds { get; }

    public List<CardApproval>? SelectedApprovalCards { get; set; }

    public List<CardApproval>? VerifierCardsToSelect { get; set; }

    public new ImageInfoBase64Url? Image1 { get; set; } = new();

    public new ImageInfoBase64Url? Image2 { get; set; }

    public new CardData? CardData { get; set; }

    public bool IsKeyInfoChanged { get; set; }

    ////dont post these 2
    //public new iCardDto? VerifiedCard { get; set; }//for draftcard case,dont POST these

    //public iCardDto? DraftCard { get; set; }//for verified card case,dont POST these

    //below roles are required,after save had to fetch Full result so
    //public bool IsCardCreator { get; set; }
    //public bool IsCardOwner { get; set; }
    //public bool IsCardVerifiedOwner { get; set; }
    //public bool IsCardVerifiedReviewer { get; set; }

    //if admin then had to passs userid of his search parameter also
    public Guid? IdCustomer { get; set; }

    private const string keyword = "BI";

    public string FileName(string? actualName) => $"{keyword}_{Id}_{actualName ?? ""}";

    public void FileNamesUpdate()
        {
        if (Image1 != null && ImageInfoBase64Url.IsBase64(Image1)) Image1.FileName = FileName(Image1.FileName);
        if (Image2 != null && ImageInfoBase64Url.IsBase64(Image2)) Image2.FileName = FileName(Image2.FileName);
        }

    public List<ImageInfo> ToUploadImages()
        {
        List<ImageInfo> list = new List<ImageInfo>();
        FileNamesUpdate();
        if (Image1 != null && ImageInfoBase64Url.IsBase64(Image1)) list.Add(new ImageInfo(Image1, nameof(Image1)));
        if (Image2 != null && ImageInfoBase64Url.IsBase64(Image2)) list.Add(new ImageInfo(Image2, nameof(Image2)));
        return list;
        }

    public (List<ImageInfo> listToUpload, List<ImageInfo> listToDelete) GetUploadAndDeleteImages(_CardBase? existing)
        {
        List<ImageInfo> listToUpload = new List<ImageInfo>();
        List<ImageInfo> listToDelete = new List<ImageInfo>();
        FileNamesUpdate();
        ImageInfoBase64Url.LoadUploadDelete(Image1, nameof(Image1), existing?.Image1, listToUpload, listToDelete);
        ImageInfoBase64Url.LoadUploadDelete(Image2, nameof(Image2), existing?.Image2, listToUpload, listToDelete);
        return new(listToUpload, listToDelete);
        }

    public static bool IsNewBase64Valid(CU_CardCommand? brand)
       => ImageInfoBase64Url.IsBase64(brand?.Image1) || ImageInfoBase64Url.IsBase64(brand?.Image2);

    public static bool IsNotImageUrlNorBase64(CU_CardCommand? card) => card == null ? true :
   ImageInfoBase64Url.IsNotImageUrlNorBase64(card.Image1) && ImageInfoBase64Url.IsNotImageUrlNorBase64(card.Image2);

    public void ClientToServerDataExclusion()
        {
        if (Image1 != null)
            Image1 = Image1.BeforePostingCorrection();
        if (Image2 != null)
            Image2 = Image2.BeforePostingCorrection();
        Type = null;
        Town = null;

        Town = null;
        //VerifiedCard = null;
        //DraftCard = null;
        IdOwner = Guid.Empty;
        Operator = Guid.Empty;
        if (ApprovedPeerCardIds.HasData() && ApprovedPeerCardIds != null)
            ApprovedPeerCardIds.RemoveAll(x => x.IdCardOfApprover == Id);
        VerifierCardsToSelect = null;

        //IsForDraftCard = null;//dont exclude this
        //IsVerifiedEntryExists = null;//dont exclude this
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
        return Equals(this, other) && CardData.Equals(CardData, other.CardData);//&& CardData.EqualsKeyInfo(this.CardData, other.CardData);
        }

    public static bool Equals(CU_CardCommand? source, CU_CardCommand? other)//compares including id
        {//usage bool isEqual1 = person1.EqualsKeyInfo(person2);
        if (source == null && other == null) return true;
        if (source == null || other == null) return false;

        return Card.Equals(source, other) && source.Id == other.Id
            //source.IsVerified == other.IsVerified//ideally this should not be checked
            && EqualImages(source, other) &&

            CardApproval.Equals(source.ApprovedPeerCardIds, other.SelectedApprovalCards);
        //image1,2,isForVerified,draft
        }

    public static (bool updateRequired, bool addRequired) UpdateExistingDbEntity(_CardBase? existingEntityForUpdate, CU_CardCommand? newUiModifiedEntity)
        {
        if (existingEntityForUpdate == null && newUiModifiedEntity == null || newUiModifiedEntity == null ||
            existingEntityForUpdate != null && existingEntityForUpdate.Equals(newUiModifiedEntity))
            return (updateRequired: false, addRequired: false);
        if (existingEntityForUpdate == null)
            {
            //existingEntityForUpdate ??=  newUiModifiedEntity;//existing null so overwrite
            return (updateRequired: false, addRequired: true);//required to Add
            }
        else
            {
            existingEntityForUpdate.IdCardType = newUiModifiedEntity.IdCardType;
            existingEntityForUpdate.IdTown = newUiModifiedEntity.IdTown;
            existingEntityForUpdate.Title = newUiModifiedEntity.Title;
            existingEntityForUpdate.SubTitle = newUiModifiedEntity.SubTitle;
            existingEntityForUpdate.Address = newUiModifiedEntity.Address;
            existingEntityForUpdate.IsVerified = newUiModifiedEntity.IsForVerifiedCard == true;

            //existingEntityForUpdate.Active = newUiModifiedEntity.Active;
            //existingEntityForUpdate.IdOwner = newUiModifiedEntity.IdOwner;
            //existingEntityForUpdate.IsClaimed = newUiModifiedEntity.IsClaimed;

            //existingEntityForUpdate.Image1 = newUiModifiedEntity.Image1;
            //existingEntityForUpdate.Image2 = newUiModifiedEntity.Image2;
            return (updateRequired: true, addRequired: false);
            }
        }

    public class CreateUpdateCardCommandValidator : AbstractValidator<CU_CardCommand>
        {
        public CreateUpdateCardCommandValidator()//(ITranslator translator)
            {
            RuleFor(p => p.Title)
                .NotNull().WithMessage("Required").NotEmpty().WithMessage("Required")
                .MinimumLength(3).WithMessage("Atleast 3 Characters Required");
            RuleFor(p => p.IdCardType)
                .NotNull().WithMessage("Type is Required")
                .NotEqual(-1).WithMessage("Type is Required")
            .NotEmpty().GreaterThan(0).WithMessage("iCard Type is Must");
            RuleFor(p => p.IdTown).NotNull().NotEqual(-1)
            .NotEmpty().GreaterThan(0).WithMessage("Town is Must");
            RuleFor(p => p.Operator).NotNull().NotEqual(Guid.Empty)
                .WithMessage("Operator Id Missing");
            }
        }
    }
