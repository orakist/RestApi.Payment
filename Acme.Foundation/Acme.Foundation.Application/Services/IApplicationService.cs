using System.Linq.Expressions;

namespace Acme.Foundation.Application.Services;

/// <summary>
/// This interface must be implemented by all application services to register and identify them by convention.
/// </summary>
public interface IApplicationService 
{

}

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }
}
