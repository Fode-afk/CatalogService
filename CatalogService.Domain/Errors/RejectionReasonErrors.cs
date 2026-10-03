using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class RejectionReasonErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(RejectionReasonErrorCodes.NullOrEmpty,
            "Rejection reason cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(RejectionReasonErrorCodes.TooLong,
            $"Rejection reason must not exceed {maxLength} characters.");
}

public static class RejectionReasonErrorCodes
{
    public const string NullOrEmpty = "RejectionReason.NullOrEmpty";
    public const string TooLong = "RejectionReason.TooLong";
}