using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductAttributeErrors
{
    public static Error CannotBeBothVariableAndUnifying() => Error.InvalidArgument(ProductAttributeErrorCodes.CannotBeBothVariableAndUnifying);
    public static Error VariableValuesOnNonVariableAttribute() => Error.InvalidArgument(ProductAttributeErrorCodes.VariableValuesOnNonVariableAttribute);
    public static Error UnifyingAttributeRequired() => Error.InvalidArgument(ProductAttributeErrorCodes.UnifyingAttributeRequired);
    public static Error VariableAttributeRequired() => Error.InvalidArgument(ProductAttributeErrorCodes.VariableAttributeRequired);
    public static Error NotVariable() => Error.InvalidArgument(ProductAttributeErrorCodes.NotVariable);
    public static Error NotFound() => Error.NotFound(ProductAttributeErrorCodes.NotFound);
}

public static class ProductAttributeErrorCodes
{
    public const string CannotBeBothVariableAndUnifying = "ProductAttribute.CannotBeBothVariableAndUnifying";
    public const string VariableValuesOnNonVariableAttribute = "ProductAttribute.VariableValuesOnNonVariableAttribute";
    public const string UnifyingAttributeRequired = "ProductAttribute.UnifyingAttributeRequired";
    public const string VariableAttributeRequired = "ProductAttribute.VariableAttributeRequired";
    public const string CannotSpecifyBothValueAndVariations = "ProductAttribute.CannotSpecifyBothValueAndVariations";
    public const string ValueOrVariationRequired = "ProductAttribute.ValueOrVariationRequired";
    public const string NotVariable = "ProductAttribute.NotVariable";
    public const string NotFound = "ProductAttribute.NotFound";
}