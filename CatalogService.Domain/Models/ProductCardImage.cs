using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Models;

public sealed class ProductCardImage : Entity
{
    private ProductCardImage(
        Guid id,
        Guid productCardId,
        ImageUrl url,
        AltText alt,
        int sortOrder,
        bool isMain) : base(id)
    {
        ProductCardId = productCardId;
        Url = url;
        Alt = alt;
        SortOrder = sortOrder;
        IsMain = isMain;
    }

    public Guid ProductCardId { get; private set; }

    public ImageUrl Url { get; private set; }
    public AltText Alt { get; private set; }

    public int SortOrder { get; private set; }
    public bool IsMain { get; private set; }

    internal static IResult<ProductCardImage> Create(
        Guid productCardId,
        ImageUrl url,
        AltText alt,
        int sortOrder,
        bool isMain)
    {
        return Ok(
            new ProductCardImage(
                Guid.NewGuid(),
                productCardId,
                url,
                alt,
                sortOrder,
                isMain));
    }

    internal IResult SetAsMain(bool isMain)
    {
        if (IsMain == isMain)
            return Ok();

        IsMain = isMain;
        return Ok();
    }

    internal IResult UpdateAlt(AltText alt)
    {
        if (Alt == alt)
            return Ok();

        Alt = alt;

        return Ok();
    }

    internal IResult ChangeOrder(int newOrder)
    {
        if (newOrder < 0)
            return Fail(ProductCardImageErrors.InvalidSortOrder());

        if (SortOrder == newOrder)
            return Ok();

        SortOrder = newOrder;

        return Ok();
    }
}