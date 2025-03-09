using CleanArchitecture.Application.Features.Products.Queries;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain;
using Moq;
using Shared;
using Shared.Features.Products.Queries;
using Shared.Wrappers;
using Shouldly;

namespace CleanArchitecture.UnitTests.ApplicationTests.Features.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ProductExists_ReturnsSuccessResultWithProductDto()
    {
        // Arrange
        CancellationTokenSource cancellationTokenSource = new();
        var productId = 1;
        var productName = "Test Product";
        var productPrice = 1000;
        var productBarCode = "123456789";

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<long>(), cancellationTokenSource.Token))
                             .ReturnsAsync(new Product(productName, productPrice, productBarCode) { Id = productId });

        var translatorMock = new Mock<ITranslator>();

        var handler = new GetProductByIdQueryHandler(productRepositoryMock.Object, translatorMock.Object);

        var query = new GetProductByIdQuery { Id = productId };

        // Act
        var result = await handler.Handle(query, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(productId);
        result.Data.Name.ShouldBe(productName);
        result.Data.Price.ShouldBe(productPrice);
        result.Data.BarCode.ShouldBe(productBarCode);
    }

    [Fact]
    public async Task Handle_ProductNotExists_ReturnsNotFoundResult()
    {
        // Arrange
        var productId = 1;
        CancellationTokenSource cancellationTokenSource = new();
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId,cancellationTokenSource.Token));

        var translatorMock = new Mock<ITranslator>();
        translatorMock.Setup(translator => translator.GetString(It.IsAny<string>()))
                      .Returns("Product not found");

        var handler = new GetProductByIdQueryHandler(productRepositoryMock.Object, translatorMock.Object);

        var query = new GetProductByIdQuery { Id = productId };

        // Act
        var result = await handler.Handle(query, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Errors.ShouldContain(err => err.ErrorCode == ErrorCode.NotFound);
        result.Data.ShouldBeNull();
    }
}
