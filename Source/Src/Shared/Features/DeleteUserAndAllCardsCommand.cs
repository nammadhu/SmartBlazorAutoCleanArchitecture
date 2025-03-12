namespace SHARED.Features;

public class DeleteUserAndAllCardsCommand : IRequest<BaseResult<bool>>
    {
    public Guid TargetUserId { get; set; }
    public Guid OperatorId { get; set; }
    public bool IsAdmin { get; set; }
    }
