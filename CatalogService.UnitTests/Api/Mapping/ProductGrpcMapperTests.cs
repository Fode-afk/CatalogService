using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Api.Grpc.Mapping;
using FluentAssertions;

namespace CatalogService.UnitTests.Api.Mapping;

public sealed class ProductGrpcMapperTests
{
    [Fact]
    public void ToCreateCommand_ShouldMapAllProperties()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var request = new CreateProductRequest
        {
            VendorId = vendorId.ToString(),
            CategoryId = categoryId.ToString(),
            BrandId = brandId.ToString(),
            Name = "iPhone 17",
            Slug = "iphone-17",
            Description = "Product description",
            ShortDescription = "Short description",
            SeoTitle = "iPhone 17 - Buy Online",
            SeoDescription = "Buy iPhone 17 online",
            SeoKeywords = "iphone, apple, smartphone"
        };

        // Act
        var command = request.ToCreateCommand();

        // Assert
        command.VendorId.Should().Be(vendorId);
        command.CategoryId.Should().Be(categoryId);
        command.BrandId.Should().Be(brandId);
        command.Name.Should().Be(request.Name);
        command.Slug.Should().Be(request.Slug);
        command.Description.Should().Be(request.Description);
        command.ShortDescription.Should().Be(request.ShortDescription);
        command.SeoTitle.Should().Be(request.SeoTitle);
        command.SeoDescription.Should().Be(request.SeoDescription);
        command.SeoKeywords.Should().Be(request.SeoKeywords);
    }

    [Fact]
    public void ToUpdateInfoCommand_ShouldMapAllProperties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var request = new UpdateProductInfoRequest
        {
            ProductId = productId.ToString(),
            VendorId = vendorId.ToString(),
            Name = "Updated product",
            Slug = "updated-product",
            Description = "Updated description",
            ShortDescription = "Updated short description",
            CategoryId = categoryId.ToString(),
            BrandId = brandId.ToString(),
            SeoTitle = "Updated SEO title",
            SeoDescription = "Updated SEO description",
            SeoKeywords = "updated, product"
        };

        // Act
        var command = request.ToUpdateInfoCommand();

        // Assert
        command.ProductId.Should().Be(productId);
        command.VendorId.Should().Be(vendorId);
        command.Name.Should().Be(request.Name);
        command.Slug.Should().Be(request.Slug);
        command.Description.Should().Be(request.Description);
        command.ShortDescription.Should().Be(request.ShortDescription);
        command.CategoryId.Should().Be(categoryId);
        command.BrandId.Should().Be(brandId);
        command.SeoTitle.Should().Be(request.SeoTitle);
        command.SeoDescription.Should().Be(request.SeoDescription);
        command.SeoKeywords.Should().Be(request.SeoKeywords);
    }

    [Fact]
    public void ToReplaceProductAttributesCommand_ShouldMapIdsAndAttributes()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var attributeId1 = Guid.NewGuid();
        var attributeId2 = Guid.NewGuid();

        var request = new ReplaceProductAttributesRequest
        {
            VendorId = vendorId.ToString(),
            ProductId = productId.ToString()
        };

        request.Attributes.Add(
            attributeId1.ToString(),
            "Black");

        request.Attributes.Add(
            attributeId2.ToString(),
            "128 GB");

        // Act
        var command = request.ToReplaceProductAttributesCommand();

        // Assert
        command.VendorId.Should().Be(vendorId);
        command.ProductId.Should().Be(productId);

        command.Attributes.Should().BeEquivalentTo(
            new Dictionary<Guid, string>
            {
                [attributeId1] = "Black",
                [attributeId2] = "128 GB"
            });
    }

    [Fact]
    public void ToReplaceProductTagsCommand_ShouldMapIdsAndTags()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var request = new ReplaceProductTagsRequest
        {
            VendorId = vendorId.ToString(),
            ProductId = productId.ToString()
        };

        request.Tags.Add("smartphone");
        request.Tags.Add("apple");
        request.Tags.Add("electronics");

        // Act
        var command = request.ToReplaceProductTagsCommand();

        // Assert
        command.VendorId.Should().Be(vendorId);
        command.ProductId.Should().Be(productId);

        command.Tags.Should().BeEquivalentTo(
            [
                "smartphone",
                "apple",
                "electronics"
            ]);
    }
}