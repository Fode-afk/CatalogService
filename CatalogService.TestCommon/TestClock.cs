using Microsoft.Extensions.Time.Testing;

namespace CatalogService.TestCommon;

public static class TestClock
{
    public static readonly DateTimeOffset DefaultNow = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static FakeTimeProvider Create(DateTimeOffset? now = null) =>
        new(now ?? new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
}