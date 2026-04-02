using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductCardErrors
{
    public static Error ImageNotFound() => Error.NotFound(ProductCardErrorCodes.ImageNotFound);
}

public static class ProductCardErrorCodes
{
    public const string ImageNotFound = "ProductCard.ImageNotFound";
}