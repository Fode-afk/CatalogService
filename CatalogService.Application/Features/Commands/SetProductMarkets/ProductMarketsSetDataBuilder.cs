using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.SetProductMarkets;

internal static class ProductMarketsSetDataBuilder
{
    public static IResult<IReadOnlyCollection<Market>> Build(SetProductMarketsCommand request)
    {
        var errors = new List<Error>();

        var marketResults = request.Markets
            .Select(Market.Create)
            .ToList();

        errors.AddRange(marketResults
            .Where(r => r.IsFailure)
            .Select(r => r.Error));

        if (errors.Count > 0)
            return Fail<IReadOnlyCollection<Market>>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(marketResults.Select(a => a.Value).ToList());
    }
}
