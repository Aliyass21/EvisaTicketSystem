using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace EVisaTicketSystem.Specifcation;

public class SpecificationEvaluator<T> where T : Entity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        if (inputQuery == null)
            throw new ArgumentNullException(nameof(inputQuery));

        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        var query = inputQuery;

        // Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes with null checks
        if (specification.Includes != null)
        {
            query = specification.Includes.Aggregate(query, 
                (current, include) => current.Include(include));
        }

        // Apply string-based includes with null checks
        if (specification.IncludeStrings != null)
        {
            query = specification.IncludeStrings.Aggregate(query, 
                (current, include) => current.Include(include));
        }

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}