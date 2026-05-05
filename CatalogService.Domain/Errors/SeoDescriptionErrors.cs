using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class SeoDescriptionErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(SeoDescriptionErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(SeoDescriptionErrorCodes.TooLong);
}

public static class SeoDescriptionErrorCodes
{
    public const string NullOrEmpty = "SeoDescription.NullOrEmpty";
    public const string TooLong = "SeoMetadata.TooLong";
}