using AutoMapper;
using BASE.Common;
using SHARED.DTOs;
using SHARED.Features.Cards.Commands;
using SHARED.Features.CardTypes.Commands;
using SHARED.Features.Towns.Commands;

namespace SHARED;

public class Mapping : Profile
    {
    public Mapping()
        {
        /*Most using are Card_Draft=>iCardDto & Card_Verified=>iCardDto
         * Once taken into ui,then iCardDto=>CreateUpdateCardCommand & passed to backend api
         * in api CreateUpdateCardCommand=>Card_Draft
         */
        CreateMap<UserDetailBase, UserDetailDto>()
             .ForMember(dto => dto.iCards, detailBase => detailBase.Ignore())
            .ForMember(dto => dto.CardApprovals, detailBase => detailBase.Ignore())
            .ReverseMap();
        CreateMap<UserDetail, UserDetailBase>()
            //.ForMember(dto => dto.CardsDraft, detailBase => detailBase.Ignore())
            //above is not required because on source it doesnt exist
            .ReverseMap();
        CreateMap<UserDetail, UserDetailDto>()
            .ForMember(dto => dto.iCards, detailBase => detailBase.MapFrom(s => s.iCards))
            //.ForMember(dto => dto.CardApprovals, detailBase => detailBase.Ignore())//not required as both source,destination same
            .ReverseMap();

        CreateMap<Card, CardDto>()
          //.ForMember(d => d.Brand, opt => opt.MapFrom(src => src.Brand))
          //.ForMember(d => d.Product, opt => opt.MapFrom(src => src.Product))

          .ForMember(d => d.DraftChanges, opt => opt.MapFrom(src => src.DraftChanges))
          .ForMember(d => d.TownsOfVerifiedCard, opt => opt.Ignore())

          .ForMember(d => d.Image1, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image1) && ImageInfoBase64Url.IsUrl(src.Image1) ? new ImageInfoBase64Url(src.Image1) : null))
          .ForMember(d => d.Image2, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image2) && ImageInfoBase64Url.IsUrl(src.Image2) ? new ImageInfoBase64Url(src.Image2) : null))
          .ForMember(d => d.CardData, opt => opt.MapFrom(src => src.CardData))
          .ForMember(d => d.CardDetail, opt => opt.MapFrom(src => src.CardDetail))
          .ForMember(x => x.ApprovedPeerCardIds, opt => opt.MapFrom(s => s.ApprovedPeerCardIds != null && s.ApprovedPeerCardIds.Count > 0 ? s.ApprovedPeerCardIds.Select(a => new CardApproval() { IdCard = a, IsVerified = true }) : new List<CardApproval>()))
          .ForMember(d => d.Town, opt => opt.Ignore())
          .ForMember(d => d.Type, opt => opt.Ignore())
          //.ForMember(d => d.ApprovedCards, opt => opt.MapFrom(src => src.approv.CardApprovals == null ? null : CardApprovalSetTitleAndNullifyNestedCards(src.CardApprovals)))
          //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
          //  .ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
          //  .ForMember(d => d.UserId, opt => opt.Ignore())
          .ReverseMap();

        CreateMap<CardDto, Card>()//mostly of no use
              .ForMember(d => d.Town, opt => opt.Ignore())
          .ForMember(d => d.Type, opt => opt.Ignore())
            .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
            .ForMember(d => d.ApprovedPeerCardIds, opt => opt.Ignore());

        CreateMap<CardDto, Card_DraftChanges>()//mostly of no use
           .ForMember(d => d.Town, opt => opt.Ignore())
          .ForMember(d => d.Type, opt => opt.Ignore())
          .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
          .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
          .ReverseMap();

        //on edit
        //<carddto,cardcommand >
        //on save <cardcommand,card> or <cardcommand,carddraftchanges>
        //on page itself having all cards so no need of fetching additional approval cards required

        CreateMap<CardDto, CU_CardCommand>()//on editing
           .ForMember(x => x.ApprovedPeerCardIds, opt => opt.MapFrom(s => s.ApprovedPeerCardIds))
           .ForMember(x => x.SelectedApprovalCards, opt => opt.MapFrom(s => s.ApprovedPeerCardIds))
           .ForMember(x => x.IsForVerifiedCard, opt => opt.MapFrom(s => s.IsVerified))
           .ReverseMap();

        //while saving this used
        CreateMap<CU_CardCommand, Card>()//on saving
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
            .ForMember(d => d.CardData, opt => opt.MapFrom(src => src.CardData))
            .ForMember(d => d.CardDetail, opt => opt.MapFrom(src => src.CardDetail))
            .ForMember(d => d.Town, opt => opt.Ignore())
            .ForMember(d => d.Type, opt => opt.Ignore())
             .ForMember(d => d.ApprovedPeerCardIds, opt => opt.Ignore())//for admin sake then add an entry on server side
                                                                        //.MapFrom(src => CardApprovalSetTitleAndNullifyNestedCards(src.CardApprovals)))
                                                                        //for self no passing of approved instead only selected and it goes to draft table

            //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
            //.ForMember(d => d.IdOwner, opt => opt.Ignore())
            //.ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
            //since detaildto => detail so
            //.ForMember(d => d.Card_Verified, null)//shouldnot post from client//this is done on Command method also
            //.ForMember(d => d.CardData, opt => opt.MapFrom(src => src.CardData))//this is not necessary bcz both same name & type
            .ReverseMap();
        CreateMap<CU_CardCommand, Card_DraftChanges>()
          .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
            .ForMember(d => d.Town, opt => opt.Ignore())
            .ForMember(d => d.Type, opt => opt.Ignore())
          .ForMember(d => d.CardApprovals, opt => opt.MapFrom(src => CardApprovalSetTitleAndNullifyNestedCards(src.SelectedApprovalCards)))
          .ReverseMap();

        CreateMap<CU_CardDataCommand, CardData>()
            .ReverseMap();
        CreateMap<CU_CardDetailCommand, CardDetail>()
            .ReverseMap();
        //may be not used
        //CreateMap<Card_Draft, CreateUpdateCardCommand>()
        //    .ForMember(d => d.IdCard, opt => opt.MapFrom(src => src.IdCard))
        //    .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.IsUrl(src.Image1) ? new ImageInfoBase64Url(src.Image1) : null))
        //    .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.IsUrl(src.Image2) ? new ImageInfoBase64Url(src.Image2) : null))
        //    .ReverseMap();

        //below is necessary for above
        CreateMap<CardDetailDto, CardDetailDto>().ReverseMap();
        CreateMap<CardData, CardData>().ReverseMap();

        CreateMap<Card_DraftChanges, Card>()
            //.ForMember(d => d.Brand, opt => opt.MapFrom(src => src.Brand))
            //.ForMember(d => d.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            //.ForMember(d => d.Card_Verified, opt => opt.Ignore())
            .ForMember(d => d.ApprovedPeerCardIds, opt => opt.MapFrom(src => src.CardApprovals != null && src.CardApprovals.Count > 0 ? src.CardApprovals.Where(x => x.IsVerified == true).Select(x => x.IdCard) : null))
            .ForMember(d => d.CardData, opt => opt.Ignore())
            .ForMember(d => d.CardDetail, opt => opt.Ignore())
            .ReverseMap();//not sure what all looses,so be cautious

        //since names are same and no separate mapping required so commented all extra

        CreateMap<CardDetail, CardDetailDto>()
            .ForMember(d => d.Image1, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image1) && ImageInfoBase64Url.IsUrl(src.Image1) ? new ImageInfoBase64Url(src.Image1) : null))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image2) && ImageInfoBase64Url.IsUrl(src.Image2) ? new ImageInfoBase64Url(src.Image2) : null))
            .ForMember(d => d.Image3, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image3) && ImageInfoBase64Url.IsUrl(src.Image3) ? new ImageInfoBase64Url(src.Image3) : null))
            .ForMember(d => d.Image4, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image4) && ImageInfoBase64Url.IsUrl(src.Image4) ? new ImageInfoBase64Url(src.Image4) : null))
            .ForMember(d => d.Image5, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image5) && ImageInfoBase64Url.IsUrl(src.Image5) ? new ImageInfoBase64Url(src.Image5) : null))
            .ForMember(d => d.Image6, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image6) && ImageInfoBase64Url.IsUrl(src.Image6) ? new ImageInfoBase64Url(src.Image6) : null))
             .ForMember(d => d.MoreImages, opt => opt.Ignore())
            //.ForMember(d => d.MoreImages,//currently not using
            //     opt => opt.MapFrom(src => src.MoreImages == null ? new List<ImageInfoBase64Url>() :
            //     src.MoreImages.Split(',', StringSplitOptions.None)//.ToList().Where(c => c.HasText())
            //     .Select(x => new ImageInfoBase64Url(x))))

            //.ForMember(d => d.TimingsUsual, opt => opt.MapFrom(src => OpenCloseTimingsOfDay.DeserializeTimingsUsual(src.TimingsUsual)))
            //.ForMember(d => d.TimingsToday, opt => opt.MapFrom(src => OpenCloseTiming.DeSerializeTimings(src.TimingsToday, src.TimingsUsual)))

            .ForMember(d => d.iCard, opt => opt.Ignore())
            ;
        //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
        //.ForMember(d => d.LastModifiedBy, opt => opt.Ignore());
        //.ReverseMap();//dont apply this
        CreateMap<CardDetailDto, CardDetail>()
           .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
           .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
           .ForMember(d => d.Image3, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image3)))
           .ForMember(d => d.Image4, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image4)))
           .ForMember(d => d.Image5, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image5)))
           .ForMember(d => d.Image6, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image6)))
           //.ForMember(d => d.MoreImages, opt => opt.MapFrom(src => src.MoreImages))//currently not using
           .ForMember(d => d.MoreImages, opt => opt.Ignore())
            //.ForMember(d => d.TimingsUsual, opt => opt.MapFrom(src => OpenCloseTimingsOfDay.SerializeTimingsUsual(src.TimingsUsual)))
            //.ForMember(d => d.TimingsToday, opt => opt.MapFrom(src => OpenCloseTiming.SerializeTimings(src.TimingsToday)))
            //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
            //.ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
            ;

        //below are for towns and cardtypes
        CreateMap<CardType, CardTypeDto>()
            //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
            //.ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CU_CardTypeCommand, CardTypeDto>().ReverseMap();

        CreateMap<Town, TownDto>()
            //.ForMember(dto => dto.TownCard, domain => domain.MapFrom(town => town.TownCard))
            //.ForMember(dto => dto.CardsVerified, domain => domain.MapFrom(town => town.CardsVerified))
            //.ForMember(dto => dto.Cards, domain => domain.MapFrom(town => town.Cards))
            //.ForMember(d => d.CreatedBy, opt => opt.Ignore())
            //.ForMember(d => d.LastModifiedBy, opt => opt.Ignore())

            //.ForMember(d => d.VerifiedCardsAdditional, opt => opt.MapFrom(src => src.VerifiedCardsAdditional.Select(z => z.VerifiedCard).ToList()))
            //.ForMember(d => d.VerifiedCards, opt => opt.MapFrom(src => src.VerifiedCards))
            //.ForMember(d => d.DraftCards, opt => opt.MapFrom(src => src.DraftCards))
            //may be abover required or not clear,need to check

            .ForMember(d => d.Image1, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image1) && ImageInfoBase64Url.IsUrl(src.Image1) ? new ImageInfoBase64Url(src.Image1) : null))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image2) && ImageInfoBase64Url.IsUrl(src.Image2) ? new ImageInfoBase64Url(src.Image2) : null))
            .ForMember(d => d.Image3, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image3) && ImageInfoBase64Url.IsUrl(src.Image3) ? new ImageInfoBase64Url(src.Image3) : null))
            .ForMember(d => d.Image4, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image4) && ImageInfoBase64Url.IsUrl(src.Image4) ? new ImageInfoBase64Url(src.Image4) : null))
            .ForMember(d => d.Image5, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image5) && ImageInfoBase64Url.IsUrl(src.Image5) ? new ImageInfoBase64Url(src.Image5) : null))
            .ForMember(d => d.Image6, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Image6) && ImageInfoBase64Url.IsUrl(src.Image6) ? new ImageInfoBase64Url(src.Image6) : null))
             .ForMember(d => d.MoreImages, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CU_TownCommand, TownDto>()
            .ReverseMap();

        CreateMap<Town, CU_TownCommand>()
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CU_TownCommand, Town>()
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.LastModifiedBy, opt => opt.Ignore())
            .ForMember(d => d.Image1, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image1)))
            .ForMember(d => d.Image2, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image2)))
            .ForMember(d => d.Image3, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image3)))
            .ForMember(d => d.Image4, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image4)))
            .ForMember(d => d.Image5, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image5)))
            .ForMember(d => d.Image6, opt => opt.MapFrom(src => ImageInfoBase64Url.GetUrl(src.Image6)))
           //.ForMember(d => d.MoreImages, opt => opt.MapFrom(src => src.MoreImages))//currently not using
           .ForMember(d => d.MoreImages, opt => opt.Ignore())
         .ReverseMap();
        }

    public static ICollection<CardApproval>? CardApprovalSetTitleAndNullifyNestedCards(ICollection<CardApproval>? cardApprovals)
        {
        if (cardApprovals != null && cardApprovals?.Count > 0)
            {
            foreach (var item in cardApprovals)
                {
                if (item != null)
                    item.SetTitleAndNullifyNestedCards();
                }
            return cardApprovals;
            }
        else return null;
        }

    /*
    public static List<CardApproval>? CardApprovalsToVerifierCardToSelect(ICollection<Domain.CardApproval>? approvals)
        {
        List<CardApproval> result = [];
        if (approvals != null && approvals.Count > 0)
            {
            approvals.ToList().ForEach(a => result.Add(new CardApproval()
                {
                IdCardOfApprover = a.IdCardOfApprover,
                Title = a.ApproverCard?.Title ?? "",
                //Type = a.IdCardOfApprover > 0 ? "Card_Draft" : "Town"
                }));
            return result;
            }
        return [];
        }
    public static List<CardApproval>? CardDtoToVerifierCardToSelect(ICollection<iCardDto>? approvals)
        {
        List<CardApproval> result = [];
        if (approvals != null && approvals.Count > 0)
            {
            approvals.ToList().ForEach(x => result.Add
               (new CardApproval() { Title = x.Title, IdCardOfApprover = x.IdCard, Type = "Card_Draft" })
                );
            return result;
            }
        return [];
        }

    public static List<Domain.CardApproval>? VerifierCardToSelectToCardApproval(ICollection<CardApproval>? toSelectList)
        {
        List<Domain.CardApproval> result = [];
        if (toSelectList != null && toSelectList.Count > 0)
            {
            toSelectList.ToList().ForEach(x => result.Add
               (new Domain.CardApproval()
                   {
                   IdCard = x.IdCard,
                   IdCardOfApprover = x.Type == "Card_Draft" ? x.IdCardOfApprover : null,
                   IdTown = x.IdCardOfApprover
                   }
                ));
            }
        return result;
        }*/
    //private List<Card_Verified>? TownVerifiedCardsUpdate(Town town)
    //    {
    //    if (town != null && town.TownToCardsVerified != null)
    //        {
    //        return town.TownToCardsVerified.Where(x => !(x.IdTown == town.IdTown && x.Card_Draft.IdCardType == ConstantsTown.TownTypeId)).Select(z => z.Card_Draft).ToList();
    //        }
    //    return null;
    //    }

    //private List<iCardDto>? TownVerifiedCardsUpdate(Town town)
    //    {
    //    if (town != null && town.VerifiedCards!= null)
    //        {
    //        return town.VerifiedCards.Select(z => z.Card_Verified).ToList();
    //        }
    //    return null;
    //    }
    }
