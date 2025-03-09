namespace Shared.Features.CardTypes.Commands;

public class DeleteCardTypeCommand : IRequest<BaseResult>
    {
    public int IdCardType { get; set; }
    }
