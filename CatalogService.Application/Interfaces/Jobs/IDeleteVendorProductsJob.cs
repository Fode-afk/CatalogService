namespace CatalogService.Application.Interfaces.Jobs;

public interface IDeleteVendorProductsJob
{
    Task Execute(Guid vendorId, CancellationToken cancellationToken);
}
