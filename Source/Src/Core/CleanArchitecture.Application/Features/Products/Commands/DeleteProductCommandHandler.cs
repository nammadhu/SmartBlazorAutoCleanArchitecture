using CleanArchitecture.Application.Helpers;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using MediatR;
using MyTown.SharedModels.Features.Products.Commands;
using SharedResponse;
using SharedResponse.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products.Commands;

public class DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteProductCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult.Ok();
    }
}
