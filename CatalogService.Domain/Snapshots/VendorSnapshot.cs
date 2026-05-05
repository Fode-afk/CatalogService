using migApp.Shared.Enums.Vendors;

namespace CatalogService.Domain.Snapshots;

public sealed class VendorSnapshot
{
    public Guid VendorId { get; set; }
    public VendorStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public long Version { get; set; }

    public bool IsActive =>
        (Status == VendorStatus.Active ||
        Status == VendorStatus.OnVacation) &&
        IsVerified;
}
