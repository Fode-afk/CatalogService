using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductSetMarketsSpecification
{
    public static readonly ISpecification<ProductSetMarketsContext> Spec =
        new VendorIsActiveSpec<ProductSetMarketsContext>()
            .And(Specification<ProductSetMarketsContext>.Create(
                ctx => ctx.CanEditOperationalData,
                ProductErrors.CannotEditOperationalData()))
            .And(new MarketsRequiredSpec<ProductSetMarketsContext>())
            .And(Specification<ProductSetMarketsContext>.Create(
                ctx => ctx.Markets.Select(m => m.Code).Distinct().Count() == ctx.Markets.Count,
                ProductErrors.DuplicateMarket()))
            .And(Specification<ProductSetMarketsContext>.Create(
                ctx => ctx.Markets.All(ctx.VendorAllowedMarkets.Contains),
                ProductErrors.MarketNotAllowedForVendor()));
}
