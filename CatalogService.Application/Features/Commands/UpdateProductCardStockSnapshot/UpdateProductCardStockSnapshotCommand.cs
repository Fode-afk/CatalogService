using MediatR;
using migApp.Shared.Enums.Inventory;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductCardStockSnapshot;

public sealed record UpdateProductCardStockSnapshotCommand(
    Guid ProductCardId, 
    Guid VendorId,
    StockStatus Status) : IRequest<IResult>;