using System;
using System.Collections.Generic;

namespace Kursovaya.Models.Common
{
    public sealed class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> items, int totalCount)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
        }

        public IReadOnlyList<T> Items { get; }
        public int TotalCount { get; }
    }
}