using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductCardImageErrors
{
    public static Error InvalidSortOrder() => Error.InvalidArgument(ProductCardImageErrorCodes.InvalidSortOrder);
    public static Error MaxImagesReached() => Error.InvalidArgument(ProductCardImageErrorCodes.MaxImagesReached);
}

public static class ProductCardImageErrorCodes
{
    public const string InvalidSortOrder = "ProductCardImage.InvalidSortOrder";
    public const string MaxImagesReached = "ProductCardImage.MaxImagesReached";
}