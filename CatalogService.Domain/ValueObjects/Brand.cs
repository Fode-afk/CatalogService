using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed partial class Brand : ValueObject
{
    public static int MaxLength => 100;

    public string Value { get; }

    private Brand(string value)
    {
        Value = value;
    }

    public static IResult<Brand> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Brand>(BrandErrors.NullOrEmpty());

        value = Normalize(value);

        if (value.Length > MaxLength)
            return Fail<Brand>(BrandErrors.TooLong());

        return Ok(new Brand(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Brand brand) => brand.ToString();

    private static string Normalize(string value)
    {
        value = value.Trim();

        value = NormalizeRegex().Replace(value, " ");

        return value;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeRegex();
}
