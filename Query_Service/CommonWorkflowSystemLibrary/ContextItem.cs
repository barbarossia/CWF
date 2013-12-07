using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    public class xxContextCategory
    {
        /// <summary>
        /// Added during conversion from EDm to ER model DB
        /// </summary>
        public Guid NEWContextCategoriesId { get; set; }
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Old EDm model
        /// </summary>
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaTags { get; set; }
        //public string AuthGroup { get; set; }
        public TimeSpan PersistencePeriod { get; set; }


    }

    [DataContract]
    public enum BusinessImpact
    {
        [EnumMember]
        LBI,
        [EnumMember]
        MBI,
        [EnumMember]
        HBI
    }

    [Serializable]
    public class xxContextItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ValueType { get; set; }
        public bool IsValidated { get; set; }
        public bool IsPicklist { get; set; }
        public string ValidationExecutable { get; set; }
        public string ValidationMethod { get; set; }
        public xxContextCategory Category { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public BusinessImpact BusinessImpact { get; set; }
        public bool SecureData { get; set; }
        public bool PrivacyData { get; set; }
        /// <summary>
        /// OAS Legacy Field
        /// </summary>
        public bool NotInDesign { get; set; }
        public bool Persistable { get; set; }
        /// <summary>
        /// OAS Legacy Field
        /// </summary>
        public bool NotInDb { get; set; }
        public string Tags { get; set; }

        

    }
}
