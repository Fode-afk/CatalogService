using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Data.Converters;

internal sealed class MarketConverter() : ValueConverter<Market, string>(
    m => m.Code,
    s => Market.Create(s).Value);
