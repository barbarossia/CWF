using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow
{
    public class Activity_Category
    {

        public Activity_Category()
        {
            Id = Guid.Empty;
        }

        /// <summary>
        /// Added during the conversion from the EDm to ER model DB
        /// </summary>
        public Guid NEWActivityCategoryId { get; set; }
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Old EDm model only
        /// </summary>
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public AuthorityGroup AuthGroup { get; set; }
        public MetaTags Metatags { get; set; }

        //
        //
        /// <summary>
        /// Returns the Unassigned category group with 
        /// a category id set to Guid.Empty
        /// </summary>
        /// <returns></returns>
        public static Activity_Category Unassigned()
        {
            Activity_Category retVal = new Activity_Category()
            {
                Id = Guid.Empty,
                Name = "Unassigned",
                Description = "Defaulted to Unassigned Category Code",
                Metatags = new MetaTags(),
                AuthGroup = new AuthorityGroup()
            };

            return retVal;
        }

    }
}
