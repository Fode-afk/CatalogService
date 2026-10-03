using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class BlockReason : ValueObject
{
    public static int MaxLength => 5000;

    public string Value { get; }

    private BlockReason(string value)
    {
        Value = value;
    }

    public static IResult<BlockReason> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<BlockReason>(BlockReasonErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<BlockReason>(BlockReasonErrors.TooLong(MaxLength));

        return Ok(new BlockReason(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(BlockReason blockReason) => blockReason.ToString();
}
