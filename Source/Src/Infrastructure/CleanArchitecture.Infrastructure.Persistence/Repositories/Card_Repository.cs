using AutoMapper;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.UserInterfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Features.Cards.Queries;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class Card_DraftChangesRepository(DbContextProvider dbContextProvider) : GenericRepository<Card_DraftChanges>(dbContextProvider), ICard_DraftChangesRepository
    {
    public Task<List<iCardDto>> GetVerifiedCardsOfTypeInTown(GetVerifiedCardsOfTypeInTownQuery query, CancellationToken cancellationToken)
        {
        throw new NotImplementedException();
        }
    }

public class Card_Repository(DbContextProvider dbContextProvider, IMapper mapper,IAuthenticatedUserService authenticatedUserService,
IAzImageStorage azImageStorage, IAccountServices accountServices) : GenericRepository<Card>(dbContextProvider: dbContextProvider), ICardRepository
    {//IIdentityRepository accountServices,  ICardApprovalRepository townCardApprovalRepository,  IUserDetailRepository userDetailRepository,
    private const int ResultLimit = 30;
    private const int MaxCardsOnSelectedPage = 300;
    //private readonly DbSet<Town2VerifiedCard> dbTown2VerifiedCard;

    private readonly DbSet<Card> dbCard = dbContextProvider.DbContext.Set<Card>();
    private readonly DbSet<Card_DraftChanges> dbCardDraft = dbContextProvider.DbContext.Set<Card_DraftChanges>();
    private readonly ApplicationDbContext dbContext = dbContextProvider.DbContext;

    /*
     *getusercardswithdraft()
     *
     */

    public async Task<List<iCardDto>> GetUserCards(Guid userId, CancellationToken cancellationToken)
        {
        //this should not be executed for admin,as they will be having lot of cards makes large set
        UserDetailDto dto = new();
        var userCards = await dbCard.Where(x => x.IdOwner == userId || x.CreatedBy == userId)
            .Include(x => x.CardData)//mostly by default included
            .Include(x => x.CardDetail)//mostly by default included
            .Include(x => x.VerifiedCardDisplayDates)//mostly by default included
            .Include(x => x.CardRatings)//mostly by default included
            .Include(x => x.AdditionalTownsOfVerifiedCard)//mostly by default included
            .Include(x => x.DraftChanges)
            .ToListAsync(cancellationToken);
        return [.. userCards.Select(x => mapper.Map<iCardDto>(x))];
        }

 /*
    public async Task<UserDetailDto> GetUserCardsMoreDetails(GetCardsOfUserQuery query, CancellationToken cancellationToken)
        {
        dbCard.Where(x => x.CreatedBy == query.UserId)
      //if creator or owner , then fetch only on Card_VerifiedEntries table
      //if VerifiedCardOwner, then fetch on Card_VerifiedEntries with ownerId column
      //returns list<verifiedId>,list<Card_VerifiedEntries> drafts,list<Card_VerifiedEntries> waitingForMyApproval

        //List<iCardDto> verifiedItems = [];
        //if (isVerifiedCardOwner)
        //    verifiedItems = await dbCardVerified.AsNoTracking()
        //        .Where(x => x.IdOwner == userId && x.IdCardType != ConstantsTown.TownTypeId)
        //        .Include(x => x.DraftCard).ThenInclude(c => c.CardDataEntries)
        //        .Include(x => x.DraftCard).ThenInclude(c => c.CardDetails)//not required for home page
        //         .OrderByDescending(x => x.LastModified ?? x.Created)
        //        .Take(ResultLimit)//not sure but better
        //        .Select(p => mapper.Map<iCardDto>(p))//don't use p.To<TownCard, TownCardDto>())
        //        .ToListAsync(cancellationToken);

      List<iCardDto> draftCards = [];
        if (query.IsCardCreator || query.IsCardOwner)
            draftCards = await dbDraftCard.AsNoTracking().Where(x => x.IdOwner == query.UserId)

                //(query.IsCardCreator ? (x.CreatedBy == query.UserId || x.IdOwner == query.UserId) : true)
                //&& (query.IsCardOwner ? x.IdOwner == query.UserId : true)

                //x.IdCardType != ConstantsTown.TownTypeId &&
                //(query.IsCardCreator ? x.CreatedBy == query.UserId : true)
                //&& (query.IsCardOwner ? x.IdOwner == query.UserId : true)

                .Include(x => x.CardData)//mostly by default included
                .Include(x => x.CardDetail)//not required for home page
                .Include(x => x.CardApprovals).ThenInclude(x => x.ApproverCard)
                .Include(x => x.VerifiedCard)
                //ideally for verified cards fetch only if VerifiedOwner roles
                .OrderByDescending(x => x.LastModified ?? x.Created)
                .Take(ResultLimit)//not sure but better
                .Select(p => mapper.Map<iCardDto>(p))//don't use p.To<TownCard, TownCardDto>())
                .ToListAsync(cancellationToken);
        List<iCardDto> verifiedItems = draftCards.Where(x => x.IsItVerified()).ToList();
        draftCards.RemoveAll(x => x.IsItVerified());//so main list will not have Verified

        List<CleanArchitecture.Domain.CardApproval> cardApprovals = [];//TODO pending
        if (ListExtensions.HasData(verifiedItems))
            {
            foreach (iCardDto item in verifiedItems)
                {
                item.DraftCard = item;
                item.DraftCard.CardData = null;
                item.DraftCard.CardDetail = null;
                item.VerifiedCard = null;
                }
            //if (isVerifiedReviewer)
            //send user cardids,fetch which all marked these ids as approver,fetch those
            cardApprovals = await townCardApprovalRepository.GetCardApprovalsOfApprover(verifiedItems.Select(x => x.Id).ToList(), cancellationToken, isVerified: null);
            }
        return new UserDetailDto() { Id = query.UserId, CardsVerified = verifiedItems, CardsDraft = draftCards, CardApprovals = cardApprovals };
        }
    */

    /// <summary>
    /// this wont fetch town,instead only town cards. And drafts are only 100-verified
    /// </summary>
    /// <param name="townId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TownCardsDto> GetCardsOfTown(int townId, CancellationToken cancellationToken)
    //public async Task<Towns> GetTownByIdAsync(int id, Guid? userId)
        {
        //var today = DateOnly.FromDateTime(DateTime.Today);
        //todo must enable for production
        //timebeing not using date for listing
        //var town = await db.FindAsync(id);

        TownCardsDto townCards = new() { Id = townId };
        //include drafts with verified also
        townCards.VerifiedCards = await dbCard
            .Where(x => (x.IdTown == townId))
            .Include(p => p.CardData)
            .Include(x => x.CardDetail)
            .Include(x => x.VerifiedCardDisplayDates)
            .Include(d => d.DraftChanges)//this is holding data & detail again takeout this to avoid repeatation
            .Select(x => mapper.Map<iCardDto>(x))
            .ToListAsync(cancellationToken);
        //then fetch drafts
        //for admin this check has to be bypassed

        townCards.VerifiedCards.ForEach(x =>
          {
              if (x.CardData != null)
                  {
                  x.CardData.IdTown = townId;
                  x.CardData.IsVerified = x.IsVerified;
                  }
              if (x.CardDetail != null)
                  {
                  x.CardDetail.IdTown = townId;
                  x.CardDetail.IsVerified = x.IsVerified;
                  }
          });
        if (townCards.DraftCards?.Count > 0)
            townCards.DraftCards.ForEach(x =>
            {
                if (x.CardData != null) x.CardData.IdTown = townId;
                if (x.CardDetail != null) x.CardDetail.IdTown = townId;
            });
        return townCards;
        }

    public async Task<Card> GetByIdIntAsync(int id, CancellationToken cancellationToken)
        {
        //here no update only fetching of verified cards
        var card = await dbCard.AsNoTracking()
            .Include(x => x.CardData)
            .Include(x => x.CardDetail)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return card;
        }

    public async Task<IList<iCardDto>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
        //todo should have townid additional
        var query = dbCardDraft.AsNoTracking().OrderBy(p => p.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(p => p.Name.Contains(name));

        return await query
             .OrderByDescending(x => x.Id)
            .Take(ResultLimit).Select(p => mapper.Map<iCardDto>(p))
            .ToListAsync(cancellationToken);
        }

    public async Task<(bool approvedResult, int townIdRefreshRequired)> ApproveCardAsync(ApproveCardCommand request, CancellationToken cancellationToken)
        {
        //1 check for valid verified card owner or admin
        //2 better approve same card type (TIMEBEING above than it also can approve) //todo
        //for this fetch user max level of verified card type

        if (!Guid.TryParse(authenticatedUserService?.UserId, out Guid approverId))
            throw new Exception("User Details not Valid");

        var userRoles = await accountServices.GetUserRolesAsync(approverId);
        bool isAdmin = userRoles.Intersect(CONSTANTS.ROLES.AdminWriters).Any();
        if (!userRoles.Intersect(CONSTANTS.ROLES.Approvers).Any())
            {
            Console.WriteLine("Not having Enough roles to approve so not doing any changes for approval of cards");
            return (false, 0);//as not admin
            }
        request.IdApprover = approverId;
        var card = await dbCard.FindAsync(request.IdCard, cancellationToken);

        if (card != null)
            {
            bool? modifiedResult;
            int totalApprovedPeerCount = 0;
            Card_DraftChanges card_DraftChanges = null;
            if (isAdmin)
                modifiedResult = await ApproveCardByAdmin(request, card, cancellationToken);
            else
                {
                card_DraftChanges = await dbCardDraft.FindAsync(request.IdCard, cancellationToken);
                (modifiedResult, totalApprovedPeerCount) = await ApproveCardByPeer(request, card_DraftChanges, userRoles, cancellationToken);
                }

            if (modifiedResult == true && (isAdmin ||
                (request.ApproveStatus == true && totalApprovedPeerCount >= 3) ||
                (request.ApproveStatus == false && totalApprovedPeerCount >= 6)))
                {
                bool changetownRefreshRequired = false;
                if (card.IsVerified != request.ApproveStatus)
                    {
                    var overWrittedResult = await OverWriteImages(request.IdCard, card, cancellationToken, card_DraftChanges);
                    await dbCard.Where(x => x.Id == request.IdCard)
                      .ExecuteUpdateAsync(y => y
                       .SetProperty(z => z.IsVerified, request.ApproveStatus)
                       .SetProperty(z => z.IsAdminVerified, isAdmin)
                       .SetProperty(z => z.LastModifiedBy, request.IdApprover)
                       .SetProperty(z => z.LastModified, DateTimeExtension.CurrentTime),
                       cancellationToken);
                    changetownRefreshRequired = true;
                    }

                var modifiedCount = await dbContext.SaveChangesAsync(cancellationToken);
                return (true, townIdRefreshRequired: changetownRefreshRequired ? request.IdTown : 0);
                }
            }
        return (false, 0);
        }

    private async Task<(bool? modifiedResult, int totalApprovedCount)> ApproveCardByPeer(ApproveCardCommand request, Card_DraftChanges cardDraft, IList<string> userRoles, CancellationToken cancellationToken)
        {//savechanges pending after this method
        //1 check for valid verified card owner or admin
        //2 better approve same card type (TIMEBEING above than it also can approve) //todo
        //for this fetch user max level of verified card type
        //update approval table entry
        //update card table entry , also delete draftchanges table entry as unnecessary

        if (cardDraft != null)
            {
            int townId = cardDraft.IdTown;
            //check if he has allowed to do or not
            if (userRoles.Intersect(CONSTANTS.ROLES.Approvers).Any())
                {
                //var maxTypeId = await dbCardVerified.Where(x => x.OwnerId == approverId).MaxAsync(x => x.TypeId);
                //later we can use above one
                var userMatchingVerifierCards = await dbCard.Where(x => x.IdOwner == request.IdApprover
                        && x.IdCardType == cardDraft.IdCardType).ToListAsync(cancellationToken);

                if (userMatchingVerifierCards != null && userMatchingVerifierCards.Any(c => c.Id > 0))
                    {
                    var existingApprovalAll = await dbContext.CardApprovals.Where(c => c.IdCard == request.IdCard).ToListAsync(cancellationToken);

                    var currentUserApproval = existingApprovalAll.Where(x => x.UserId == request.IdApprover ||
                    x.IdCardOfApprover > 0 && userMatchingVerifierCards.Select(x => x.Id).ToList().Contains(x.IdCardOfApprover ?? 0)).ToList();
                    //existingApprovals.OrderBy(c => c.IsVerifiedEntryExists);//todo had to confirm whether tru first or last
                    if (currentUserApproval.Count > 1)
                        {
                        //remove more than 1 entry,hope this never comes
                        foreach (var item in currentUserApproval)
                            {
                            if (item.IdCard == currentUserApproval.First().IdCard) continue;
                            dbContext.CardApprovals.Remove(item);
                            }
                        }
                    CardApproval newCardApproval = null;
                    if (currentUserApproval.Count > 0)
                        {
                        if (currentUserApproval.All(x => x.IsVerified == request.ApproveStatus))
                            return (null, 0);//no changes
                        //then no change required as already same just return as it is
                        else
                            foreach (var item in currentUserApproval)
                                {
                                item.IsVerified = request.ApproveStatus;
                                item.LastModifiedBy = request.IdApprover;
                                item.UserId = request.IdApprover ?? Guid.Empty;
                                }
                        }
                    else
                        {
                        newCardApproval = new CleanArchitecture.Domain.CardApproval()
                            {
                            IdCard = request.IdCard,
                            UserId = request.IdApprover ?? Guid.Empty,
                            IdCardOfApprover = request.IdApproverCard,
                            IdTown = request.IdTown,
                            Message = request.Message,
                            IsVerified = request.ApproveStatus
                            };
                        }
                    //await dbContext.SaveChangesAsync(cancellationToken);//dotn call here
                    return (true, totalApprovedCount: existingApprovalAll.Count
                        (a => a.IsVerified == true) + 1);//+1 is of now
                                                         //if now rejected then it gets decreased that logic not yet done
                    }
                else
                    {
                    throw new Exception("Authenticated user is not Owner of Specific card type,so not allowed to Approve");
                    }
                }
            }
        return (null, 0);//no changes pending,so no approval required
        }

    public async Task<bool?> ApproveCardByAdmin(ApproveCardCommand request, Card card, CancellationToken cancellationToken)//IList<string> adminRoles,
        {//savechanges pending
        if (card != null)
            {
            //since this is by admin so check for admin id only
            request.IdTown = card.IdTown;
            request.IdApproverCard = 0;

            var existingApproval = await dbContext.CardApprovals.FirstOrDefaultAsync(x =>
            x.IdCard == request.IdCard && x.IdTown == request.IdTown, cancellationToken);
            //request.IdApproverCard > 0 ? x.IdCardOfApprover == request.IdApproverCard : true &&
            //request.IdTown > 0 ? x.IdTown == request.IdTown : true,

            CardApproval newCardApproval = null;
            if (existingApproval != null)
                {
                if (existingApproval.IsVerified == request.ApproveStatus) return null;//no changes

                existingApproval.UserId = request.IdApprover;
                existingApproval.IsVerified = request.ApproveStatus;
                existingApproval.Message = request.Message;
                dbContext.CardApprovals.Update(existingApproval);

                //Ideally update only if isverified or message is different but to make it sync updating all as above
                /*  bool changed = false;
                  if (existingApproval.IsVerified != true)
                  {
                      existingApproval.IsVerified = true;
                      changed = true;
                  }

                  if (!string.IsNullOrEmpty(request.Message) && !StringExtensions.Equals(existingApproval.Message, request.Message))
                  {
                      existingApproval.Message = request.Message;
                      changed = true;
                  }
                  if (changed) dbContext.CardApprovals.Update(existingApproval);
                  */
                }
            else
                {
                newCardApproval = new CleanArchitecture.Domain.CardApproval()
                    {
                    IdCard = request.IdCard,
                    IdTown = request.IdTown,
                    IsVerified = request.ApproveStatus,
                    IdCardOfApprover = request.IdApproverCard,
                    UserId = request.IdApprover ?? Guid.Empty,
                    Message = request.Message
                    };
                //Town2VerifiedCards cant add here,as Card_VerifiedEntries still not added
                await dbContext.CardApprovals.AddAsync(newCardApproval);
                }
            return true;
            }
        else
            {
            // Handle the case where the Card_VerifiedEntries was not found
            throw new Exception("VerifiedCard not found");
            }
        }

    private async Task<bool> OverWriteImages(int cardId, Card existingCard, CancellationToken cancellationToken, Card_DraftChanges cardDraft = null)
        {
        try
            {
            if (existingCard == null || cardDraft == null ||
                (string.IsNullOrEmpty(existingCard.Image1) && string.IsNullOrEmpty(existingCard.Image2)))
                return false;

            List<string> ForDeletionImages = new List<string>();

            dbContext.Entry(existingCard).State = EntityState.Detached;

            //first non mapping,copy manually then do mapping
            //when moving from draft to approved,images had to be checked
            if (!StringExtensions.Equals(existingCard.Image1, cardDraft.Image1))
                ForDeletionImages.Add(existingCard.Image1);
            if (!StringExtensions.Equals(existingCard.Image2, cardDraft.Image2))
                ForDeletionImages.Add(existingCard.Image2);

            existingCard = mapper.Map<Card>(cardDraft);

            //here we can do compare property so,it ignores if no change
            //dbContext.Card_VerifiedEntries.Attach(existingVerifiedCard);
            //dbContext.Entry(existingVerifiedCard).State = EntityState.Modified;
            //if issue in verified cards saving then nullify object.CardDataEntries & object.CardDetails
            dbContext.Cards.Update(existingCard);

            if (ListExtensions.HasData(ForDeletionImages))
                {
                await azImageStorage.DeleteFilesOfTownId(ForDeletionImages, cancellationToken);
                // Start the task without waiting for completion
                //_ = Task.Run(() => azImageStorage.DeleteFilesOfCardId(existingVerifiedCard.IdCARD, ForDeletionImages, cancellationToken));
                }

            return true;
            }
        catch (Exception e)
            {
            Console.WriteLine(e.ToString());
            throw e;
            }
        }
    }
