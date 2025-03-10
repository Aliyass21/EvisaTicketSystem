using System.Linq.Expressions;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Specifcation;

public interface ISpecification<T> where T : Entity
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    Expression<Func<T, object>>? SecondaryOrderBy { get; }
    Expression<Func<T, object>>? SecondaryOrderByDescending { get; }
    bool IsSecondaryDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}