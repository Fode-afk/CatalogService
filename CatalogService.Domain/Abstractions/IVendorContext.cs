namespace CatalogService.Domain.Abstractions;

public interface IVendorContext
{
    bool VendorIsActive { get; }
}
