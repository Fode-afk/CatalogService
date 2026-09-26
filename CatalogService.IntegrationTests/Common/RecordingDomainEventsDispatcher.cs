using CatalogService.Domain.Primitives;
using CatalogService.Infrastructure.DomainEvents;

namespace CatalogService.IntegrationTests.Common;

public sealed class RecordingDomainEventsDispatcher : IDomainEventsDispatcher
{
    public List<IDomainEvent> PreCommitEvents { get; } = [];
    public List<IDomainEvent> PostCommitEvents { get; } = [];

    public Task DispatchPreCommitDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        PreCommitEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }

    public Task DispatchPostCommitDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        PostCommitEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }
}