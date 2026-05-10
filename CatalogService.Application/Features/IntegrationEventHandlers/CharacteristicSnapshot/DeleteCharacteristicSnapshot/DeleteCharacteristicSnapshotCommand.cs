using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;

public sealed record DeleteCharacteristicSnapshotCommand(Guid CharacteristicId) : IRequest<IResult>;