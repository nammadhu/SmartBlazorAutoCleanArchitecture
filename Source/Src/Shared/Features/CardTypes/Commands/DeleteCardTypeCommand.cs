namespace SHARED.Features.CardTypes.Commands;

public class DeleteCardTypeCommand : IRequest<BaseResult>
    {
    public int IdCardType { get; set; }
    }
