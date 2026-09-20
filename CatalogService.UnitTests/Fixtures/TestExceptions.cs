using CatalogService.Domain.Exceptions;

namespace CatalogService.UnitTests.Fixtures;

public sealed class ExpectedTestException() : Exception("expected"), IExpectedException;

public sealed class UnexpectedTestException() : Exception("unexpected");
