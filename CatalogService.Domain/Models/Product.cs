using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.RequestData;
using CatalogService.Domain.Specifications.Product;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Characteristics;
using migApp.Shared.Enums.Products;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Models;

public sealed class Product : AggregateRoot 
{
    private Product() : base(Guid.Empty) { }

    private Product(
        Guid id,
        ProductName name,
        Slug slug,
        Description description,
        ShortDescription shortDescription,
        Guid categoryId,
        Guid vendorId,
        Guid brandId,
        SeoMetadata seoMetadata,
        DateTimeOffset createdAt) : base(id)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ShortDescription = shortDescription;
        CategoryId = categoryId;
        VendorId = vendorId;
        BrandId = brandId;
        SeoMetadata = seoMetadata;
        CreatedAt = createdAt;
    }

    public ProductName Name { get; private set; }
    public Slug Slug { get; private set; }
    public Description Description { get; private set; }
    public ShortDescription ShortDescription { get; private set; }

    public Guid CategoryId { get; private set; }
    public Guid VendorId { get; private set; }
    public Guid BrandId { get; private set; }

    public ProductStatus ProductStatus { get; private set; } = ProductStatus.Draft;

    private readonly HashSet<ProductSuspensionReason> _suspensionReasons = [];
    public IReadOnlyCollection<ProductSuspensionReason> SuspensionReasons => _suspensionReasons;
    public bool IsLockedByAdmin { get; private set; }
    public bool CanBeModified =>
        ProductStatus != ProductStatus.Archived &&
        !IsLockedByAdmin &&
        !IsDeleted;

    public bool IsVisiblePublicly =>
        ProductStatus == ProductStatus.Published &&
        !IsLockedByAdmin &&
        !IsDeleted;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;
    public DateTimeOffset? DeletedAt { get; private set; }

    public SeoMetadata SeoMetadata { get; private set; }

    public static int MaxAttributes => 30;

    private readonly List<ProductAttribute> _attributes = [];
    public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

    public static int MaxTags => 50;

    private readonly List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags;

    public static IResult<Product> Create(
        ProductCreationContext ctx,
        ProductCreationData data,
        Guid vendorId,
        Guid categoryId,
        Guid brandId,
        DateTimeOffset now)
    {
        var result = ProductCreationSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<Product>(result.Error);

        var product = new Product(
            Guid.NewGuid(),
            data.Name,
            data.Slug,
            data.Description,
            data.ShortDescription,
            categoryId,
            vendorId,
            brandId,
            data.SeoMetadata,
            now);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            product.Id,
            product.CategoryId,
            product.VendorId,
            product.BrandId,
            product.Name,
            product.Slug,
            product.Description,
            product.ShortDescription,
            product.SeoMetadata,
            product.ProductStatus,
            product.CanBeModified,
            product.IsVisiblePublicly,
            product.Version));

        return Ok(product);
    }

    public IResult UpdateInfo(
        ProductUpdateInfoContext ctx,
        ProductUpdateInfoData data,
        Guid categoryId,
        Guid brandId,
        DateTimeOffset now)
    {
        if (Name == data.Name &&
            Slug == data.Slug &&
            Description == data.Description &&
            ShortDescription == data.ShortDescription &&
            CategoryId == categoryId &&
            BrandId == brandId &&
            SeoMetadata == data.SeoMetadata)
            return Ok();

        var result = ProductUpdateInfoSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return Fail<Product>(result.Error);

        Name = data.Name;
        Slug = data.Slug;
        Description = data.Description;
        ShortDescription = data.ShortDescription;
        CategoryId = categoryId;
        BrandId = brandId;
        SeoMetadata = data.SeoMetadata;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductInfoUpdatedDomainEvent(
            Id,
            CategoryId,
            BrandId,
            Name,
            Slug,
            Description,
            ShortDescription,
            SeoMetadata,
            CanBeModified,
            Version));

        return Ok();
    }

    public IResult ReplaceAttributes(
        ProductAttributesReplaceContext ctx,
        List<ProductAttribute> newAttributes,
        DateTimeOffset now)
    {
        var result = ProductAttributesReplaceSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _attributes.RemoveAll(a => !a.IsVariable);

        var variableCharIds = _attributes
            .Where(a => a.IsVariable)
            .Select(a => a.CharacteristicId)
            .ToHashSet();

        var toAdd = newAttributes
            .Where(a => !variableCharIds.Contains(a.CharacteristicId))
            .ToList();

        _attributes.AddRange(toAdd);
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductAttributesReplacedDomainEvent(
            Id,
            _attributes,
            Version));

        return Ok();
    }

    public IResult AddVariantToAttributes(
        ProductAddVariantToAttributesContext ctx,
        List<(Guid CharacteristicId, AttributeName Name, AttributeCharType CharType, AttributeGroupName? GroupName, AttributeVariableValue VariableValue)> variantValues,
        DateTimeOffset now)
    {
        var result = ProductAddVariantToAttributesSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        foreach (var item in variantValues)
        {
            var attribute = _attributes
                .FirstOrDefault(a => a.CharacteristicId == item.CharacteristicId);

            if (attribute is null)
            {
                var createResult = ProductAttribute.CreateVariable(
                    item.CharacteristicId,
                    item.Name,
                    item.CharType,
                    item.GroupName);

                if (createResult.IsFailure)
                    return createResult;

                attribute = createResult.Value;
                attribute.MarkAsVariable();
                _attributes.Add(attribute);
            }
            else
            {
                attribute.MarkAsVariable();
            }

            var addResult = attribute.AddVariableValue(item.VariableValue);
            if (addResult.IsFailure)
                return addResult;
        }

        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductVariantAddedDomainEvent(
            Id,
            _attributes,
            Version));

        return Ok();
    }

    public IResult RemoveVariantFromAttributes(
        ProductRemoveVariantFromAttributesContext ctx,
        Guid variantId,
        DateTimeOffset now)
    {
        var validation = ProductRemoveVariantFromAttributesSpecification.Spec.IsSatisfiedBy(ctx);
        if (validation.IsFailure)
            return validation;

        foreach (var attribute in _attributes.Where(a => a.IsVariable))
        {
            var result = attribute.RemoveVariableValue(variantId);
            if (result.IsFailure)
                return result;    
        }

        _attributes.RemoveAll(a => a.IsVariable && a.VariableValues.Count == 0);

        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductVariantRemovedDomainEvent(
            Id,
            _attributes,
            Version));

        return Ok();
    }

    public IResult ReplaceTags(
        ProductTagsReplaceContext ctx,
        List<Tag> tags,
        DateTimeOffset now)
    {
        if (_tags.SequenceEqual(tags))
            return Ok();

        var result = ProductTagsReplaceSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _tags.Clear();
        _tags.AddRange(tags);
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductTagsReplacedDomainEvent(
            Id,
            _tags,
            Version));

        return Ok();
    }

    public IResult Publish(
        ProductPublishContext ctx,
        DateTimeOffset now)
    {
        if (ProductStatus == ProductStatus.Published)
            return Ok();

        var result = ProductPublishSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _suspensionReasons.Clear();
        ProductStatus = ProductStatus.Published;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductPublishedDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            ProductStatus,
            [.. _suspensionReasons],
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Unpublish(
        ProductUnpublishContext ctx,
        DateTimeOffset now)
    {
        if (ProductStatus != ProductStatus.Published) 
            return Ok();

        var result = ProductUnpublishSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        ProductStatus = ProductStatus.Draft;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductUnpublishedDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            ProductStatus,
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Lock(
        ProductLockContext ctx,
        DateTimeOffset now)
    {
        if (IsLockedByAdmin)
            return Ok();

        var result = ProductLockSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        IsLockedByAdmin = true;
        ProductStatus = ProductStatus.Draft;

        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductLockedDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            IsLockedByAdmin,
            ProductStatus,
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Unlock(
        ProductUnlockContext ctx,
        DateTimeOffset now)
    {
        if (!IsLockedByAdmin)
            return Ok();

        var result = ProductUnlockSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        IsLockedByAdmin = false;
        ProductStatus = ProductStatus.Draft;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductUnlockedDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            IsLockedByAdmin,
            ProductStatus,
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Suspend(
        ProductSuspendContext ctx,
        ProductSuspensionReason reason,
        DateTimeOffset now)
    {
        if (ProductStatus != ProductStatus.Published && ProductStatus != ProductStatus.Suspended)
            return Ok();

        var result = ProductSuspendSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _suspensionReasons.Add(reason);

        if (ProductStatus == ProductStatus.Suspended)
            return Ok();

        ProductStatus = ProductStatus.Suspended;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductSuspendedDomainEvent(
            Id,
            VendorId,
            ProductStatus,
            [.. _suspensionReasons],
            IsVisiblePublicly,
            Version));


        return Ok();
    }

    public IResult TryRestore(
        ProductTryRestoreContext ctx,
        ProductSuspensionReason reason,
        DateTimeOffset now)
    {
        if (ProductStatus != ProductStatus.Suspended)
            return Ok();

        var result = ProductTryRestoreSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _suspensionReasons.Remove(reason);

        if (_suspensionReasons.Count != 0)
            return Ok();

        ProductStatus = ProductStatus.Draft;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductUnsuspendedDomainEvent(
            Id,
            VendorId,
            ProductStatus,
            [.. _suspensionReasons],
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Archive(
        ProductArchiveContext ctx, 
        DateTimeOffset now)
    {
        if (ProductStatus == ProductStatus.Archived)
            return Ok();

        var result = ProductArchiveSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        _suspensionReasons.Clear();
        ProductStatus = ProductStatus.Archived;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductArchivedDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            ProductStatus,
            [.._suspensionReasons],
            IsVisiblePublicly,
            Version));

        return Ok();
    }

    public IResult Restore(ProductRestoreContext ctx, DateTimeOffset now)
    {
        if (ProductStatus != ProductStatus.Archived)
            return Ok();

        var result = ProductRestoreSpecification.Spec.IsSatisfiedBy(ctx);
        if (result.IsFailure)
            return result;

        ProductStatus = ProductStatus.Draft;
        UpdatedAt = now;

        IncreaseVersion();

        RaiseDomainEvent(new ProductRestoredDomainEvent(
            Id,
            CategoryId,
            VendorId,
            CanBeModified,
            ProductStatus,
            IsVisiblePublicly,
            Version));
        
        return Ok();
    }

    public IResult Delete(ProductDeleteContext ctx, DateTimeOffset now)
        => DeleteInternal(ctx, now);

    public IResult ForceDelete(DateTimeOffset now)
        => DeleteInternal(null, now);

    private IResult DeleteInternal(ProductDeleteContext? ctx, DateTimeOffset now)
    {
        if (IsDeleted) return Ok();

        if (ctx is not null)
        {
            var result = ProductDeleteSpecification.Spec.IsSatisfiedBy(ctx);
            if (result.IsFailure)
                return result;
        }

        DeletedAt = now;

        if (ProductStatus == ProductStatus.Published)
            ProductStatus = ProductStatus.Draft;

        RaiseDomainEvent(new ProductDeletedDomainEvent(Id));

        return Ok();
    }
}