using CatalogService.Application.Interfaces.Services;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ExchangeRates;

namespace CatalogService.Infrastructure.Messaging.Consumers;

public sealed class CurrenciesUpdatedIntegrationEventConsumer(IExchangeRateService exchangeRateService) : IConsumer<CurrenciesUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CurrenciesUpdatedIntegrationEvent> context) => 
        await exchangeRateService.UpdateExcahngeRatesAsync(context.Message.Rates, context.CancellationToken);
}
