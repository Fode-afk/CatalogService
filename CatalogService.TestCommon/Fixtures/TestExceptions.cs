using CatalogService.Domain.Exceptions;

namespace CatalogService.TestCommon.Fixtures;

public sealed class ExpectedTestException() : Exception("expected"), IExpectedException;

public sealed class UnexpectedTestException() : Exception("unexpected");
