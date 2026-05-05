using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class SeoTitleErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(SeoTitleErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(SeoTitleErrorCodes.TooLong);
}

public static class SeoTitleErrorCodes
{
    public const string NullOrEmpty = "SeoTitle.NullOrEmpty";
    public const string TooLong = "SeoTitle.TooLong";
}