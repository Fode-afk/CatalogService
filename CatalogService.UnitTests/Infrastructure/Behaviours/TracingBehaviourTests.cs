using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Infrastructure.Behaviours;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace CatalogService.UnitTests.Infrastructure.Behaviours;

public class TracingBehaviourTests
{
    private sealed record SomeQuery : IRequest<string>;
    private sealed record SomeCommand : IRequest<string>;
    private sealed record SomeSnapshotCommand : IRequest<string>;
    private sealed record SomeProjectionCommand : IRequest<string>;

    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Return_Response_When_Handler_Succeeds()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        // Act
        var result = await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Should_Record_Duration_With_Command_Prefix_For_Regular_Request()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        // Act
        await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordHandlerDuration(
            Arg.Any<double>(), nameof(SomeCommand), "command");
    }

    [Fact]
    public async Task Should_Record_Duration_With_Query_Prefix_For_Query_Request()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeQuery, string>(_metrics);
        var request = new SomeQuery();
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        // Act
        await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordHandlerDuration(
            Arg.Any<double>(), nameof(SomeQuery), "query");
    }

    [Fact]
    public async Task Should_Record_Duration_With_Projection_Prefix_For_Snapshot_Request()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeSnapshotCommand, string>(_metrics);
        var request = new SomeSnapshotCommand();
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        // Act
        await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordHandlerDuration(
            Arg.Any<double>(), nameof(SomeSnapshotCommand), "projection");
    }

    [Fact]
    public async Task Should_Record_Duration_With_Projection_Prefix_For_Projection_Request()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeProjectionCommand, string>(_metrics);
        var request = new SomeProjectionCommand();
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        // Act
        await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordHandlerDuration(
            Arg.Any<double>(), nameof(SomeProjectionCommand), "projection");
    }

    [Fact]
    public async Task Should_Rethrow_Exception_From_Handler()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => throw new UnexpectedTestException();

        // Act
        var act = () => behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnexpectedTestException>();
    }

    [Fact]
    public async Task Should_Record_Handler_Error_When_Exception_Is_Unexpected()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => throw new UnexpectedTestException();

        // Act
        var act = () => behaviour.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<UnexpectedTestException>();

        // Assert
        _metrics.Received(1).RecordHandlerError(nameof(SomeCommand), "command");
    }

    [Fact]
    public async Task Should_Not_Record_Handler_Error_When_Exception_Is_Expected()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => throw new ExpectedTestException();

        // Act
        var act = () => behaviour.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<ExpectedTestException>();

        // Assert
        _metrics.DidNotReceive().RecordHandlerError(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Still_Record_Duration_When_Handler_Throws()
    {
        // Arrange
        var behaviour = new TracingBehaviour<SomeCommand, string>(_metrics);
        var request = new SomeCommand();
        RequestHandlerDelegate<string> next = _ => throw new UnexpectedTestException();

        // Act
        var act = () => behaviour.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<UnexpectedTestException>();

        // Assert
        _metrics.Received(1).RecordHandlerDuration(
            Arg.Any<double>(), nameof(SomeCommand), "command");
    }
}