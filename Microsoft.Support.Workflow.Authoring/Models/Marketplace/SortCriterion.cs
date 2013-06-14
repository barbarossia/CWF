using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    /// <summary>
    /// Known type used to specify a sort criterion
    /// </summary>
    public class SortCriterion
    {
        /// <summary>
        /// 
        /// </summary>
        public string FieldName { get; set; }
        public bool IsAscending { get; set; }

        public SortCriterion() { }
        public SortCriterion(string fieldName, bool isAscending) 
        {
            this.FieldName = fieldName;
            this.IsAscending = isAscending;
        }
    }
}
