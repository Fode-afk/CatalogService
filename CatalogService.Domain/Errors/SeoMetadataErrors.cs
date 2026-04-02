using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class SeoMetadataErrors
{
    public static Error TitleNullOrEmpty() => Error.InvalidArgument(SeoMetadataErrorCodes.TitleNullOrEmpty);
    public static Error DescriptionNullOrEmpty() => Error.InvalidArgument(SeoMetadataErrorCodes.DescriptionNullOrEmpty);
    public static Error KeywordsNullOrEmpty() => Error.InvalidArgument(SeoMetadataErrorCodes.KeywordsNullOrEmpty);
    public static Error TitleTooLong() => Error.InvalidArgument(SeoMetadataErrorCodes.TitleTooLong);
    public static Error DescriptionTooLong() => Error.InvalidArgument(SeoMetadataErrorCodes.DescriptionTooLong);
    public static Error KeywordsTooLong() => Error.InvalidArgument(SeoMetadataErrorCodes.KeywordsTooLong);
}

public static class SeoMetadataErrorCodes
{
    public const string TitleNullOrEmpty = "SeoMetadata.TitleNullOrEmpty";
    public const string DescriptionNullOrEmpty = "SeoMetadata.DescriptionNullOrEmpty";
    public const string KeywordsNullOrEmpty = "SeoMetadata.KeywordsNullOrEmpty";
    public const string TitleTooLong = "SeoMetadata.TitleTooLong";
    public const string DescriptionTooLong = "SeoMetadata.DescriptionTooLong";
    public const string KeywordsTooLong = "SeoMetadata.KeywordsTooLong";
}