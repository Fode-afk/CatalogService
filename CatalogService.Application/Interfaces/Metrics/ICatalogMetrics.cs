namespace CatalogService.Application.Interfaces.Metrics;

public interface ICatalogMetrics
{
    void RecordProductCreated();
    void RecordProductSuspended(string reason);
    void RecordProductRestored();
    void RecordProductDeleted();
    void RecordProductNotFound();
    void RecordSnapshotNotFound(string snapshotType);
    void RecordSnapshotOutdated(string handlerName);
    void SetActiveProductsCount(int count);
    void RecordHandlerError(string handlerName, string handlerType);
    void RecordHandlerDuration(double ms, string handlerName, string handlerType);
}