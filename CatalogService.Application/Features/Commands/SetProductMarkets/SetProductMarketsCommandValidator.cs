using CatalogService.Domain.Errors;
using FluentValidation;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Application.Features.Commands.SetProductMarkets;

public sealed class SetProductMarketsCommandValidator : AbstractValidator<SetProductMarketsCommand>
{
    public SetProductMarketsCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
           .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.Markets)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductErrorCodes.MarketsRequired)
            .Must(x => x.Distinct().Count() == x.Count).WithErrorCode(ProductErrorCodes.DuplicateMarket)
            .Must(x => x.All(m => m.Length == Market.CodeLength)).WithErrorCode(MarketErrorCodes.InvalidFormat);
    }
}
