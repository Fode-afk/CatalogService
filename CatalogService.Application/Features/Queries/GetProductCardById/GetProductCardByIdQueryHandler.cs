using CatalogService.Application.Caching;
using CatalogService.Application.Dtos;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Services;
using CatalogService.Domain.Errors;
using MediatR;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Queries.GetProductCardById;

public sealed class GetProductCardByIdQueryHandler(
    IProductReadRepository productCardReadRepository,
    IMoneyConverter moneyConverter,
    IFusionCache cache) : IRequestHandler<GetProductCardByIdQuery, IResult<ProductDto>>
{
    public async Task<IResult<ProductDto>> Handle(GetProductCardByIdQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<ProductDto>(currencyResult.Error);

        var currency = currencyResult.Value;

        var productCardDto = await cache.GetOrSetAsync<ProductDto?>(
            CacheKeys.ProductById(request.ProductCardId, currency.Code),
            async (entry, ct) => await GetProductCardDto(
                request.ProductCardId,
                currency,
                ct),
            tags: [CacheTags.ProductById(request.ProductCardId)],
            token: cancellationToken);

        return productCardDto != null ?
            Ok(productCardDto) :
            Fail<ProductDto>(ProductErrors.NotFound());
    }

    public async Task<ProductDto?> GetProductCardDto(
        Guid productCardId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        var productCard = await productCardReadRepository.GetByIdAsync(productCardId, cancellationToken);

        if (productCard == null)
            return null;

        long priceAmountMinor = 0;
        long oldPriceAmountMinor = 0;

        if (productCard.PriceAmount is not null)
        {
            var convertedPriceResult = await ConvertUsdToTargetMinorAsync(
                productCard.PriceAmount.Value,
                currency,
                cancellationToken);

            if (convertedPriceResult.IsFailure)
                return null;

            priceAmountMinor = convertedPriceResult.Value;

            if (productCard.OldPriceAmount is not null)
            {
                var convertedOldPriceResult = await ConvertUsdToTargetMinorAsync(
                    productCard.OldPriceAmount.Value,
                    currency,
                    cancellationToken);

                if (convertedOldPriceResult.IsFailure)
                    return null;

                oldPriceAmountMinor = convertedOldPriceResult.Value;
            }
        }

        return new ProductDto(
            productCard.Id.ToString(),
            productCard.Name,
            productCard.Slug,
            productCard.Description,
            productCard.ShortDescription,
            productCard.RatingAvg,
            productCard.RatingCount,
            productCard.DefaultProductId.ToString(),
            productCard.ProductCount,
            priceAmountMinor,
            oldPriceAmountMinor,
            productCard.StockStatus,
            productCard.CategoryId.ToString(),
            productCard.CategoryName,
            productCard.CategorySlug,
            productCard.VendorId.ToString(),
            productCard.BrandName,
            productCard.ProductStatus,
            new SeoMetadataDto (
                productCard.SeoTitle,
                productCard.SeoDescription,
                productCard.SeoKeywords),
            JsonSerializer.Deserialize<Dictionary<string, string>>(productCard.AttributesJson)!,
            JsonSerializer.Deserialize<List<string>>(productCard.TagsJson)!);
    }

    private async Task<IResult<long>> ConvertUsdToTargetMinorAsync(
        decimal usdAmount,
        Currency targetCurrency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var usdMoneyResult = Money.Create(usdAmount, Currency.USD);
        if (usdMoneyResult.IsFailure)
            return Fail<long>(usdMoneyResult.Error);

        var money = usdMoneyResult.Value;

        if (targetCurrency != Currency.USD)
        {
            var converted = await moneyConverter.ConvertAsync(
                money,
                targetCurrency,
                cancellationToken);

            if (converted.IsFailure)
                return Fail<long>(converted.Error);

            money = converted.Value;
        }

        var minorResult = Money.ToMinor(money.Amount, targetCurrency);

        return minorResult.IsFailure
            ? Fail<long>(minorResult.Error)
            : Ok(minorResult.Value);
    }
}
