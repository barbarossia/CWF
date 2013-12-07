using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow
{
    public interface ITaxonomy
    {
        /// <summary>
        /// Added during conversion from EDm to Er model DB
        /// </summary>
        Guid NEWTaxonomyID { get; set; }
        Int32 NEWId { get; set; }

        /// <summary>
        /// only used in EDm 
        /// </summary>
        Guid ID { get; set; }

        String Description { get;  }

        bool HasValueList { get;  }



        //IList<ITaxonomy> GetValues();

        //void Save();

        //void Delete();


    }

    public enum CategoryTypes
    {
        World,
        Company,
        BusinessArea,
        Application,
        Workflow,
        Temporary

    }

    public enum PrivacyTypes
    {
        Low,
        Medium,
        High,
    }


    public class xxTaxonomy : ITaxonomy
    {
        /// <summary>
        /// New
        /// </summary>
        public Int32 NEWId { get; set; }
        public Guid NEWTaxonomyID {get; set;}

        /// <summary>
        /// Old
        /// </summary>
        public Guid ID { get; set; }

        public String Name { get; set; }

        public string Description { get; set; }

        public bool HasValueList { get; set; }

        //public CategoryTypes CategoryType { get; set; }

        public string CategoryKey { get; set; }

        public string ValueType { get; set; }

        public string RegularExpression { get; set; }


        public int Volaltility { get; set; }

        public PrivacyTypes Privacy { get; set; }
        
    }
}
