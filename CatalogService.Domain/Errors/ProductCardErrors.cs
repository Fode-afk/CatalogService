using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductCardErrors
{
    public static Error ImageNotFound() => Error.NotFound(ProductCardErrorCodes.ImageNotFound);
    public static Error NoVariants() => Error.InvalidArgument(ProductCardErrorCodes.NoVariants);
    public static Error AttributesRequired() => Error.InvalidArgument(ProductCardErrorCodes.AttributesRequired);
    public static Error TagsRequired() => Error.InvalidArgument(ProductCardErrorCodes.TagsRequired);
    public static Error ImagesRequired() => Error.InvalidArgument(ProductCardErrorCodes.ImagesRequired);
    public static Error MaxAttributesReached() => Error.InvalidArgument(ProductCardErrorCodes.MaxAttributesReached);
    public static Error MaxTagsReached() => Error.InvalidArgument(ProductCardErrorCodes.MaxTagsReached);
    public static Error InvalidOldPrice() => Error.InvalidArgument(ProductCardErrorCodes.InvalidOldPrice);
    public static Error NotFound() => Error.NotFound(ProductCardErrorCodes.NotFound);
    public static Error NoDefaultProduct() => Error.InvalidArgument(ProductCardErrorCodes.NoDefaultProduct);
    public static Error CannotModify() => Error.InvalidArgument(ProductCardErrorCodes.CannotModify);
    public static Error VendorMismatch() => Error.Unauthenticated(ProductCardErrorCodes.VendorMismatch);
    public static Error ProductDoesNotBelongToCard() => Error.InvalidArgument(ProductCardErrorCodes.ProductDoesNotBelongToCard);
}

public static class ProductCardErrorCodes
{
    public const string ImageNotFound = "ProductCard.ImageNotFound";
    public const string NoVariants = "ProductCard.NoVariants";
    public const string AttributesRequired = "ProductCard.AttributesRequired";
    public const string TagsRequired = "ProductCard.TagsRequired";
    public const string ImagesRequired = "ProductCard.ImagesRequired";
    public const string MaxAttributesReached = "ProductCard.MaxAttributesReached";
    public const string MaxTagsReached = "ProductCard.MaxTagsReached";
    public const string InvalidOldPrice = "ProductCard.InvalidOldPrice";
    public const string NotFound = "ProductCard.NotFound";
    public const string NoDefaultProduct = "ProductCard.NoDefaultProduct";
    public const string InvalidId = "ProductCard.InvalidId";
    public const string CannotModify = "ProductCard.CannotModify";
    public const string VendorMismatch = "ProductCard.VendorMismatch";
    public const string ProductDoesNotBelongToCard = "ProductCard.ProductDoesNotBelongToCard";
}