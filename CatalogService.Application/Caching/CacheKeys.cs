namespace CatalogService.Application.Caching;

public static class CacheKeys
{
    public static string ProductById(Guid productId, string currency)
        => $"Products:{productId}:{currency}";

    public static string ExchangeRateByCurrency(string currency) => $"ExchangeRate:{currency}";
}
