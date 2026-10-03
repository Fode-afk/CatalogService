using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductErrors
{
    public static Error NoVariations() =>
        Error.InvalidArgument(ProductErrorCodes.NoVariations,
            "No variations available.");

    public static Error AttributesRequired() =>
        Error.InvalidArgument(ProductErrorCodes.AttributesRequired,
            "Attributes are required.");

    public static Error TagsRequired() =>
        Error.InvalidArgument(ProductErrorCodes.TagsRequired,
            "Tags are required.");

    public static Error MaxAttributesReached(int maxAttributes) =>
        Error.InvalidArgument(ProductErrorCodes.MaxAttributesReached,
            $"Maximum number of attributes ({maxAttributes}) reached.");

    public static Error MaxTagsReached(int maxTags) =>
        Error.InvalidArgument(ProductErrorCodes.MaxTagsReached,
            $"Maximum number of tags ({maxTags}) reached.");

    public static Error NotFound() =>
        Error.NotFound(ProductErrorCodes.NotFound,
            "Product not found.");

    public static Error CannotModify() =>
        Error.InvalidArgument(ProductErrorCodes.CannotModify,
            "The product cannot be modified.");

    public static Error VendorMismatch() =>
        Error.Unauthenticated(ProductErrorCodes.VendorMismatch,
            "Vendor mismatch.");

    public static Error AlreadyExists() =>  
        Error.AlreadyExists(ProductErrorCodes.AlreadyExists,
            "Product with the same ID already exists.");

    public static Error StaleSubmission() =>
        Error.InvalidArgument(ProductErrorCodes.StaleSubmission,
            "Stale submission.");
}

public static class ProductErrorCodes
{
    public const string NoVariations = "Product.NoVariations";
    public const string AttributesRequired = "Product.AttributesRequired";
    public const string TagsRequired = "Product.TagsRequired";
    public const string MaxAttributesReached = "Product.MaxAttributesReached";
    public const string MaxTagsReached = "Product.MaxTagsReached";
    public const string NotFound = "Product.NotFound";
    public const string InvalidId = "Product.InvalidId";
    public const string CannotModify = "Product.CannotModify";
    public const string VendorMismatch = "Product.VendorMismatch";
    public const string AlreadyExists = "Product.AlreadyExists";
    public const string StaleSubmission = "Product.StaleSubmission";
}