using BASE;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class CardApprovalRepository
: GenericRepository<CleanArchitecture.Domain.CardApproval>, ICardApprovalRepository
    {
    public CardApprovalRepository(DbContextProvider dbContextProvider) : base(dbContextProvider: dbContextProvider)
        {
        this.dbContextProvider = dbContextProvider;
        //dbTown2VerifiedCard = dbContextProvider.DbContext.Set<Town2VerifiedCard>();
        dbCardApproval = dbContextProvider.DbContext.Set<CleanArchitecture.Domain.CardApproval>();
        }

    private readonly DbContextProvider dbContextProvider;

    //private readonly DbSet<Town2VerifiedCard> dbTown2VerifiedCard;
    private readonly DbSet<CleanArchitecture.Domain.CardApproval> dbCardApproval;

    public async Task<bool> SetApproverCardOfDraftCard(ApprovalCardSetRequestCommand request, CancellationToken cancellationToken)
        {
        //await dbContext.CardApprovals.AddOrUpdateAsync(new CardApprovals() { IdCard = cardId, IdCardOfApprover = approverCardId, CreatedBy = requestorId }, p => p.IdCard, cancellationToken);
        //await dbContext.SaveChangesAsync();

        //await dbContext.AddOrUpdateAndSaveChangesAsync(new CardApprovals() { IdCard = cardId, IdCardOfApprover = approverCardId, CreatedBy = requestorId }, p => p.IdCard, cancellationToken);
        if (request.IdApproverCard == request.IdCard) return false;

        if (!(await dbCardApproval.AnyAsync(x => x.IdCard == request.IdCard && x.IdCardOfApprover == request.IdApproverCard
        && x.IdTown == request.IdTown, cancellationToken)))
            {
            //not exists ,so add now
            var created = await dbCardApproval.AddAsync(new CleanArchitecture.Domain.CardApproval() { IdCard = request.IdCard, IdCardOfApprover = request.IdApproverCard, IdTown = request.IdTown, CreatedBy = request.IdRequestor }, cancellationToken);
            return await dbContextProvider.DbContext.SaveChangesAsync(cancellationToken) > 0;
            }
        else
            {
            //think of more if required
            return true;
            }
        }

    /// <summary>
    /// user verifiedCardIdsForApproval will be sent,if those are marked as reviewer then only fetched
    /// isVerified:null fetches pending,true fetches approved,false fetches rejected
    /// </summary>
    public async Task<List<CleanArchitecture.Domain.CardApproval>> GetCardApprovalsOfApprover(List<int> verifiedCardIdsForApproval, CancellationToken cancellationToken, bool? isVerified = null)
        {
        if (ListExtensions.HasData(verifiedCardIdsForApproval))
            {
            //lets fetch all
            return await dbCardApproval.Include(x => x.iCard)
                .Where(x => x.IdCardOfApprover != null && verifiedCardIdsForApproval.Contains(x.IdCardOfApprover ?? 0)
            //&& x.IsVerifiedEntryExists == isVerified//fetches only null by default
            )
                .ToListAsync(cancellationToken);
            }
        return default;
        }

    public async Task<List<CleanArchitecture.Domain.CardApproval>> GetCardApprovals(int cardId, CancellationToken cancellationToken, bool? isVerified = null)
        {
        return await dbCardApproval.Where(x => x.IdCard == cardId).ToListAsync(cancellationToken);//fetches only null
        }

    //public async Task<int> DeleteCardApprovals(int cardId, List<int> approverIds, CancellationToken cancellationToken)
    ////instead of deleting,lets mark as False,so will be traceble & update by user itself or approver also
    //    {
    //    return await dbCardApproval.Where(x => x.IdCard == cardId && x.IdCardOfApprover!=null
    //    && approverIds.Contains(x.IdCardOfApprover??0))
    //        .ExecuteUpdateAsync(ss => ss.SetProperty(a => a.IsVerifiedEntryExists, false), cancellationToken);//fetches only null
    //    }
    public async Task<int> DeleteCardApprovals(int cardId, List<CardApproval> approvers, CancellationToken cancellationToken)
    //instead of deleting,lets mark as False,so will be traceble & update by user itself or approver also
        {
        if (approvers == null || !approvers.Any()) return 0;

        var query = dbCardApproval.Where(x => x.IdCard == cardId);

        foreach (var approver in approvers)
            {
            query = query.Where(x =>
                (x.IdCardOfApprover == approver.IdCardOfApprover || !approver.IdCardOfApprover.HasValue) &&
                (x.IdTown == approver.IdTown || !approver.IdTown.HasValue));
            }

        return await query.ExecuteUpdateAsync(ss => ss.SetProperty(a => a.IsVerified, false), cancellationToken);
        //below way of exists or any makes translation failure ,so use query building

        //return approvers == null ? 0 :
        //    await dbCardApproval.Where(x => x.IdCard == cardId && x.IdCardOfApprover != null
        //&& approvers.Exists(a =>  a.IdCardOfApprover == x.IdCardOfApprover && a.IdTown == x.IdTown))
        //approvers.Any(ap => ap.IdCardOfApprover == x.IdCardOfApprover && ap.IdTown == x.IdTown))
        //    .ExecuteUpdateAsync(ss => ss.SetProperty(a => a.IsVerifiedEntryExists, false), cancellationToken);//fetches only null
        }
    }
