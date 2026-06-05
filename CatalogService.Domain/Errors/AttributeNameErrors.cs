using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class AttributeNameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AttributeNameErrorCodes.NullOrEmpty,
            "Attribute name must not be empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AttributeNameErrorCodes.TooLong,
            $"Attribute name must not exceed {maxLength} characters.");
}

public static class AttributeNameErrorCodes
{
    public const string NullOrEmpty = "AttributeName.NullOrEmpty";
    public const string TooLong = "AttributeName.TooLong";
}