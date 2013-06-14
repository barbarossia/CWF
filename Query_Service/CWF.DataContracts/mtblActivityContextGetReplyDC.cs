using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;


namespace CWF.DataContracts
{
    [DataContract]
    public class mtblActivityContextGetReplyDC : ReplyHeader
    {
        IList<mtblActivityContextGetBase> list;

        [DataMember]
        public  IList<mtblActivityContextGetBase> List
        {
            get { return list; }
            set { list = value; }
        }

    }
}
