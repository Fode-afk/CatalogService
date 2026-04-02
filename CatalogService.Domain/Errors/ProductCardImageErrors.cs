using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductCardImageErrors
{
    public static Error InvalidSortOrder() => Error.InvalidArgument(ProductCardImageErrorCodes.InvalidSortOrder);
}

public static class ProductCardImageErrorCodes
{
    public const string InvalidSortOrder = "ProductCardImage.InvalidSortOrder";
}