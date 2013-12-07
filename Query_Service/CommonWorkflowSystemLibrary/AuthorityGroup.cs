using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow
{
    public class AuthorityGroup
    {
        public string Name { get; set; }


        public static IList<AuthorityGroup> GetList()
        {
            IList<AuthorityGroup> aList = new List<AuthorityGroup>();

            //todo replace with lookup
            //// call to iasset store authority groups?
            //// tie into windows security?

            return aList;
        }

    }
}
