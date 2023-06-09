﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Smartstore.Web.Modelling.DataGrid
{
    public static class GridCommandQueryExtensions
    {
        /// <summary>
        /// Applies a bound <see cref="GridCommand"/> specification to a query.
        /// </summary>
        /// <param name="query">The query to apply the command to.</param>
        /// <param name="command">The source command.</param>
        /// <param name="applyPaging">Whether to apply the paging part also.</param>
        public static IQueryable<T> ApplyGridCommand<T>(this IQueryable<T> query, GridCommand command, bool applyPaging = false)
        {
            Guard.NotNull(query, nameof(query));
            Guard.NotNull(command, nameof(command));

            IOrderedQueryable<T> orderedQuery = null;

            foreach (var sort in command.Sorting)
            {
                var expr = sort.Member;
                if (sort.Descending) expr += " descending";

                if (orderedQuery == null)
                {
                    orderedQuery = query.OrderBy(expr);
                }
                else
                {
                    orderedQuery = orderedQuery.ThenBy(expr);
                }

                query = orderedQuery;
            }

            if (applyPaging)
            {
                query = query.ApplyPaging(command.Page - 1, command.PageSize);
            }

            return query;
        }
    }
}
