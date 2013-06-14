//-----------------------------------------------------------------------
// <copyright file="StoreActivitiesDC.cs" company="Microsoft">
// Copyright
// StoreActivities DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    /// <summary>
    /// Request/Reply data contract for StoreActivities
    /// </summary>
    [DataContract]
    public class StoreActivitiesDC : RequestReplyCommonHeader
    {
        /// <summary>
        /// The state this record is in -- is it public, private, or retired?
        /// </summary>
        public Versioning.WorkflowRecordState WorkflowRecordState { get; set; }

        /// <summary>
        /// <para>The identity for the row entry </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - if it is 0, then this a create(insert) otherwise an update </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// <para>The Guid for the row entry </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Used in create - Required, Update - Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// <para>The unique name for this row </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Used in create - Required, Update - Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// <para>The unique name for this row </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Used in create - Required, Update - Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - May or may not be the identifier for the operation </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// <para>A narative description of the Activity</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// <para>The name of the authgroup row entry that the AuthGroupID points to </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Used to specify the AuthGroup - Required; Update, optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string AuthGroupName { get; set; }

        /// <summary>
        /// <para>The PK of the authgroup row entry </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - n/a </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int AuthGroupId { get; set; }

        /// <summary>
        /// <para>A field of meta tags that where each entry is separated by a ";" and used for searching</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string MetaTags { get; set; }

        /// <summary>
        /// <para>The PK in the Icons tableK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int IconsId { get; set; }

        /// <summary>
        /// <para>Is this a service</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public bool IsService { get; set; }

        /// <summary>
        /// <para>The name of the Activity Library that this StoreActivity points to</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Optional - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ActivityLibraryName { get; set; }

        /// <summary>
        /// <para>The version of the Activity Library that this StoreActivity points to</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Optional, but required if ActivityLibraryName is specified - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ActivityLibraryVersion { get; set; }

        /// <summary>
        /// <para>The FK pointing to the ActivityLibrary for this entry</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ActivityLibraryId { get; set; }

        /// <summary>
        /// <para>The Activity Category name used to generate a FK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ActivityCategoryName { get; set; }

        /// <summary>
        /// <para>The Activity Category FK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - n/a </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ActivityCategoryId { get; set; }

        /// <summary>
        /// <para>The toolbox tab number</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Optional - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ToolBoxtab { get; set; }

        /// <summary>
        /// <para>Version of the StoreActivities row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// <para>FK pointing to the status table</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - n/a </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int StatusId { get; set; }

        /// <summary>
        /// <para>The name of the StatusCode used to generate a FK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Required - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string StatusCodeName { get; set; }

        /// <summary>
        /// <para>The FK pointing to the Workflow type table row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - n/a </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int WorkflowTypeID { get; set; }

        /// <summary>
        /// <para>The name of the workflowtype used to generate a FK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Required - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string WorkflowTypeName { get; set; }

        /// <summary>
        /// <para>The Guid of the workflowtype</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Required - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public Guid WorkflowTypeGuid { get; set; }

        /// <summary>
        /// <para>The lock bit for the StoreActivity row indicating that it is in use</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public bool Locked { get; set; }

        /// <summary>
        /// <para>The user alias that locked this row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string LockedBy { get; set; }

        /// <summary>
        /// <para>The XAML has a dll associated with it required to run the XAML</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public bool IsCodeBeside { get; set; }

        /// <summary>
        /// <para>XAML code</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Xaml { get; set; }

        /// <summary>
        /// <para>Name space for the activity</para>
        /// <para>CreateOrUpdate </para>
        /// <para>;Request - Create, Optional - Update, Optional</para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Namespace { get; set; }

        /// <summary>
        /// <para>user alias that inserted this row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Required </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public DateTime InsertedDateTime { get; set; }

        /// <summary>
        /// <para>user alias that inserted this row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Required </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public DateTime UpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets developer notes.
        /// </summary>
        [DataMember]
        public string DeveloperNotes { get; set; }

        /// <summary>
        /// The vesion will update lock
        /// </summary>
        [DataMember]
        public string OldVersion { get; set; }

    }
}
