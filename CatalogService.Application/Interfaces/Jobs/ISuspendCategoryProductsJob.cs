namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendCategoryProductsJob
{
    Task Execute(Guid categoryId, CancellationToken cancellationToken = default);
}
