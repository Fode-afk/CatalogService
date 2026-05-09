namespace CatalogService.Application.Caching;

public static class CacheTags
{
    public static string ProductById(Guid productId)
       => $"Products:{productId}";
}