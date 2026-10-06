using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.Specifications.Product;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductPublishSpecificationTests
{
    public static TheoryData<
        bool,
        bool,
        bool,
        bool,
        IReadOnlyCollection<ProductAttribute>,
        IReadOnlyCollection<Tag>,
        List<ProductVariantSnapshot>,
        List<ProductVariantPriceSnapshot>,
        string?> TestCases =>
        new()
        {
            // Vendor is inactive
            {
                false,
                true,
                true,
                true,
                [],
                [],
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                VendorSnapshotErrorCodes.CannotModify
            },

            // Product cannot be modified
            {
                true,
                false,
                true,
                true,
                [],
                [],
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                ProductErrorCodes.CannotEditContent
            },

            // Brand is not assignable
            {
                true,
                true,
                false,
                true,
                [],
                [],
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                BrandSnapshotErrorCodes.Inactive
            },

            // Category is inactive
            {
                true,
                true,
                true,
                false,
                [],
                [],
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                CategorySnapshotErrorCodes.Inactive
            },

            // Attributes are not provided
            {
                true,
                true,
                true,
                true,
                [],
                CreateTags(1),
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                ProductErrorCodes.AttributesRequired
            },

            // Tags are not provided
            {
                true,
                true,
                true,
                true,
                CreateAttributes(1),
                [],
                CreateVariations(1, hasMainImage: true),
                CreatePrices(1, hasPrice: true),
                ProductErrorCodes.TagsRequired
            },

            // Not every variation has a price
            {
                true,
                true,
                true,
                true,
                CreateAttributes(1),
                CreateTags(1),
                CreateVariations(2, hasMainImage: true),
                CreatePrices(2, hasPrice: false, pricesWithPrice: 1),
                ProductVariantPriceSnapshotErrorCodes.NoPrice
            },

            // Not every variation has a main image
            {
                true,
                true,
                true,
                true,
                CreateAttributes(1),
                CreateTags(1),
                CreateVariations(2, hasMainImage: false, variationsWithMainImage: 1),
                CreatePrices(2, hasPrice: true),
                ProductVariantSnapshotErrorCodes.ImagesRequired
            },

            // Everything is valid
            {
                true,
                true,
                true,
                true,
                CreateAttributes(1),
                CreateTags(1),
                CreateVariations(2, hasMainImage: true),
                CreatePrices(2, hasPrice: true),
                null
            }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool canBeModified,
        bool brandIsAssignable,
        bool categoryIsActive,
        IReadOnlyCollection<ProductAttribute> attributes,
        IReadOnlyCollection<Tag> tags,
        List<ProductVariantSnapshot> variationSnapshots,
        List<ProductVariantPriceSnapshot> priceSnapshots,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductSubmitForPublishContext(
            vendorIsActive,
            categoryIsActive,
            brandIsAssignable,
            canBeModified,
            attributes,
            tags,
            variationSnapshots,
            priceSnapshots);

        // Act
        var result = ProductSubmitForPublishSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<ProductVariantSnapshot> CreateVariations(
        int count,
        bool hasMainImage,
        int? variationsWithMainImage = null)
    {
        var imageCount = variationsWithMainImage ?? count;

        return [.. Enumerable
            .Range(0, count)
            .Select(i => new ProductVariantSnapshot
            {
                ProductVariantId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                HasMainImage = i < imageCount ? hasMainImage : !hasMainImage,
                UpdatedAt = DateTimeOffset.UtcNow,
                Version = 1
            })];
    }

    private static List<ProductVariantPriceSnapshot> CreatePrices(
        int count,
        bool hasPrice,
        int? pricesWithPrice = null)
    {
        var priceCount = pricesWithPrice ?? count;

        return [.. Enumerable
            .Range(0, count)
            .Select(i => new ProductVariantPriceSnapshot
            {
                ProductVariantId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                HasPrice = i < priceCount ? hasPrice : !hasPrice,
                UpdatedAt = DateTimeOffset.UtcNow,
                Version = 1
            })];
    }

    private static IReadOnlyCollection<ProductAttribute> CreateAttributes(int count)
    {
        return [..Enumerable
            .Range(0, count)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric).Value)];
    }

    private static IReadOnlyCollection<Tag> CreateTags(int count)
    {
        return [..Enumerable
            .Range(0, count)
            .Select(i => Tag.Create($"tag-{i}").Value)];
    }
}