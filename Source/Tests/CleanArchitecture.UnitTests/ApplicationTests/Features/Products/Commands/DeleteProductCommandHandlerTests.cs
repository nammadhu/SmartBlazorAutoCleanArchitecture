using CleanArchitecture.Application.Features.Products.Commands;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain;
using Moq;
using Shared;
using Shared.Features.Products.Commands;
using Shared.Wrappers;
using Shouldly;

namespace CleanArchitecture.UnitTests.ApplicationTests.Features.Products.Commands;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProductExists_ReturnsSuccessResult()
    {
        CancellationTokenSource cancellationTokenSource = new();
        // Arrange
        var productId = 1;
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<long>(), cancellationTokenSource.Token))
                             .ReturnsAsync(new Product("", 100, "") { Id = productId });

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var translatorMock = new Mock<ITranslator>();

        var handler = new DeleteProductCommandHandler(productRepositoryMock.Object, unitOfWorkMock.Object, translatorMock.Object);

        var command = new DeleteProductCommand { Id = productId };

        // Act
        var result = await handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        productRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(cancellationTokenSource.Token), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotExists_ReturnsNotFoundResult()
    {
        // Arrange
        CancellationTokenSource cancellationTokenSource = new();
        var productId = 1;
        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, cancellationTokenSource.Token));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var translatorMock = new Mock<ITranslator>();
        translatorMock.Setup(translator => translator.GetString(It.IsAny<string>()))
                      .Returns("Product not found");

        var handler = new DeleteProductCommandHandler(productRepositoryMock.Object, unitOfWorkMock.Object, translatorMock.Object);

        var command = new DeleteProductCommand { Id = productId };

        // Act
        var result = await handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Errors.ShouldContain(err => err.ErrorCode == ErrorCode.NotFound);

        productRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Never);
        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(cancellationTokenSource.Token), Times.Never);
    }
}
