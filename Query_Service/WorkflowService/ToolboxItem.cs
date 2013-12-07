using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow.Catalog
{
    public partial class ToolboxItem
    {
        /// <summary>
        /// System.Version type is sealed and can not be 
        /// serialized, so we utilize a string version of
        /// it for storage, and an public, IgnoreDataMember
        /// property to perform version checking logic
        /// 
        /// </summary>
        [IgnoreDataMember]
        public Version VersionNo
        {
            get { return new Version(Version); }
            set { Version = value.ToString(); }
        }

        public static IList<ToolboxItem> GetList(VersionFlag versionFlag)
        {
            IList<ToolboxItem> tempList;
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                tempList = (from t in proxy.ToolboxItems
                            select t).ToList<ToolboxItem>();


            }

            if (versionFlag == VersionFlag.AllBuilds)
            {
                return tempList;
            }

            IList<ToolboxItem> returnList = new List<ToolboxItem>();
            if (versionFlag == VersionFlag.LastestVersion)
            {
                IEnumerable<ToolboxItem> nodups =
                    tempList.Distinct(new ToolboxItemNameEqual());

                foreach (var r in nodups)
                {
                    returnList.Add((from r2 in tempList
                                    where r2.Name == r.Name
                                    orderby (new Version(r2.Version)) descending
                                    select r2).FirstOrDefault());
                }

            }
            else
            {
                IEnumerable<ToolboxItem> nodups =
                    tempList.Distinct(new ToolboxItemVersionEqual());

                foreach (var r in nodups)
                {
                    Version rv = new Version(r.Version);
                    returnList.Add((from r2 in tempList
                                    where r2.Name == r.Name &&
                                    (new Version(r2.Version)).Major == rv.Major &&
                                    (new Version(r2.Version)).Minor == rv.Minor
                                    orderby (new Version(r2.Version)) descending
                                    select r2).FirstOrDefault());
                }
            }

            return returnList;
        }
    }



    
    public class ToolboxItemNameEqual : IEqualityComparer<ToolboxItem>
    {

        public bool Equals(ToolboxItem x, ToolboxItem y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase));
                
        }

        public int GetHashCode(ToolboxItem obj)
        {
            return obj.Name.GetHashCode();
        }
    }
   
    public class ToolboxItemVersionEqual : IEqualityComparer<ToolboxItem>
    {

        public bool Equals(ToolboxItem x, ToolboxItem y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                x.VersionNo.Major == y.VersionNo.Major &&
                x.VersionNo.Minor == y.VersionNo.Minor);
        }

        public int GetHashCode(ToolboxItem obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.Major ^ obj.VersionNo.Minor;
        }
    }

    public class ToolboxItemRevisionEqual : IEqualityComparer<ToolboxItem>
    {

        public bool Equals(ToolboxItem x, ToolboxItem y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                 x.VersionNo.Equals(y.VersionNo));
        }

        public int GetHashCode(ToolboxItem obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.GetHashCode();
        }
    }


}