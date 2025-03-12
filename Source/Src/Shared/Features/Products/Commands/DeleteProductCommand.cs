namespace SHARED.Features.Products.Commands;

public class DeleteProductCommand : IRequest<BaseResult>
    {
    public long Id { get; set; }
    }
