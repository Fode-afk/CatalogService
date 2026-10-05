using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductUpdateInfoTests
{
    private static readonly DateTimeOffset UpdatedAt = new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void UpdateInfo_Should_Update_All_Fields_And_Raise_Event_When_Data_Differs_And_Context_Is_Valid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var newCategoryId = Guid.NewGuid();
        var newBrandId = Guid.NewGuid();
        var newData = ProductDataTestFactory.UpdateInfoData("_updated");

        // Act
        var result = product.UpdateInfo(
            ProductContextsTestFactory.ValidUpdateInfoContext(),
            newData, 
            newCategoryId, 
            newBrandId,
            UpdatedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Name.Should().Be(newData.Name);
        product.Slug.Should().Be(newData.Slug);
        product.Description.Should().Be(newData.Description);
        product.ShortDescription.Should().Be(newData.ShortDescription);
        product.SeoMetadata.Should().Be(newData.SeoMetadata);
        product.CategoryId.Should().Be(newCategoryId);
        product.BrandId.Should().Be(newBrandId);
        product.UpdatedAt.Should().Be(UpdatedAt);
        product.Version.Should().Be(1); 

        product.DomainEvents.Should().ContainSingle();
        var domainEvent = product.DomainEvents.Single().Should()
            .BeOfType<ProductInfoUpdatedDomainEvent>().Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.CategoryId.Should().Be(newCategoryId);
        domainEvent.BrandId.Should().Be(newBrandId);
        domainEvent.Name.Should().Be(newData.Name);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void UpdateInfo_Should_Be_NoOp_When_Data_Is_Identical_Even_If_Context_Is_Invalid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var sameData = ProductDataTestFactory.CreateData();
        var invalidCtx = new ProductUpdateInfoContext(
            VendorIsActive: false,
            CanEditContent: false,
            CategoryIsActive: false, 
            BrandIsAssignable: false);

        // Act
        var result = product.UpdateInfo(
            invalidCtx,
            new ProductUpdateInfoData(
                sameData.Name, 
                sameData.Slug, 
                sameData.Description, 
                sameData.ShortDescription,
                sameData.SeoMetadata),
            product.CategoryId,
            product.BrandId,
            UpdatedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.UpdatedAt.Should().BeNull();
        product.Version.Should().Be(0);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdateInfo_Should_Detect_Change_When_Only_Single_Field_Differs()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var original = ProductDataTestFactory.CreateData();
        var dataWithOnlyNameChanged = new ProductUpdateInfoData(
            ProductName.Create("Different Name").Value,
            original.Slug,
            original.Description,
            original.ShortDescription,
            original.SeoMetadata);

        // Act
        var result = product.UpdateInfo(
            ProductContextsTestFactory.ValidUpdateInfoContext(),
            dataWithOnlyNameChanged,
            product.CategoryId,
            product.BrandId, 
            UpdatedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Version.Should().Be(1);
        product.DomainEvents.Should().ContainSingle();
    }

    [Theory]
    [InlineData(false, true, true, true)]
    [InlineData(true, false, true, true)]
    [InlineData(true, true, false, true)]
    [InlineData(true, true, true, false)]
    public void UpdateInfo_Should_Fail_When_Data_Differs_And_Specification_Is_Not_Satisfied(
        bool vendorIsActive,
        bool canBeModified,
        bool categoryIsActive, 
        bool brandIsAssignable)
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var invalidCtx = new ProductUpdateInfoContext(
            vendorIsActive, 
            canBeModified, 
            categoryIsActive, 
            brandIsAssignable);
        var newData = ProductDataTestFactory.UpdateInfoData("_changed");

        // Act
        var result = product.UpdateInfo(invalidCtx, newData, product.CategoryId, product.BrandId, UpdatedAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        product.Name.Should().Be(ProductDataTestFactory.CreateData().Name);
        product.Version.Should().Be(0);
        product.DomainEvents.Should().BeEmpty();
    }
}