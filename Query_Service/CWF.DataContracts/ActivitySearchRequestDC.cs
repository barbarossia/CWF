namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common base for ActivityLibraries (ActivityLibraries does not follow common pattern of request/reply DC)
    /// </summary>
    [DataContract]
    public class ActivitySearchRequestDC : WorkflowSearchRequestHeader
    {
        [DataMember]
        public bool FilterOlder {get;set;}
		
        [DataMember]
        public bool FilterByName {get;set;}

        [DataMember]
        public bool FilterByDescription {get;set;}

        [DataMember]
		public bool FilterByTags {get;set;}

        [DataMember]
		public bool FilterByType {get;set;}

        [DataMember]
		public bool FilterByVersion {get;set;}

        [DataMember]
        public bool FilterByCreator { get; set; }
       
    }
}