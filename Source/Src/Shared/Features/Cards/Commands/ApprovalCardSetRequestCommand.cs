namespace SHARED.Features.Cards.Commands;

public class ApprovalCardSetRequestCommand : IRequest<BaseResult<bool>>
    {
    public int IdCard { get; set; }
    public int IdTown { get; set; }

    //public bool? ApproveStatus { get; set; }
    //public string? Message { get; set; }
    public Guid IdRequestor { get; set; }

    public int IdApproverCard { get; set; }
    }
