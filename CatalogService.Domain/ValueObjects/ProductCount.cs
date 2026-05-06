using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class ProductCount : ValueObject
{
    public static int MinValue => 0;
    public static int MaxValue => 20;

    public static ProductCount Zero => new(0);

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
