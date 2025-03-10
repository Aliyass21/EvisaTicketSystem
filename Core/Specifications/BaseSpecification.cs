using System.Linq.Expressions;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Specifcation;

public abstract class BaseSpecification<T> : ISpecification<T> where T : Entity
{
    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; private set; } = null!;
    public Expression<Func<T, object>> OrderByDescending { get; private set; }= null!;
    public Expression<Func<T, object>>? SecondaryOrderBy { get; private set; }
    public Expression<Func<T, object>>? SecondaryOrderByDescending { get; private set; }

    public bool IsSecondaryDescending { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; } = false;

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
    public void WithoutPagination()
    {
        ApplyPaging(0, int.MaxValue); // Removes pagination by setting a very large page size
    }

    protected BaseSpecification() 
    {
        Criteria = x => true; // Default criteria to match all
    }
        // Apply the secondary ordering
    public void ApplySecondaryOrderBy(Expression<Func<T, object>> secondaryOrderByExpression, bool isDescending = true)
    {
        SecondaryOrderBy = secondaryOrderByExpression;
        IsSecondaryDescending = isDescending;
    }
       // Apply secondary ordering in descending order
    public void ApplySecondaryOrderByDescending(Expression<Func<T, object>> secondaryOrderByDescendingExpression)
    {
        SecondaryOrderByDescending = secondaryOrderByDescendingExpression;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending)
    {
        OrderByDescending = orderByDescending;
    }
}

