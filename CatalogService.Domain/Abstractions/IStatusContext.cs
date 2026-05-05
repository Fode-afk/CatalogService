using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Abstractions;

public interface IStatusContext
{
    ProductCardStatus ProductCardStatus { get; }
}
