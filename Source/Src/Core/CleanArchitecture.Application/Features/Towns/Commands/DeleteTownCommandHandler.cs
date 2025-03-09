namespace CleanArchitecture.Application.Features.Towns.Commands;

public class DeleteTownCommandHandler(ITownRepository townRepo, IUnitOfWork unitOfWork, ITranslator translator
    , ServerCachingServiceTowns _cachingServiceTown, ILogger<DeleteTownCommandHandler> logger) : IRequestHandler<DeleteTownCommand, BaseResult>
    {
    public async Task<BaseResult> Handle(DeleteTownCommand request, CancellationToken cancellationToken)
        {
        try
            {
            var data = await townRepo.GetByIdAsync(request.IdTown, cancellationToken);

            if (data is null)
                {
                return new Error(ErrorCode.NotFound, translator.GetString($"Not found with id:{request.IdTown}"), nameof(request.IdTown));
                }

            townRepo.Delete(data);
            if (await unitOfWork.SaveChangesAsync(cancellationToken))
                {
                _cachingServiceTown.RemoveTownInTowns(request.IdTown);
                return BaseResult.OkNoClientCaching();
                }

            return new Error(ErrorCode.Exception, $"Some issue {nameof(DeleteTownCommandHandler)}", nameof(request.IdTown));
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in Town Deletion"));
            }
        }
    }
