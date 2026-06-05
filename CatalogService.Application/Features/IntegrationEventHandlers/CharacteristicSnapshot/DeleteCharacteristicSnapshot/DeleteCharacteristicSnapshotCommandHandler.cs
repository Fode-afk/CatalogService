using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;

public sealed class DeleteCharacteristicSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteCharacteristicSnapshotCommand>
{
    public async Task Handle(DeleteCharacteristicSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.CharacteristicSnapshots
            .FirstOrDefaultAsync(c => c.CharacteristicId == request.CharacteristicId, cancellationToken);
        if (snapshot is null)
            return;

        context.CharacteristicSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
