using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductWithdrawPublishSubmissionSpecification
{
    public static readonly ISpecification<ProductWithdrawPublishSubmissionContext> Spec =
        new VendorIsActiveSpec<ProductWithdrawPublishSubmissionContext>();
}
