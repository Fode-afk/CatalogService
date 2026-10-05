using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductReplaceTagsTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    private static Tag CreateTag(string value)
    {
        return Tag.Create(value).Value;
    }

    [Fact]
    public void ReplaceTags_ShouldSucceed_WhenTagsAreValid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var tags = new[]
        {
            CreateTag("phone"),
            CreateTag("apple")
        };

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(tags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ReplaceTags_ShouldReplaceExistingTags()
    {
        // Arrange
        var oldTag = CreateTag("old");
        var newTag = CreateTag("new");

        var product = ProductTestFactory.CreateWithTags([oldTag]);

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext([newTag]),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Tags
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(newTag);

        product.Tags
            .Should()
            .NotContain(oldTag);
    }

    [Fact]
    public void ReplaceTags_ShouldReplaceMultipleTags()
    {
        // Arrange
        var oldTags = new[]
        {
            CreateTag("old-1"),
            CreateTag("old-2")
        };

        var newTags = new[]
        {
            CreateTag("new-1"),
            CreateTag("new-2"),
            CreateTag("new-3")
        };

        var product = ProductTestFactory.CreateWithTags(oldTags);

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(newTags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Tags
            .Should()
            .HaveCount(3);

        product.Tags
            .Should()
            .Equal(newTags);
    }

    [Fact]
    public void ReplaceTags_ShouldNotAllowReplacingTagsWithEmptyCollection()
    {
        // Arrange
        var tags = new[]
         {
            CreateTag("phone"),
            CreateTag("apple")
        };
        var product = ProductTestFactory.CreateWithTags(tags);

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext([]),
            Now);

        // Assert
        result.IsFailure.Should().BeTrue();
        product.Tags.Should().Equal(tags);
    }

    [Fact]
    public void ReplaceTags_ShouldNotChangeProduct_WhenTagsAreEqual()
    {
        // Arrange
        var tags = new[]
        {
            CreateTag("phone"),
            CreateTag("apple")
        };

        var product = ProductTestFactory.CreateWithTags(tags);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(tags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Tags.Should().Equal(tags);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceTags_ShouldValidateContext_WhenTagsAreEqual()
    {
        // Arrange
        var tags = new[]
        {
            CreateTag("phone"),
            CreateTag("apple")
        };

        var product = ProductTestFactory.CreateWithTags(tags);

        var invalidContext = new ProductTagsReplaceContext(
            VendorIsActive: false,
            CanEditContent: false,
            Tags: tags);

        var versionBefore = product.Version;

        // Act
        var result = product.ReplaceTags(
            invalidContext,
            Now);

        // Assert
        result.IsSuccess.Should().BeFalse();

        product.Tags.Should().Equal(tags);
        product.Version.Should().Be(versionBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceTags_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithTags([CreateTag("old")]);

        var newTags = new[]
        {
            CreateTag("new")
        };

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(newTags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void ReplaceTags_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithTags([CreateTag("old")]);

        var versionBefore = product.Version;

        var newTags = new[]
        {
            CreateTag("new")
        };

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(newTags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void ReplaceTags_ShouldRaiseDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithTags([CreateTag("old")]);

        var newTags = new[]
        {
            CreateTag("new")
        };

        // Act
        var result = product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(newTags),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductTagsReplacedDomainEvent>();
    }

    [Fact]
    public void ReplaceTags_ShouldRaiseEventWithCurrentProductVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithTags([CreateTag("old")]);


        var newTags = new[]
        {
            CreateTag("new")
        };

        // Act
        product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(newTags),
            Now);

        // Assert
        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductTagsReplacedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void ReplaceTags_ShouldFail_WhenContextIsInvalid()
    {
        // Arrange
        var oldTag = CreateTag("old");
        var newTag = CreateTag("new");

        var product = ProductTestFactory.CreateWithTags([oldTag]);

        var ctx = new ProductTagsReplaceContext(
            VendorIsActive: false,
            CanEditContent: true,
            Tags: [newTag]);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.ReplaceTags(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.Tags.Should().Equal(oldTag);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceTags_ShouldFail_WhenTagsAreNotProvided()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var ctx = new ProductTagsReplaceContext(
            VendorIsActive: true,
            CanEditContent: true,
            Tags: []);

        // Act
        var result = product.ReplaceTags(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
