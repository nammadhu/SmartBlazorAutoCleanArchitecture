namespace CleanArchitecture.Application.Features.Cards.Commands;

public class ApprovalCardSetRequestCommandHandler(ICardApprovalRepository townCardRepo, ITranslator translator, ILogger<ApprovalCardSetRequestCommandHandler> logger)
    : IRequestHandler<ApprovalCardSetRequestCommand, BaseResult<bool>>
    {
    public async Task<BaseResult<bool>> Handle(ApprovalCardSetRequestCommand request, CancellationToken cancellationToken)
        {
        try
            {
            var result = await townCardRepo.SetApproverCardOfDraftCard(request, cancellationToken);
            return BaseResult<bool>.OkNoClientCaching(result);
            //here only setting as approver
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCard ApprovalSetRequest"));
            }
        }
    }
