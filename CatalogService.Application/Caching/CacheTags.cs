namespace CatalogService.Application.Caching;

public static class CacheTags
{
    public static string ProductCardById(Guid productCardId)
       => $"ProductCards:{productCardId}";
}