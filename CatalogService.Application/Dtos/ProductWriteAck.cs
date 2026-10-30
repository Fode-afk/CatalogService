namespace CatalogService.Application.Dtos;

public sealed record ProductWriteAck(Guid ProductId, long Version);