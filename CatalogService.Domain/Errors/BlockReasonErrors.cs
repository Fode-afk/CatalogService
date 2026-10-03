using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class BlockReasonErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(BlockReasonErrorCodes.NullOrEmpty,
            "Block reason cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(BlockReasonErrorCodes.TooLong,
            $"Block reason must not exceed {maxLength} characters.");
}

public static class BlockReasonErrorCodes
{
    public const string NullOrEmpty = "BlockReason.NullOrEmpty";
    public const string TooLong = "BlockReason.TooLong";
}
