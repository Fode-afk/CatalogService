using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;

namespace CatalogService.Application.Interfaces.Services;

public interface IMoneyConverter
{
    Task<IResult<Money>> ConvertAsync(
        Money money, 
        Currency targetCurrency, 
        CancellationToken cancellationToken = default);
}
