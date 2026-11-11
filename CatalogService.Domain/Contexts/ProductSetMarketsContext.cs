using CatalogService.Domain.Abstractions;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductSetMarketsContext(
    bool VendorIsActive, 
    bool CanEditOperationalData,
    IReadOnlyCollection<Market> Markets,
    IReadOnlyCollection<Market> VendorAllowedMarkets) : IVendorContext, IMarketsContext;