using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductCreateTests
{
    [Fact]
    public void Create_Should_Succeed_When_Context_Is_Valid()
    {
        // Arrange
        var ctx = ProductContextsTestFactory.ValidCreateContext();
        var data = ProductDataTestFactory.CreateData();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        // Act
        var result = Models.Product.Create(
            ctx, 
            data, 
            vendorId,
            categoryId, 
            brandId,
            now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var product = result.Value;
        product.Name.Should().Be(data.Name);
        product.Slug.Should().Be(data.Slug);
        product.Description.Should().Be(data.Description);
        product.ShortDescription.Should().Be(data.ShortDescription);
        product.SeoMetadata.Should().Be(data.SeoMetadata);
        product.CategoryId.Should().Be(categoryId);
        product.VendorId.Should().Be(vendorId);
        product.BrandId.Should().Be(brandId);
        product.CreatedAt.Should().Be(now);
        product.ProductStatus.Should().Be(ProductStatus.Draft);
        product.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Should_Raise_Single_ProductCreatedDomainEvent_With_Matching_Data()
    {
        // Arrange
        var ctx = ProductContextsTestFactory.ValidCreateContext();
        var data = ProductDataTestFactory.CreateData();

        // Act
        var result = Models.Product.Create(
            ctx,
            data, 
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        // Assert
        var product = result.Value;
        product.DomainEvents.Should().ContainSingle();

        var domainEvent = product.DomainEvents.Single().Should()
            .BeOfType<ProductCreatedDomainEvent>().Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.CategoryId.Should().Be(product.CategoryId);
        domainEvent.VendorId.Should().Be(product.VendorId);
        domainEvent.BrandId.Should().Be(product.BrandId);
        domainEvent.Name.Should().Be(product.Name);
        domainEvent.Slug.Should().Be(product.Slug);
        domainEvent.ProductStatus.Should().Be(product.ProductStatus);
        domainEvent.CanBeModified.Should().Be(product.CanBeModified);
        domainEvent.IsVisiblePublicly.Should().Be(product.IsVisiblePublicly);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void Create_Should_Fail_When_Specification_Is_Not_Satisfied(
        bool vendorIsActive, bool categoryIsActive, bool brandIsAssignable)
    {
        // Arrange
        var ctx = new ProductCreationContext(vendorIsActive, categoryIsActive, brandIsAssignable);
        var data = ProductDataTestFactory.CreateData();

        // Act
        var result = Models.Product.Create(
            ctx, 
            data,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}