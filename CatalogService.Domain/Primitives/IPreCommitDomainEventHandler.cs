using MediatR;

namespace CatalogService.Domain.Primitives;

public interface IPreCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
