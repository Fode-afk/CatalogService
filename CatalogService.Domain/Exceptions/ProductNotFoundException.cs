namespace CatalogService.Domain.Exceptions;

public sealed class ProductNotFoundException(Guid productId)
    : Exception($"Product not found: {productId}"), IExpectedException;