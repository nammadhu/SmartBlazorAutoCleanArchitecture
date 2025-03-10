using AutoMapper;
using Shared.Features.Cards.Queries;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class Card_AdditionalTownRespository(DbContextProvider dbContextProvider, IMapper mapper)
: GenericRepository<Card_AdditionalTown>(dbContextProvider: dbContextProvider), ICard_AdditionalTownRepository
    {
    private const int ResultLimit = 30;
    private readonly DbSet<Card_AdditionalTown> cardAdditionalTownsDb = dbContextProvider.DbContext.Set<Card_AdditionalTown>();

    public async Task<List<iCardDto>> SearchTownAdditionalVerifiedCards(int townId, string name,
      bool includeNonSensitiveData = false, bool includeDetails = false)
        {
        if (townId == 0) return null;
        var query = cardAdditionalTownsDb.AsNoTracking()
             .Where(p => p.IdTown == townId)
             .Include(x => x.iCard).ThenInclude(x => x.AdditionalTownsOfVerifiedCard)
             //.Include(x => x.Card_Drafts.CardDataEntries)
             //.Include(x => x.Card_Drafts.Details)         //later loading optionally
             .Where(x => string.IsNullOrEmpty(name) ? true : x.iCard.Name.Contains(name))
             //&& x.Card_VerifiedEntries.IdCardType != ConstantsTown.TownTypeId)//towncard not required here
             .Take(ResultLimit)
             .AsQueryable();

        if (includeNonSensitiveData)//this is for full page related only
            query = query.Include(x => x.iCard).ThenInclude(c => c.CardData);

        if (includeDetails)//this is for full page related only
            query = query.Include(x => x.iCard).ThenInclude(c => c.CardDetail);
        return await query.Select(x => mapper.Map<iCardDto>(x.iCard)).ToListAsync();
        }

    //showing for approval selection
    public async Task<List<iCardDto>> GetTownAdditionalVerifiedCardsOfType(GetVerifiedCardsOfTypeInTownQuery query, CancellationToken cancellationToken)
        {
        return await
            cardAdditionalTownsDb.Include(x => x.iCard)
            .Where(x => x.IdTown == query.TownId && x.iCard.IdCardType == query.TypeId)
            .Select(p => mapper.Map<iCardDto>(p.iCard)).ToListAsync(cancellationToken);
        }
    }
