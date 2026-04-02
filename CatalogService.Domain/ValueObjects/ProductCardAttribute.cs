using CatalogService.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class ProductCardAttribute : ValueObject
{
    public AttributeName Name { get; }
    public AttributeValue Value { get; }

    private ProductCardAttribute(AttributeName name, AttributeValue value)
    {
        Name = name;
        Value = value;
    }

    public static IResult<ProductCardAttribute> Create(string name, string value)
    {
        var nameResult = AttributeName.Create(name);

        if (nameResult.IsFailure)
            return Fail<ProductCardAttribute>(nameResult.Error);

        var valueResult = AttributeValue.Create(value);

        if (valueResult.IsFailure)
            return Fail<ProductCardAttribute>(valueResult.Error);

        return Ok(new ProductCardAttribute(nameResult.Value, valueResult.Value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Value;
    }
}
