using AutoMapper;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class TownRepository(DbContextProvider dbContextProvider, IMapper mapper) : GenericRepository<Town>(dbContextProvider), ITownRepository
    {
    private const int MaxCardsOnSelectedPage = 100;
    private const int ResultLimit = 20;
    private readonly DbSet<Town> db = dbContextProvider.DbContext.Set<Town>();

    public async Task<IList<TownDto>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
        var query = db.OrderBy(p => p.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(p => p.Name.Contains(name));

        return await query
             .OrderByDescending(x => x.Id)
            .Take(ResultLimit).Select(p => mapper.Map<TownDto>(p)) //dont use  p.To<Towns, TownDto>())
            .ToListAsync(cancellationToken);
        }

    public async Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken)
        {
        return await db.AnyAsync(x => x.Name == name, cancellationToken);
        }
    }

/*
  /// <summary>
  /// returns Towns,its towncard, Card_VerifiedEntries (of today)
  /// </summary>
  /// <param name="id"></param>
  /// <param name="idCard"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<TownDto> GetTownAndVerifiedCardsOfToday(int id, int? idCard, CancellationToken cancellationToken)
  //public async Task<Towns> GetTownByIdAsync(int id, Guid? userId)
  {
      //var today = DateOnly.FromDateTime(DateTime.Today);
      //todo must enable for production
      //timebeing not using date for listing
      //var town = await db.FindAsync(id);
      var town = await db
          .Where(x => (x.Id == id) || (idCard > 0 && x.Card_VerifiedEntries.Any(c => c.IdCARD == idCard)))
       .Include(t => t.Card_VerifiedEntries).ThenInclude(d => d.Card_VerifiedEntries.CardDataEntries)
       .Include(t => t.Card_VerifiedEntries).ThenInclude(d => d.Card_VerifiedEntries.CardDetails)
       .Include(t => t.Card_VerifiedEntries).ThenInclude(d => d.Card_VerifiedEntries.CardDisplayDate)
       .FirstOrDefaultAsync(cancellationToken);

      var townDto = mapper.Map<TownDto>(town);
      //for admin this check has to be bypassed
      if (town.Card_VerifiedEntries?.Count < MaxCardsOnSelectedPage)
      { //then fetch remaining
          var remainingLimit = (MaxCardsOnSelectedPage - town.Card_VerifiedEntries?.Count) ?? 0;
          if (remainingLimit > 0)
          {
              townDto.Card_Drafts = dbContextProvider.DbContext.Set<Card_Drafts>()
                  .Where(x => x.IdTown == id && x.IsVerifiedEntryExists != true)
                  .OrderBy(x => x.Created)
                  .Take(remainingLimit).Select(x => mapper.Map<iCardDto>(x)).ToList();
          }
      }

      return townDto;
      /*
         //.Select(x => new TownDto()
          //    {
          //    IdTown = x.IdTown,
          //    Title = x.Title,
          //    SubTitle = x.SubTitle,
          //    Description = x.Description,
          //    District = x.District,
          //    State = x.State,
          //    UrlName1 = x.UrlName1,
          //    UrlName2 = x.UrlName2,
          //    TownDraftCard = mapper.Map<iCardDto>(x.TownDraftCard),
          //    TownVerifiedCard = mapper.Map<iCardDto>(x.TownVerifiedCard),
          //    TownVerifiedCards = x.Card_VerifiedEntries.Select(y => mapper.Map<iCardDto>(y.Card_VerifiedEntries)).ToList(),
          //    }).FirstOrDefaultAsync(cancellationToken);

         //if (town != null)
         //    {
         //    if (town.Card_VerifiedEntries != null && town.TownVerifiedCard != null)
         //        {
         //        var townCard = town.Card_VerifiedEntries.FirstOrDefault(x => x.IdCard == town.TownVerifiedCard.IdCard);
         //        if (townCard != null)
         //            town.Card_VerifiedEntries.Remove(townCard);
         //        }
         //    }

         //if (town != null)//below is only for fetching towncard as its not linked by FK
         //    {
         //    var townCardVerifiedMapping = town.Card_VerifiedEntries.Find(x => x.IdCARD == town.IdTownCard);

         //    town.TownVerifiedCard = townCardVerifiedMapping.Card_VerifiedEntries;
         //    //if (town.TownCard != null && town.CardVerifiedItems.Exists(c => c.IdCard == town.IdCard))
         //    town.Card_VerifiedEntries.Remove(townCardVerifiedMapping);//to avoid redundancy

         //    //later date filtration
         //    //todo
         //    //await dbContext.Entry(town)
         //    //    .Collection(t => t.CardVerifiedItems)
         //    //    .Query()
         //    //    .Include(x => x.Brand).Include(x => x.Product)
         //    //    //.Where(ac => ac.SelectedDates.Any(sd => sd.Date == today))
         //    //    .LoadAsync(cancellationToken);
         //    }
         //town.CreatedBy = Guid.Empty;
         //town.LastModifiedBy = Guid.Empty;

         //if (town?.TownCardVerified?.Card_Drafts != null)
         //    town.TownCardVerified.Card_Drafts.Card_VerifiedEntries = null;

         //if (town?.TownToCardsVerified != null)
         //    town.TownToCardsVerified.ForEach(x => x.Card_Drafts.Card_Drafts.Card_VerifiedEntries = null);

         //if (town?.TownToCardsDraft != null)
         //    town.TownToCardsDraft.ForEach(x => x.Card_Drafts.Card_VerifiedEntries.Card_Drafts = null);
         //return town;
         */
