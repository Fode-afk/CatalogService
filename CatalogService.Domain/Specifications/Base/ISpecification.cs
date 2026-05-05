using migApp.Shared.Results;

namespace CatalogService.Domain.Specifications.Base;

public interface ISpecification<T>
{
    IResult IsSatisfiedBy(T candidate);
}