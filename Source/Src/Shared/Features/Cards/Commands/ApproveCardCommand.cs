namespace SHARED.Features.Cards.Commands;

public class ApproveCardCommand : IRequest<BaseResult<bool>>
    {
    public int IdTown { get; set; }
    public int IdCard { get; set; }
    public bool? ApproveStatus { get; set; }
    public string? Message { get; set; }
    public Guid? IdApprover { get; set; }
    public int IdApproverCard { get; set; }
    //id of approver card or for admin case townid itself
    }
