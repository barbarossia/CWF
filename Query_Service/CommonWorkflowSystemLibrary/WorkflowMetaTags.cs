using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    [DataContract]
    public class WorkflowMetaTag
    {
        public xxTaxonomy Key { get; set; }
        public xxContent Content { get; set; }

        public WorkflowMetaTag()
        {
            Key = new xxTaxonomy();
            Content = new xxContent();
        }

        public WorkflowMetaTag(string dbString)
        {
            string[] parts = dbString.Split(';');
            if (parts.Length == 2)
            {
                Guid tID;
                Guid cID;
                if (!(Guid.TryParse(parts[0], out tID)))
                {
                    //todo handle error

                }
                else
                {
                    
                }
                if (!(Guid.TryParse(parts[1], out cID)))
                {

                }

            }
            else
            {
                throw new ArgumentOutOfRangeException("Error spliting workflow tag, expected two guids");

            }
            
        }

        public string ToDbString()
        {
            return Key.ID.ToString() + "=" + Content.Id.ToString() + ";";
        }
    }
    [Serializable]
    [DataContract]
    public class MetaTags
    {



        public IList<WorkflowMetaTag> Tags { get; set; }

        IDictionary<Guid, Guid> intList { get; set; }

        public MetaTags()
        {
            Tags = new List<WorkflowMetaTag>();
            intList = new Dictionary<Guid, Guid>();

        }

        public MetaTags(string dbString)
        {

        }

        public override string ToString()
        {
            if (Tags == null)
                return "";
            string retString = "";
            foreach (var item in Tags)
            {
                retString = retString + item.ToDbString();
            }
            return retString;
        }

    }
}
