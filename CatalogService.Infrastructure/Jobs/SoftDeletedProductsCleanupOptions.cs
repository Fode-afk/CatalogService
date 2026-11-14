namespace CatalogService.Infrastructure.Jobs;

public sealed class SoftDeletedProductsCleanupOptions
{
    public const string SectionName = "SoftDeletedProductsCleanup";

    public string CronExpression { get; init; } = "0 */6 * * *";
    public TimeSpan RetentionPeriod { get; init; } = TimeSpan.FromDays(30);
    public int BatchSize { get; init; } = 500;
}
