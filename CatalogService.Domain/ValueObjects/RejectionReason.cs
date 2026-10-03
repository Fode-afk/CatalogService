using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class RejectionReason : ValueObject
{
    public static int MaxLength => 5000;

    public string Value { get; }

    private RejectionReason(string value)
    {
        Value = value;
    }

    public static IResult<RejectionReason> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<RejectionReason>(RejectionReasonErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<RejectionReason>(RejectionReasonErrors.TooLong(MaxLength));

        return Ok(new RejectionReason(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(RejectionReason rejectionReason) => rejectionReason.ToString();
}
