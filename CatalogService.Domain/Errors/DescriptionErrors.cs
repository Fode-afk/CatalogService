using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class DescriptionErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(DescriptionErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(DescriptionErrorCodes.TooLong);
}

public static class DescriptionErrorCodes
{
    public const string NullOrEmpty = "Description.NullOrEmpty";
    public const string TooLong = "Description.TooLong";
}