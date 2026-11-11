using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Domain.Abstractions;

public interface IMarketsContext
{
    IReadOnlyCollection<Market> Markets { get; }
}