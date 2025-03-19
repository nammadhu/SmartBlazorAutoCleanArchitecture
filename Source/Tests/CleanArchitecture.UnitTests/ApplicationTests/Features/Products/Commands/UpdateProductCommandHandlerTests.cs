using CleanArchitecture.Application.Features.Products.Commands;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain;
using Moq;
using SHARED;
using SHARED.DTOs;
using SHARED.Features.Products.Commands;
using SHARED.Wrappers;
using Shouldly;

namespace CleanArchitecture.UnitTests.ApplicationTests.Features.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProductExists_ReturnsSuccessResult()
    {
        // Arrange
        CancellationTokenSource cancellationTokenSource = new();
        var productId = 1;
        var productName = "Updated Product";
        var productPrice = 1500;
        var productBarCode = "987654321";

        var productRepositoryMock = new Mock<IProductRepository>();
        productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<long>(), cancellationTokenSource.Token))
                             .ReturnsAsync(new Product("", 100, "") { Id = productId });

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var translatorMock = new Mock<ITranslator>();

        var handler = new UpdateProductCommandHandler(productRepositoryMock.Object, unitOfWorkMock.Object, translatorMock.Object);

        var command = new UpdateProductCommand
        {
            Id = productId,
            Name = productName,
            Price = productPrice,
            BarCode = productBarCode
        };

        // Act
        var result = await handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        productRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<long>(), cancellationTokenSource.Token), Times.Once);
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

        var handler = new UpdateProductCommandHandler(productRepositoryMock.Object, unitOfWorkMock.Object, translatorMock.Object);

        var command = new UpdateProductCommand { Id = productId };

        // Act
        var result = await handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Errors.ShouldContain(err => err.ErrorCode == ErrorCode.NotFound);

        productRepositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Never);
        unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(cancellationTokenSource.Token), Times.Never);
    }
}
