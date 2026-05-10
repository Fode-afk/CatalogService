namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendVendorProductsJob
{
    Task Execute(Guid vendorId, bool isActive, CancellationToken cancellationToken = default);
}
