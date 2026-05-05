using CatalogService.Application.Interfaces.Data;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Messaging.IntegrationEvents.Discounts;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ValidateProductCardForDiscount;

public sealed class ValidateProductCardForDiscountCommandHandler(
    IAppDbContext context,
    IPublishEndpoint publish) : IRequestHandler<ValidateProductCardForDiscountCommand, IResult>
{
    public async Task<IResult> Handle(ValidateProductCardForDiscountCommand request, CancellationToken cancellationToken)
    {
        var productCardExists = await context.ProductCards
            .AnyAsync(p =>
                p.Id == request.ProductCardId &&
                p.VendorId == request.VendorId,
                cancellationToken);

        if (productCardExists)
            await publish.Publish(new ProductCardValidated(request.CorrelationId), cancellationToken);
        else
            await publish.Publish(new DiscountProcessFailed(
                request.CorrelationId,
                "ProductCard not found or does not belong to vendor"), cancellationToken);

        return Ok();
    }
}