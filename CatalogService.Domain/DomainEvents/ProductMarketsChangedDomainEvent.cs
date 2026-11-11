using migApp.Shared.Domain.Primitives;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductMarketsChangedDomainEvent(
    Guid ProductId, 
    IReadOnlyList<Market> Markets,
    IReadOnlyList<Market> AddedMarkets,
    IReadOnlyList<Market> RemovedMarkets,
    long Version) : IDomainEvent;