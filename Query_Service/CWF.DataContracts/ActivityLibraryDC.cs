//-----------------------------------------------------------------------
// <copyright file="ActivityLibraryDC.cs" company="Microsoft">
// Copyright
// ActivityLibrary DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common base for ActivityLibraries (ActivityLibraries does not follow common pattern of request/reply DC)
    /// </summary>
    [DataContract]
    public class ActivityLibraryDC : RequestReplyCommonHeader
    {
        //private string insertedByUserAlias;
        //private string updatedByUserAlias;

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
        /// <para>FK constraint to AuthGroup table </para>
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
        /// <para>Category Guid </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Used to specify the Category GUID - Required; Update, optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public Guid Category { get; set; }

        /// <summary>
        /// <para>The activityCategory name that is used in a create to derive a FK repltionship in the DB </para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create - Used to specify the CategoruID FK - Required, Update - Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// <para>The FK that points at a row in the ActivityCategory table</para>
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
        public int CategoryId { get; set; }

        /// <summary>
        /// <para>The Dll contents as a byte array</para>
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
        public byte[] Executable { get; set; }

        /// <summary>
        /// <para>Boolean that indicates that this library is associated with StoreActivity entries</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - true or false </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public bool HasActivities { get; set; }

        /// <summary>
        /// <para>A narative description of the library</para>
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
        /// <para>The user alias that created this library</para>
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
        public string ImportedBy { get; set; }

        /// <summary>
        /// <para>The version of the library</para>
        /// <para>CreateOrUpdate </para>
        /// <para>Request - Create, Optional - Update, Optional </para>
        /// <para>Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>Request - n/a </para>
        /// <para>Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>Request - Not required, however, if present will return only that version</para>
        /// <para>Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string VersionNumber { get; set; }

        /// <summary>
        /// <para>The status of this library row (see enum in CommonWorkflowSystemLibrary.Status.cs</para>
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
        public string StatusName { get; set; }

        /// <summary>
        /// <para>The status of this library row (see enum in CommonWorkflowSystemLibrary.Status.cs</para>
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
        public int Status { get; set; }

        /// <summary>
        /// A field of meta tags that where each entry is separated by a ";" and used for searching
        /// </summary>
        [DataMember]
        public string MetaTags { get; set; }

        /// <summary>
        /// Field for a supporting Friendly Names in the Toolbox and other areas of the app
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// A field of release notes for the user to store notes during the import operation
        /// </summary>
        [DataMember]
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// Determines whether the ActivityLibrary has data in its [Executable] field.
        /// </summary>
        [DataMember]
        public bool HasExecutable { get; set; }
    }
}