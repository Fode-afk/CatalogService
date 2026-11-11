namespace CatalogService.Application.Interfaces.Jobs;

public interface IRestrictVendorProductsMarketsJob
{
    Task Execute(Guid vendorId, CancellationToken cancellationToken = default);
}
