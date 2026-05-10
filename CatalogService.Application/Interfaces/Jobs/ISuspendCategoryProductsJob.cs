namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendCategoryProductsJob
{
    Task Execute(Guid categoryId, bool isActive, CancellationToken cancellationToken = default);
}
