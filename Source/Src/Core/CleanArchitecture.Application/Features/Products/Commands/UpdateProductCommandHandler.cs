using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using MediatR;
using SharedResponse;
using Shared.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Features.Products.Commands;

namespace CleanArchitecture.Application.Features.Products.Commands;

public class UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateProductCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
        }

        product.Update(request.Name, request.Price, request.BarCode);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Ok();
    }
}
