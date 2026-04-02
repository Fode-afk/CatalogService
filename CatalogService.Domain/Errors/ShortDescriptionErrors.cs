using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ShortDescriptionErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(ShortDescriptionErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(ShortDescriptionErrorCodes.TooLong);
}

public static class ShortDescriptionErrorCodes
{
    public const string NullOrEmpty = "ShortDescription.NullOrEmpty";
    public const string TooLong = "ShortDescription.TooLong";
}