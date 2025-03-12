using AutoMapper;
using CleanArchitecture.Application.Interfaces.UserInterfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
    {
    public class CleanUpRepository(DbContextProvider dbContextProvider, IMapper mapper, ILogger<CleanUpRepository> logger,
        IAccountServices accountServices,
        // IIdentityRepository accountServices,
        IBackgroundJobsRepository backgroundJobsRepository) : ICleanUpRepository
        {
        private ApplicationDbContext dbContext = dbContextProvider.DbContext;

        public async Task<CardDto> GetCardFromTrash(int cardId, CancellationToken cancellationToken)
            {
            string message = null;
            if (cardId > 0)
                {
                var trash = await dbContext.CardTrashes.FirstOrDefaultAsync(x => x.Id == cardId, cancellationToken);
                if (trash != null && !string.IsNullOrEmpty(trash.CardDataAsJsonString))
                    {
                    if (JsonExtensions.TryDeserialize<Card>(trash.CardDataAsJsonString, out Card card))
                        {
                        CardDto result = mapper.Map<CardDto>(card);
                        return result;
                        }
                    else
                        message = $"Trash Data Unable To Extract for id:{cardId}";
                    }
                message = $"Trash Data not found for id:{cardId}";
                }
            message = $"Invalid iCardId Passed({cardId})";
            Console.WriteLine(message);
            logger.LogError(message);
            return null;
            }

        public async Task<(List<int> cardsToDelete, int cardDeletionCount, IdentityResult userDeletionResult)> DeleteUserAndAllUserCardsAndDataCompletely(Guid targetUserId, Guid operatorUserId, CancellationToken cancellationToken, bool isAdmin = false)
            {
            //todo
            //comments,likes id referenced should be removed
            //cards details had to be moved to trash place as backup
            //az image deletion pending,which addressed in handler
            var cardsToDelete = await dbContext.Cards.Where(x => x.IdOwner == targetUserId).Select(x => x.Id).ToListAsync(cancellationToken);
            int cardDeletionChangesCount = 0;
            if (ListExtensions.HasData(cardsToDelete))
                {
                // cardsToDelete.ForEach(async x =>//dont use this, it wont wait for first to complete,instead runs in parallel.so transaction makes issue
                foreach (var card in cardsToDelete)
                    cardDeletionChangesCount += await DeleteCardAndDependentDataCompletely(card, operatorUserId, cancellationToken, isAdmin);
                }

            IdentityResult userDeletionResult = await accountServices.DeleteUserCompletely(targetUserId);//, operatorUserId, cancellationToken);

            return (cardsToDelete: cardsToDelete, cardDeletionCount: cardDeletionChangesCount, userDeletionResult);
            }

        public async Task<int> DeleteCardAndDependentDataCompletely(int cardId, Guid userId, CancellationToken cancellationToken, bool isAdmin = false)
            {//az image deletion pending,which addressed in handler
            logger.LogWarning($"{nameof(DeleteCardAndDependentDataCompletely)} For id:{cardId} by UserId:{userId}");
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
                {
                var card = await dbContext.Cards
                    .Include(x => x.CardData)
                    .Include(x => x.CardDetail)
                    .Include(x => x.DraftChanges)
                    .FirstOrDefaultAsync(x => x.Id == cardId);
                if (card != null)
                    {
                    var cardTrash = card.CloneBySerializing();
                    if (isAdmin || card.IdOwner == userId ||
                        (card.IdOwner == Guid.Empty && (card.CreatedBy == userId || card.LastModifiedBy == userId)))
                        {
                        //if (card.IdCardType == ConstantsTown.TownTypeId && !isAdmin)
                        //    {
                        //    string message = $"{nameof(UnauthorizedAccessException)} as TownCard for {nameof(DeleteCardAndDependentDataCompletely)} iCard-id {cardId}";
                        //    Console.WriteLine(message);
                        //    throw new UnauthorizedAccessException(message);
                        //    }
                        if (card.DraftChanges != null)
                            dbContext.Card_DraftChanges.Remove(card.DraftChanges);
                        //deleting impacts approvedcard table entry where this particular owner approved,so replace with townadmin or some other mapping required

                        if (ListExtensions.HasData(card.VerifiedCardDisplayDates))
                            dbContext.CardDisplayDates.RemoveRange(card.VerifiedCardDisplayDates);
                        if (ListExtensions.HasData(card.AdditionalTownsOfVerifiedCard))
                            dbContext.Card_AdditionalTowns.RemoveRange(card.AdditionalTownsOfVerifiedCard);
                        if (ListExtensions.HasData(card.CardRatings))
                            dbContext.CardRatings.RemoveRange(card.CardRatings);
                        dbContext.Remove(card);

                        cardTrash = card.CloneBySerializing();

                        if (card.CardDetail != null)
                            dbContext.CardDetails.Remove(card.CardDetail);
                        if (card.CardData != null)
                            dbContext.CardDataEntries.Remove(card.CardData);
                        dbContext.Remove(card);

                        var resultCount = await dbContext.SaveChangesAsync();
                        logger.LogWarning($"Success:{nameof(DeleteCardAndDependentDataCompletely)} For id:{cardId} by UserId:{userId}");
                        if (resultCount > 0)
                            {
                            await dbContext.CardTrashes.AddAsync(new CardTrash() { Id = cardId, CardDataAsJsonString = cardTrash.Serialize() }, cancellationToken);
                            await dbContext.SaveChangesAsync();
                            // Start the task without waiting for completion
                            //_ = Task.Run(() => backgroundJobsRepository.MarkTownAsCardsUpdated(cardTrash.IdTown, cancellationToken));
                            //above making problem of 2nd operation started kind of failures,so avoiding
                            await backgroundJobsRepository.MarkTownAsCardsUpdated(cardTrash.IdTown, cancellationToken);
                            }
                        transaction.Commit();
                        Console.WriteLine($"{nameof(DeleteCardAndDependentDataCompletely)} Total Deletion Count:{resultCount}");
                        return resultCount;
                        }
                    else
                        {
                        string message = $"{nameof(UnauthorizedAccessException)} for {nameof(DeleteCardAndDependentDataCompletely)} iCard-id {cardId}";
                        Console.WriteLine(message);
                        throw new UnauthorizedAccessException(message);
                        //return -1;//unreachable
                        }
                    }
                else
                    {
                    string message = $"{nameof(DeleteCardAndDependentDataCompletely)} iCard-id {cardId} Details Not found";
                    Console.WriteLine(message);
                    throw new Exception(message);
                    }
                }
            catch (Exception ex)
                {
                transaction.Rollback();
                Console.WriteLine(ex.ToString());
                throw;
                }
            }
        }
    }
