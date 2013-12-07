using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Common
{
    public class PagedList<T> : List<T>
    {
        public int StartIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int PageCount
        {
            get
            {
                if ((TotalCount % PageSize) > 0)
                    return (TotalCount / PageSize) + 1;
                else
                    return TotalCount / PageSize;
            }
        }

        public PagedList(IEnumerable<T> pagedList, int startIndex, int pageSize, int totalCount)
            : base(pagedList)
        {
            StartIndex = startIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

    }
}
