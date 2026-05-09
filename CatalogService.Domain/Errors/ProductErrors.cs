using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductErrors
{
    public static Error ImageNotFound() => Error.NotFound(ProductErrorCodes.ImageNotFound);
    public static Error NoVariations() => Error.InvalidArgument(ProductErrorCodes.NoVariations);
    public static Error AttributesRequired() => Error.InvalidArgument(ProductErrorCodes.AttributesRequired);
    public static Error TagsRequired() => Error.InvalidArgument(ProductErrorCodes.TagsRequired);
    public static Error ImagesRequired() => Error.InvalidArgument(ProductErrorCodes.ImagesRequired);
    public static Error MaxAttributesReached() => Error.InvalidArgument(ProductErrorCodes.MaxAttributesReached);
    public static Error MaxTagsReached() => Error.InvalidArgument(ProductErrorCodes.MaxTagsReached);
    public static Error InvalidOldPrice() => Error.InvalidArgument(ProductErrorCodes.InvalidOldPrice);
    public static Error NotFound() => Error.NotFound(ProductErrorCodes.NotFound);
    public static Error CannotModify() => Error.InvalidArgument(ProductErrorCodes.CannotModify);
    public static Error VendorMismatch() => Error.Unauthenticated(ProductErrorCodes.VendorMismatch);
    public static Error VariationDoesNotBelongToProduct() => Error.InvalidArgument(ProductErrorCodes.VariationDoesNotBelongToProduct);
}

public static class ProductErrorCodes
{
    public const string ImageNotFound = "Product.ImageNotFound";
    public const string NoVariations = "Product.NoVariations";
    public const string AttributesRequired = "Product.AttributesRequired";
    public const string TagsRequired = "Product.TagsRequired";
    public const string ImagesRequired = "Product.ImagesRequired";
    public const string MaxAttributesReached = "Product.MaxAttributesReached";
    public const string MaxTagsReached = "Product.MaxTagsReached";
    public const string InvalidOldPrice = "Product.InvalidOldPrice";
    public const string NotFound = "Product.NotFound";
    public const string InvalidId = "Product.InvalidId";
    public const string CannotModify = "Product.CannotModify";
    public const string VendorMismatch = "Product.VendorMismatch";
    public const string VariationDoesNotBelongToProduct = "Product.VariationDoesNotBelongToProduct";
}