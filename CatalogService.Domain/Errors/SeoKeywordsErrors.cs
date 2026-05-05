using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class SeoKeywordsErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(SeoKeywordsErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(SeoKeywordsErrorCodes.TooLong);
}

public static class SeoKeywordsErrorCodes
{
    public const string NullOrEmpty = "SeoKeywords.NullOrEmpty";
    public const string TooLong = "SeoKeywords.TooLong";
}