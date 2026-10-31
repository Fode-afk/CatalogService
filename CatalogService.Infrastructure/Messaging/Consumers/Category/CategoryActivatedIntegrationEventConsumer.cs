using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CatalogService.Infrastructure.Messaging.Consumers.Category;

public sealed class CategoryActivatedIntegrationEventConsumer(ISender sender) : IConsumer<CategoryActivatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CategoryActivatedIntegrationEvent> context) =>
        await sender.Send(new UpdateCategorySnapshotCommand(
            context.Message.CategoryId,
            context.Message.IsActive,
            context.Message.Version), context.CancellationToken);
}
