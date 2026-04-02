using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class ImageUrl : ValueObject
{
    public string Value { get; }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public static IResult<ImageUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<ImageUrl>(ImageUrlErrors.NullOrEmpty());

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
            || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            return Fail<ImageUrl>(ImageUrlErrors.InvalidFormat());

        return Ok(new ImageUrl(value.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.ToString();
}
