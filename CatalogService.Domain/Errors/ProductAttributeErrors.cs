using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductAttributeErrors
{
    public static Error UnifyingAttributeRequired() =>
        Error.InvalidArgument(ProductAttributeErrorCodes.UnifyingAttributeRequired,
            "A unifying attribute is required.");

    public static Error NotVariable() =>
        Error.InvalidArgument(ProductAttributeErrorCodes.NotVariable,
            "The attribute is not variable.");
}

public static class ProductAttributeErrorCodes
{
    public const string UnifyingAttributeRequired = "ProductAttribute.UnifyingAttributeRequired";
    public const string CannotSpecifyBothValueAndVariations = "ProductAttribute.CannotSpecifyBothValueAndVariations";
    public const string ValueOrVariationRequired = "ProductAttribute.ValueOrVariationRequired";
    public const string NotVariable = "ProductAttribute.NotVariable";
}