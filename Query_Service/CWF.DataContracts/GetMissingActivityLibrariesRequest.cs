
namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common data contract for StoreLibraryAndActivities
    /// </summary>
    [DataContract]
    public class GetMissingActivityLibrariesRequest : RequestHeader
    {
        public GetMissingActivityLibrariesRequest()
        {
            ActivityLibrariesList = null;
        }

        /// <summary>
        /// List of activity libraries that will be checked in the asset store
        /// </summary>
        [DataMember]
        public List<ActivityLibraryDC> ActivityLibrariesList { get; set; }

    }
}
