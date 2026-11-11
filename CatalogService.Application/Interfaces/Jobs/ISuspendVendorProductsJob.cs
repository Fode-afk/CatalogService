namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendVendorProductsJob
{
    Task Execute(Guid vendorId, CancellationToken cancellationToken = default);
}
