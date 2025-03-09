namespace CleanArchitecture.Application.Features
    {
    public class DeleteUserAndAllCardsCommandHandler(ICleanUpRepository cleanUpRepository, IAzImageStorage azImageStorage, ILogger<DeleteUserAndAllCardsCommandHandler> logger) : IRequestHandler<DeleteUserAndAllCardsCommand, BaseResult<bool>>
        {
        public async Task<BaseResult<bool>> Handle(DeleteUserAndAllCardsCommand request, CancellationToken cancellationToken)
            {
            logger.LogWarning($"{nameof(DeleteUserAndAllCardsCommandHandler)} for id:{request.TargetUserId} by {(request.TargetUserId == request.OperatorId ? "Self" : request.OperatorId.ToString())} Started");
            (List<int> cardsToDelete, int cardDeletionCount, IdentityResult userDeletionResult) = await cleanUpRepository.DeleteUserAndAllUserCardsAndDataCompletely(request.TargetUserId, request.OperatorId, cancellationToken, request.IsAdmin);

            if (cardDeletionCount > 0 && ListExtensions.HasData(cardsToDelete))
                {
                logger.LogWarning($"{nameof(DeleteUserAndAllCardsCommandHandler)} for id:{request.TargetUserId} by {(request.TargetUserId == request.OperatorId ? "Self" : request.OperatorId.ToString())} Account Deletion state:({userDeletionResult.Succeeded}) & Cards({cardsToDelete.Count}) Deleted with Changes ({cardDeletionCount})");

                List<(string filename, bool result)> fileResult = new();
                //cardsToDelete.ForEach(async x =>//dont use this, it wont wait for first to complete,instead runs in parallel.so transaction makes issue
                foreach (var item in cardsToDelete)
                    {
                    var result = await azImageStorage.DeleteWholeCardImagesFolder(item, cancellationToken);
                    if (ListExtensions.HasData(result))
                        {
                        fileResult.Concat(result);
                        }
                    }
                fileResult.ForEach(r => Console.WriteLine($"{r.filename}-{r.result}"));
                logger.LogWarning($"{nameof(DeleteUserAndAllCardsCommandHandler)} for id:{request.TargetUserId} by {(request.TargetUserId == request.OperatorId ? "Self" : request.OperatorId.ToString())}, {fileResult.Count} Images deleted");

                return new BaseResult<bool> { Success = true, Data = true };
                }
            logger.LogWarning($"{nameof(DeleteUserAndAllCardsCommandHandler)} for id:{request.TargetUserId} by {(request.TargetUserId == request.OperatorId ? "Self" : request.OperatorId.ToString())}, Failed");
            return new BaseResult<bool> { Success = false };
            }
        }
    }
