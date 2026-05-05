using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class ProductCount : ValueObject
{
    public const int MinValue = 0;
    public const int MaxValue = 20;

    public static ProductCount Zero => Create(0).Value;

    public int Value { get; }
    public bool IsEmpty => Value == 0;

    private ProductCount(int value)
    {
        Value = value;
    }

    public static IResult<ProductCount> Create(int value)
    {
        if (value < MinValue)
            return Fail<ProductCount>(ProductCountErrors.InvalidValue());

        if (value > MaxValue)
            return Fail<ProductCount>(ProductCountErrors.LimitReached());

        return Ok(new ProductCount(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator int(ProductCount count) => count.Value;
}
