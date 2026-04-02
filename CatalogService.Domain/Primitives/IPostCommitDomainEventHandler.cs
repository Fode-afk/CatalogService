using MediatR;

namespace CatalogService.Domain.Primitives;

public interface IPostCommitDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
