using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace CWF.DataContracts
{
    public class GetActivitiesByActivityLibraryNameAndVersionReplyDC : ReplyHeader
    {
        private List<StoreActivitiesDC> list;

        public List<StoreActivitiesDC> List
        {
            set { list = value; }
            get { return list; }
        }

    }
}
