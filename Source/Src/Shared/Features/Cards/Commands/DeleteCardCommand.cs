namespace SHARED.Features.Cards.Commands;

public class DeleteCardCommand : IRequest<BaseResult<bool>>
    {
    public int IdCard { get; set; }
    public Guid OperatorId { get; set; }
    public bool IsAdmin { get; set; }
    }
