namespace SHARED.Features.Products.Commands;

public class CreateProductCommand : IRequest<BaseResult<long>>
    {
    public string Name { get; set; }
    public double Price { get; set; }
    public string BarCode { get; set; }
    }
