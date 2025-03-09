namespace CleanArchitecture.Application.Features.CardTypes.Commands;

public class DeleteCardTypeCommandHandler(ICardTypeRepository townCardTypeRepo, IUnitOfWork unitOfWork, ITranslator translator, ServerCachingCardTypes cachingServiceTown, ILogger<DeleteCardTypeCommandHandler> logger) : IRequestHandler<DeleteCardTypeCommand, BaseResult>
    {
    public async Task<BaseResult> Handle(DeleteCardTypeCommand request, CancellationToken cancellationToken)
        {
        try
            {
            var data = await townCardTypeRepo.GetByIdAsync(request.IdCardType, cancellationToken);

            if (data is null)
                {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.IdCardType)), nameof(request.IdCardType));
                }

            townCardTypeRepo.Delete(data);
            if (await unitOfWork.SaveChangesAsync(cancellationToken))
                {
                cachingServiceTown.RemoveCardTypeInCardTypes(request.IdCardType);
                return BaseResult.OkNoClientCaching();
                }

            return new Error(ErrorCode.Exception, $"Some issue {nameof(DeleteCardTypeCommandHandler)}", nameof(request.IdCardType));
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCardType Deletion"));
            }
        }
    }
