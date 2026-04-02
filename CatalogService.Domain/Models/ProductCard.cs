using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Enums;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Models;

public sealed class ProductCard : AggregateRoot
{
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
        CreatedAt = createdAt;
    }

    public Name Name { get; private set; }
    public Slug Slug { get; private set; }
    public Description Description { get; private set; }
    public ShortDescription ShortDescription { get; private set; }

    public RatingSnapshot Rating { get; private set; } = RatingSnapshot.Empty;
    public int SkuCount { get; private set; }
    public Money? MinPrice { get; private set; }
    public Money? MaxPrice { get; private set; }
    public bool HasStock { get; private set; }

    public Guid CategoryId { get; private set; }
    public Guid VendorId { get; private set; }
    public Brand Brand { get; private set; }

    public ProductStatus Status { get; private set; } = ProductStatus.Draft;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public SeoMetadata SeoMetadata { get; private set; }

    private List<ProductCardAttribute> _attributes = [];
    public IReadOnlyCollection<ProductCardAttribute> Attributes => _attributes;

    private readonly List<ProductCardImage> _images = [];
    public IReadOnlyCollection<ProductCardImage> Images => _images;

    private List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags;

    public static IResult<ProductCard> Create(
        Name name,
        Slug slug,
        Description description,
        ShortDescription shortDescription,
        Guid categoryId,
        Guid vendorId,
        Brand brand,
        SeoMetadata seoMetadata,
        DateTimeOffset now)
    {
        var product = new ProductCard(
            Guid.NewGuid(),
            name,
            slug,
            description,
            shortDescription,
            categoryId,
            vendorId,
            brand,
            seoMetadata,
            now);

        product.RaiseDomainEvent(new ProductCardCreatedDomainEvent(product.Id));

        return Ok(product);
    }

    public IResult UpdateInfo(
        Name name,
        Description description,
        ShortDescription shortDescription,
        Guid categoryId,
        Brand brand,
        SeoMetadata seoMetadata,
        DateTimeOffset now)
    {
        if (Name == name &&
            Description == description &&
            ShortDescription == shortDescription &&
            CategoryId == categoryId &&
            Brand == brand &&
            SeoMetadata == seoMetadata)
            return Ok();

        Name = name;
        Description = description;
        ShortDescription = shortDescription;
        CategoryId = categoryId;
        Brand = brand;
        SeoMetadata = seoMetadata;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardInfoUpdatedDomainEvent(Id));

        return Ok();
    }

    public IResult Publish(DateTimeOffset now)
    {
        if (Status == ProductStatus.Published)
            return Ok();

        if (SkuCount == 0)
            return Fail("Product must have at least one variant");

        Status = ProductStatus.Published;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardPublishedDomainEvent(Id));

        return Ok();
    }

    public IResult Archive(DateTimeOffset now)
    {
        if (Status == ProductStatus.Archived)
            return Ok();

        Status = ProductStatus.Archived;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardArchivedDomainEvent(Id));

        return Ok();
    }

    public IResult UpdateRating(RatingSnapshot rating, DateTimeOffset now)
    {
        if (Rating == rating)
            return Ok();

        Rating = rating;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductCardRatingUpdatedDomainEvent(Id));

        return Ok();
    }

    public IResult ReplaceAttributes(
        List<ProductCardAttribute> attributes,
        DateTimeOffset now)
    {
        if (_attributes.SequenceEqual(attributes))
            return Ok();

        _attributes = [.. attributes];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductAttributesReplacedDomainEvent(Id));

        return Ok();
    }

    public IResult ReplaceTags(
        List<string> tags,
        DateTimeOffset now)
    {
        if (_tags.SequenceEqual(tags))
            return Ok();

        _tags = [.. tags];
        UpdatedAt = now;

        RaiseDomainEvent(new ProductTagsReplacedDomainEvent(Id));

        return Ok();
    }

    public void IncrementSkuCount(DateTimeOffset now)
    {
        SkuCount++;
        UpdatedAt = now;
    }

    public void DecrementSkuCount(DateTimeOffset now)
    {
        SkuCount--;
        UpdatedAt = now;
    }

    public IResult AddProductImage(
        ImageUrl url,
        AltText alt,
        bool isMain,
        DateTimeOffset now)
    {
        var imageResult = ProductCardImage.Create(Id, url, alt, _images.Count, isMain);

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

        RaiseDomainEvent(new ProductCardImageAddedDomainEvent(Id));
        return Ok();
    }

    public IResult RemoveProductImage(Guid imageId, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        bool wasMain = image.IsMain;
        _images.Remove(image);

        RecalculateImageOrder();

        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageRemovedDomainEvent(Id));
        return Ok();
    }

    public IResult ChangeImageOrder(Guid imageId, int newOrder, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        if (newOrder < 0 || newOrder >= _images.Count)
            return Fail(ProductCardImageErrors.InvalidSortOrder());

        _images.Remove(image);
        _images.Insert(newOrder, image);

        RecalculateImageOrder();

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageOrderChangedDomainEvent(Id));
        return Ok();
    }

    private void RecalculateImageOrder()
    {
        for (int i = 0; i < _images.Count; i++)
            _images[i].ChangeOrder(i);
    }

    public IResult SetMainImage(Guid imageId, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        foreach (var img in _images)
            img.SetAsMain(false);

        image.SetAsMain(true);

        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageSetMainDomainEvent(Id));

        return Ok();
    }

    public IResult UpdateImageAlt(Guid imageId, AltText alt, DateTimeOffset now)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return Fail(ProductCardErrors.ImageNotFound());

        var result = image.UpdateAlt(alt);

        if (result.IsFailure)
            return result;

        UpdatedAt = now;
        UpdatedAt = now;

        RaiseDomainEvent(new ProductImageAltUpdatedDomainEvent(Id));

        return Ok();
    }
}