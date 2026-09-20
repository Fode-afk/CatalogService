using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CatalogService.UnitTests.Common;

public static class LoggerTestsHelper
{
    public static void VerifyLog<T>(
       this ILogger<T> logger,
       LogLevel level,
       EventId eventId,
       int times = 1)
    {
        var matchingCalls = logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Count(call =>
            {
                var args = call.GetArguments();
                var actualLevel = (LogLevel)args[0]!;
                var actualEventId = (EventId)args[1]!;
                return actualLevel == level && actualEventId.Id == eventId.Id;
            });

        matchingCalls.Should().Be(times,
            $"expected {times} log call(s) at {level} with EventId {eventId.Id}, but found {matchingCalls}");
    }
}
