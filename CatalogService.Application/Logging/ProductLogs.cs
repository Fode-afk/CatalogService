using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Logging;

public static partial class ProductLogs
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Could not suspend Product {ProductId}: {Reason}. Product may already be suspended.")]
    public static partial void ProductSuspendFailed(
        this ILogger logger, Guid productId, string reason);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Could not restore Product {ProductId}: {Reason}. Manual review required.")]
    public static partial void ProductRestoreFailed(
        this ILogger logger, Guid productId, string reason);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "Invalid snapshot data for Characteristic {CharacteristicId}: {Field} = '{Value}'")]
    public static partial void InvalidSnapshotData(
        this ILogger logger, Guid characteristicId, string field, string value);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Warning,
        Message = "Could not add variant {VariantId} to product {ProductId} attributes: {Reason}. " +
                  "Product may not be in a modifiable state.")]
    public static partial void AddVariantToAttributesFailed(
        this ILogger logger, Guid variantId, Guid productId, string reason);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Warning,
        Message = "Could not remove variant {VariantId} from product {ProductId} attributes: {Reason}. " +
                  "Product may not be in a modifiable state. Proceeding with snapshot removal.")]
    public static partial void RemoveVariantFromAttributesFailed(
        this ILogger logger, Guid variantId, Guid productId, string reason);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Information,
        Message = "Ignored stale decision {SubmissionId} for product {ProductId}")]
    public static partial void IgnoredStaleDecision(
        this ILogger logger, Guid submissionId, Guid productId);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Warning,
        Message = "Failed to create block reason for product {ProductId}")]
    public static partial void FailedToCreateBlockReason(
        this ILogger logger, Guid productId);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Warning,
        Message = "Failed to block product {ProductId}: {Error}")]
    public static partial void FailedToBlockProduct(
        this ILogger logger, Guid productId, string error);

    [LoggerMessage(
        EventId = 1009,
        Level = LogLevel.Warning,
        Message = "Failed to create rejection reason for product {ProductId}")]
    public static partial void FailedToCreateRejectionReason(
        this ILogger logger, Guid productId);

    [LoggerMessage(
        EventId = 1010,
        Level = LogLevel.Warning,
        Message = "Failed to reject product publish {SubmissionId} for product {ProductId}: {Error}")]
    public static partial void FailedToRejectProductPublish(
        this ILogger logger, Guid submissionId, Guid productId, string error);

    [LoggerMessage(
        EventId = 1011,
        Level = LogLevel.Warning,
        Message = "Failed to unblock product with ID {ProductId}. Reason: {Reason}")]
    public static partial void FailedToUnblockProduct(
        this ILogger logger, Guid productId, string reason);

    [LoggerMessage(
        EventId = 1012,
        Level = LogLevel.Warning,
        Message = "Failed to approve product publish {SubmissionId} for product {ProductId}: {Error}")]
    public static partial void FailedToApproveProductPublish(
        this ILogger logger, Guid submissionId, Guid productId, string error);
}
