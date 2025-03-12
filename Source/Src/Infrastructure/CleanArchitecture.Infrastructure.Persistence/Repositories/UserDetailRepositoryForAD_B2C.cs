using AutoMapper;
using BASE.Common;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

//Mostly used in case of AD_B2C using as db doesnt contain more info of user roles ,cards entry and all.
//Whichever user makes operations only those will exist here
//Still on aspnet core also we can attach or extend.but this should be key entry for any users changes checks
public class UserDetailRepositoryForAD_B2C(DbContextProvider dbContextProvider,  IMapper mapper,IIdentityRepositoryAdB2C identityRepository) //: GenericRepository<UserDetail>(dbContextProvider), IUserDetailRepositoryAdB2c
//IIdentityRepository identityRepository,
//, IGenericRepository<UserInfo>
//, ITownRepository ,
    {
    private readonly DbSet<UserDetail> db = dbContextProvider.DbContext.Set<UserDetail>();

    public async Task<UserDetailDto> GetByIdIncludeCardsAsync(Guid userId, CancellationToken cancellationToken)
        {
        //this should not be executed for admin,as they will be having lot of cards makes large set

        UserDetail userDetail = await db.Where(u => u.Id == userId)
            .Include(x => x.iCards).ThenInclude(x => x.CardData)//mostly by default included
            .Include(x => x.iCards).ThenInclude(x => x.CardDetail)//mostly by default included
            .Include(x => x.iCards).ThenInclude(x => x.VerifiedCardDisplayDates)//mostly by default included
            .Include(x => x.iCards).ThenInclude(x => x.CardRatings)//mostly by default included
            .Include(x => x.iCards).ThenInclude(x => x.AdditionalTownsOfVerifiedCard)//mostly by default included
            .Include(x => x.iCards).ThenInclude(x => x.DraftChanges)
            .FirstOrDefaultAsync(cancellationToken);

        var result = userDetail == null || userDetail == default ? null : mapper.Map<UserDetailDto>(userDetail);

        return result;
        }

    public async Task<IdentityResult> AddUserRoles(Guid userId, List<string> roles, Guid operatorId,
        UserDetailBase existingUserInGraph, CancellationToken cancellationToken)
        {
        if (roles?.Count > 0)
            {
            var existingUserInDatabase = await db.FindAsync(userId, cancellationToken);
            if (existingUserInDatabase == null)
                {
                //insert now by fetching detail from graphdb for verification
                existingUserInGraph ??= await identityRepository.GetUserAsync(userId.ToString(), cancellationToken);
                existingUserInGraph.Roles ??= [];
                existingUserInGraph.Roles.AddRange(roles);
                existingUserInGraph.Roles = existingUserInGraph.Roles.Distinct().ToList();

                var userDatabaseEntity = mapper.Map<UserDetail>(existingUserInGraph);
                userDatabaseEntity.CreatedBy = operatorId;
                await db.AddAsync(userDatabaseEntity);
                int resultCount = await dbContextProvider.DbContext.SaveChangesAsync(cancellationToken);
                if (resultCount > 0)
                    return IdentityResult.Success;
                }
            else
                {
                var existingRoles = existingUserInDatabase.Roles?.ToList();
                if (existingRoles == roles) return IdentityResult.Success;
                //already same ,so no changes requried

                //update entity with new roles

                existingUserInDatabase.Roles ??= [];
                existingUserInDatabase.Roles.AddRange(roles);
                existingUserInDatabase.Roles = existingUserInDatabase.Roles.Distinct().ToList();
                //sort is required
                if (existingRoles == existingUserInDatabase.Roles) return IdentityResult.Success;
                //already same ,so no changes requried

                int resultCount = await db.Where(x => x.Id == userId)
                .ExecuteUpdateAsync(u =>
                u.SetProperty(p => p.Roles, existingUserInDatabase.Roles)
                .SetProperty(p => p.LastModifiedBy, operatorId)
            );
                if (resultCount > 0)
                    return IdentityResult.Success;
                }
            }
        return IdentityResult.Failed();
        }
    }

/*
  public async Task<UserDetailDto> GetUserCardsMoreDetails(GetCardsOfUserQuery query, CancellationToken cancellationToken)
  {
      db.Where(x => x.Id == query.UserId)
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
      return CardSegregation(new UserDetailDto() { Id = query.UserId, CardsVerified = verifiedItems, CardsDraft = draftCards, CardApprovals = cardApprovals });
  }
  */
