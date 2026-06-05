using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class AttributeGroupNameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AttributeGroupNameErrorCodes.NullOrEmpty,
            "Attribute group name must not be empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AttributeGroupNameErrorCodes.TooLong,
            $"Attribute group name must not exceed {maxLength} characters.");
}

public static class AttributeGroupNameErrorCodes
{
    public const string NullOrEmpty = "AttributeGroupName.NullOrEmpty";
    public const string TooLong = "AttributeGroupName.TooLong";
}