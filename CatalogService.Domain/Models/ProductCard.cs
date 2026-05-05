using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.RequestData;
using CatalogService.Domain.Specifications.ProductCard;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.ProductCards;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Models;

public sealed class ProductCard : AggregateRoot 
{
    private ProductCard() : base(Guid.Empty) { }

    private ProductCard(
        Guid id,
        Name name,
        Slug slug,
        Description description,
        ShortDescription shortDescription,
        Guid categoryId,
        Guid vendorId,
        Brand brand,
        SeoMetadata seoMetadata,
        List<ProductCardAttribute> attributes,
        List<Tag> tags,
        DateTimeOffset createdAt) : base(id)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ShortDescription = shortDescription;
        CategoryId = categoryId;
        VendorId = vendorId;
        Brand = brand;
        SeoMetadata = seoMetadata;
        _attributes = attributes;
        _tags = tags;
        CreatedAt = createdAt;
    }

    public const int MaxAttributes = 30;
    public const int MaxTags = 50;
    public const int MaxImages = 10;

    public Name Name { get; private set; }
    public Slug Slug { get; private set; }
    public Description Description { get; private set; }
    public ShortDescription ShortDescription { get; private set; }

    public Guid? DefaultProductId { get; private set; }
    public ProductCount ProductCount { get; private set; } = ProductCount.Zero;

    public Guid CategoryId { get; private set; }

    public Guid VendorId { get; private set; }

    public Brand Brand { get; private set; }

    public ProductCardStatus ProductCardStatus { get; private set; } = ProductCardStatus.Draft;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public SeoMetadata SeoMetadata { get; private set; }

    private List<ProductCardAttribute> _attributes = [];
    public IReadOnlyCollection<ProductCardAttribute> Attributes => _attributes;

    private readonly List<ProductCardImage> _images = [];
    public IReadOnlyCollection<ProductCardImage> Images => _images;

    private List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags;

    public static IResult<ProductCard> Create(
        ProductCardCreationContext ctx,
        ProductCardCreationData data,
        Guid categoryId,
        Guid vendorId,
        DateTimeOffset now)
    {
        var result = ProductCardCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<ProductCard>(result.Error);

        var product = new ProductCard(
            Guid.NewGuid(),
            data.Name,
            data.Slug,
            data.Description,
            data.ShortDescription,
            categoryId,
            vendorId,
            data.Brand,
            data.SeoMetadata,
            [..data.Attributes],
            [..data.Tags],
            now);

        product.RaiseDomainEvent(new ProductCardCreatedDomainEvent(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.ShortDescription,
            product.CategoryId,
            product.VendorId,
            product.Brand,
            product.ProductCardStatus,
            product.SeoMetadata,
            [.. product.Attributes],
            [.. product.Tags],
            product.CreatedAt));

        return Ok(product);
    }

    public IResult UpdateInfo(
        ProductCardUpdateInfoContext ctx,
        ProductCardUpdateInfoData data,
        Guid categoryId,
        DateTimeOffset now)
    {
        if (Name == data.Name &&
            Description == data.Description &&
            ShortDescription == data.ShortDescription &&
            CategoryId == categoryId &&
            Brand == data.Brand &&
            SeoMetadata == data.SeoMetadata)
            return Ok();

        var result = ProductCardUpdateInfoSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<ProductCard>(result.Error);

        Name = data.Name;
        Description = data.Description;
        ShortDescription = data.ShortDescription;
        CategoryId = categoryId;
        Brand = data.Brand;
        SeoMetadata = data.SeoMetadata;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardInfoUpdatedDomainEvent(
            Id,
            Name,
            Description,
            ShortDescription,
            CategoryId,
            Brand,
            SeoMetadata,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult Publish(
        ProductCardPublishContext ctx,
        DateTimeOffset now)
    {
        if (ProductCardStatus == ProductCardStatus.Published)
            return Ok();

        var result = ProductCardPublishSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        ProductCardStatus = ProductCardStatus.Published;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardPublishedDomainEvent(
            Id,
            VendorId,
            ProductCardStatus,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult Archive(
        ProductCardArchivedContext ctx, 
        DateTimeOffset now)
    {
        if (ProductCardStatus == ProductCardStatus.Archived)
            return Ok();

        var result = ProductCardArchivedSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        ProductCardStatus = ProductCardStatus.Archived;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardArchivedDomainEvent(
            Id,
            VendorId,
            ProductCardStatus,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult SetDefaultProduct()
    {
        return Ok();
    }

    public IResult UnsetDefaultProduct()
    {
        return Ok();
    }

    public IResult ReplaceAttributes(
        ProductCardAttributesReplacedContext ctx,
        List<ProductCardAttribute> attributes,
        DateTimeOffset now)
    {
        if (_attributes.SequenceEqual(attributes))
            return Ok();

        var result = ProductCardAttributesReplacedSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _attributes = [.. attributes];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardAttributesReplacedDomainEvent(
            Id,
            [..Attributes],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult ReplaceTags(
        ProductCardTagsReplacedContext ctx,
        List<Tag> tags,
        DateTimeOffset now)
    {
        if (_tags.SequenceEqual(tags))
            return Ok();

        var result = ProductCardTagsReplacedSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _tags = [.. tags];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardTagsReplacedDomainEvent(
            Id,
            [.. Tags],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult IncrementProductCount(ProductCountIncrementedContext ctx, DateTimeOffset now)
    {
        var validation = ProductCountIncrementedSpecification.Spec.IsSatisfiedBy(ctx);
        if (validation.IsFailure) 
            return validation;

        var result = ProductCount.Create(ProductCount.Value + 1);

        if (result.IsFailure)
            return result;

        ProductCount = result.Value;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardCountIncrementedDomainEvent(
            Id,
            ProductCount,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult DecrementProductCount(ProductCountDecrementedContext ctx, DateTimeOffset now)
    {
        var validation = ProductCountDecrementedSpecification.Spec.IsSatisfiedBy(ctx);
        if (validation.IsFailure)
            return validation;

        var result = ProductCount.Create(ProductCount.Value - 1);

        if (result.IsFailure)
            return result;

        ProductCount = result.Value;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardCountDecrementedDomainEvent(
            Id,
            ProductCount,
            UpdatedAt.Value));

        return Ok();
    }

    public IResult AddProductCardImage(
        ProductCardAddImageContext ctx,
        AddProductCardImageData data,
        bool isMain,
        DateTimeOffset now)
    {
        var result = ProductCardAddImageSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var imageResult = ProductCardImage.Create(Id, data.Url, data.Alt, _images.Count, isMain);

        if (imageResult.IsFailure)
            return imageResult;

        var image = imageResult.Value;

        if (isMain || _images.Count == 0)
        {
            foreach (var img in _images)
                img.SetAsMain(false);

            image.SetAsMain(true);
        }

        _images.Add(image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardImageAddedDomainEvent(
            Id,
            [..Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult RemoveProductCardImage(
        ProductCardRemoveImageContext ctx,
        ImageUrl url,
        DateTimeOffset now)
    {
        var result = ProductCardRemoveImageSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        bool wasMain = image.IsMain;
        _images.Remove(image);

        RecalculateImageOrder();
        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardImageRemovedDomainEvent(
            Id,
            [.. Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult ChangeImageOrder(
        ProductCardChangeImageOrderContext ctx,
        ImageUrl url, 
        int newOrder,
        DateTimeOffset now)
    {
        var result = ProductCardChangeImageOrderSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var image = _images.FirstOrDefault(x => x.Url == url);
        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        if (newOrder < 0 || newOrder >= _images.Count)
            return Fail(ProductCardImageErrors.InvalidSortOrder());

        _images.Remove(image);
        _images.Insert(newOrder, image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardImageOrderChangedDomainEvent(
            Id,
            [.. Images],
            UpdatedAt.Value));

        return Ok();
    }

    private void RecalculateImageOrder()
    {
        for (int i = 0; i < _images.Count; i++)
            _images[i].ChangeOrder(i);
    }

    public IResult SetMainImage(
        ProductCardSetMainImageContext ctx,
        ImageUrl url,
        DateTimeOffset now)
    {
        var result = ProductCardSetMainImageSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        var image = _images.FirstOrDefault(x => x.Url == url);

        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        foreach (var img in _images)
            img.SetAsMain(false);

        image.SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardImageSetMainDomainEvent(
            Id,
            [.. Images],
            UpdatedAt.Value));

        return Ok();
    }

    public IResult UpdateImageAlt(
        ProductCardUpdateImageAltContext ctx,
        ProductCardUpdateImageAltData data,
        DateTimeOffset now)
    {
        var validation = ProductCardUpdateImageAltSpecification.Spec.IsSatisfiedBy(ctx);
        if (validation.IsFailure)
            return validation;

        var image = _images.FirstOrDefault(x => x.Url == data.Url);

        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        var result = image.UpdateAlt(data.Alt);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardImageAltUpdatedDomainEvent(
            Id,
            [.. Images],
            UpdatedAt.Value));

        return Ok();
    }
}