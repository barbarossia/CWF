//-----------------------------------------------------------------------
// <copyright file="GetAllActivityLibrariesReplyDC.cs" company="Microsoft">
// Copyright
// ActivityLibrary DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Get all Activity library replyDC
    /// </summary>
    [DataContract]
    public class GetAllActivityLibrariesReplyDC : StatusReplyDC
    {
        private List<ActivityLibraryDC> list;

        /// <summary>
        /// List of ActivityLibraryDC
        /// </summary>
        [DataMember]
        public List<ActivityLibraryDC> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
