using BASE;

namespace CleanArchitecture.Application.Features.Cards.Commands;

public class DeleteCardCommandHandler(ICleanUpRepository cleanUpRepository, IAzImageStorage azImageStorage, ITranslator translator, ILogger<DeleteCardCommandHandler> logger) : IRequestHandler<DeleteCardCommand, BaseResult<bool>>
    {
    public async Task<BaseResult<bool>> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
        try
            {
            int result = await cleanUpRepository.DeleteCardAndDependentDataCompletely(request.IdCard, request.OperatorId, cancellationToken, request.IsAdmin);
            if (result > 0)
                {
                var resultImages = await azImageStorage.DeleteWholeCardImagesFolder(request.IdCard, cancellationToken);
                if (ListExtensions.HasData(resultImages))
                    {
                    resultImages.ForEach(r => Console.WriteLine($"a1 {r.filename}-{r.result}"));
                    }
                return BaseResult<bool>.OkNoClientCaching(true);
                }
            else
                return new Error(ErrorCode.NotHaveAnyChangeInData, "Delete iCard Failed");
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard ApprovalSetRequest"));
            }
        }
    }
