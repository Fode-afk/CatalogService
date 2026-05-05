namespace CatalogService.Application.Caching;

public static class CacheKeys
{
    public static string ProductCardById(Guid productCardId, string currency)
        => $"ProductCards:{productCardId}:{currency}";

    public static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";
}
