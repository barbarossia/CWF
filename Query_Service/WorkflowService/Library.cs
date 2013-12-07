using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Microsoft.Support.Workflow.Catalog
{
    public partial class Library : IComparable
    {

        /// <summary>
        /// System.Version type is sealed and can not be 
        /// serialized, so we utilize a string version of
        /// it for storage, and an public, IgnoreDataMember
        /// property to perform version checking logic
        /// 
        /// </summary>
        /// 
        [IgnoreDataMember]
        public Version VersionNo
        {
            get { return new Version(VersionNumber); }
            set { VersionNumber = value.ToString(); }
        }

        [DataMember]
        public IList<Store_Activity> Activities 
        { 
            get; 
            set; 
        }
        bool _isSelected = false;
        [DataMember]
        public bool isSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public Library()
        {
            Id = Guid.Empty;
        }

        public EditReturnValue Edit()
        {

            //
            // Check library id for valid value.
            if (this.Id == null)
            {
                string msg = "Null Library.ID passed to LibrarySave ";
                CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return new EditReturnValue(false, msg);
            }
            else if (this.Id == Guid.Empty)
            {
                string msg = "Library.ID passed to LibrarySave had a Guid.Empty value ";
                CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return new EditReturnValue(false, msg);
            }

            // Remove trailing spaces on name prior to edit.
            this.Name = this.Name.Trim();
            //
            // Make sure we received a name
            if (string.IsNullOrEmpty(this.Name))
            {
                string msg = "Library Name not provided for LibrarySave";
                CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return new EditReturnValue(false, msg);
            }
            else if (this.Name.Length > 50)
            {
                string msg = "Library Name provided for LibrarySave is longer than 50 characters";
                CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return new EditReturnValue(false, msg);
            }

            //
            // Check for a valid version number format

            this.Description = this.Description.Trim();
            //
            //Check for a description
            if (String.IsNullOrEmpty(this.Description))
            {
                {
                    string msg = "Description not provided for LibrarySave";
                    CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Warning);
                    EditReturnValue temp = new EditReturnValue(true, msg);
                    temp.IsWarning = true;
                    return temp;
                }
            }

            //
            //Check for a description
            if (this.Description.Length > 250)
            {
                {
                    string msg = "Description provided for LibrarySave was longer than 250 characters and was truncated";
                    CWFHelpers.LogEventLog.Log2EventLog(msg, EventLogEntryType.Warning);
                    EditReturnValue temp = new EditReturnValue(true, msg);
                    temp.IsWarning = true;
                    return temp;
                }
            }

            return new EditReturnValue(true, "Library edited");

        }

        /// <summary>
        /// Compares two Library based on the full version number
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Less than 0 if this library is less than the library specified by the CompareTo method, 
        /// 0 if they are equal, and more </returns>
        public int CompareTo(object obj)
        {
            if (!(obj.GetType() == typeof(Library)))
            {
                return obj.GetType().Name.CompareTo(typeof(Library).Name);       
            }
            Library y = (Library)obj;
            if (this.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase))
            {
                return this.VersionNo.CompareTo(y.VersionNo);
            }
            else
            {
                return this.Name.CompareTo(y.Name);
            }
        }

        public static IList<Library> GetList(bool HasActivitiesOnly)
        {
            IList<Library> retList;
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                IList<StoreActivity> actList = (from al in proxy.StoreActivities
                                                  select al).ToList();

                if (HasActivitiesOnly)
                {
                    retList = (from ll in proxy.Libraries
                               where (bool)ll.HasActivities
                               select ll).ToArray();
                }
                else
                {
                    retList = (from ll in proxy.Libraries
                               select ll).ToArray();
                }

                foreach (var item in retList)
                {
                    item.Activities = (from a in actList
                                       where a.ActivityLibrary == item.Id
                                       select a.ConvertFrom()).ToList();
                }
            
            }

            return retList;

        
        }

        public static IList<Library> GetList(string name, VersionFlag versionFlag, bool HasActivitiesOnly)
        {
            IList<Library> retList;
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                IList<StoreActivity> actList = (from al in proxy.StoreActivities
                                              select al).ToList();

                if (HasActivitiesOnly)
                {
                    retList = (from ll in proxy.Libraries
                               where ll.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                               (bool)ll.HasActivities
                               select ll).ToArray();
                }
                else
                {
                    retList = (from ll in proxy.Libraries
                               where ll.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                               select ll).ToArray();
                }

                foreach (var item in retList)
                {
                    item.Activities = (from a in actList
                                       where a.ActivityLibrary == item.Id
                                       select a.ConvertFrom()).ToList();
                }
            }

            return GetVersion(retList, versionFlag);
        }

        private static IList<Library> GetVersion(IList<Library> aList,VersionFlag versionFlag)
        {
            IList<Library> retList = new List<Library>();

            if (versionFlag == VersionFlag.LastestVersion)
            {
                IEnumerable<Library> nodups =
                    aList.Distinct(new LibraryNameEqual());

                foreach (var item in nodups)
                {
                    retList.Add((from r2 in aList
                                 where r2.Name.ToUpper() == item.Name.ToUpper()
                                 orderby r2.VersionNo descending
                                 select r2).FirstOrDefault());

                }

                return retList;
            }
            else if (versionFlag == VersionFlag.LatestBuild)
            {
                IEnumerable<Library> nodups =
                    aList.Distinct(new LibraryVersionEqual());

                foreach (var item in nodups)
                {
                    retList.Add((from r2 in aList
                                 where r2.Name.ToUpper() == item.Name.ToUpper() &&
                                 r2.VersionNo.Major == item.VersionNo.Major &&
                                 r2.VersionNo.Minor == item.VersionNo.Minor
                                 orderby r2.VersionNo descending
                                 select r2).FirstOrDefault());

                }

                return retList;

            }
            else
            {
                return aList;
            }

        }

        public static IList<Library> GetList(VersionFlag versionFlag, bool HasActivitiesOnly)
        {
            IList<Library> aList = Library.GetList(HasActivitiesOnly);
            return GetVersion(aList, versionFlag);
        }

        
    }

   

    public class LibraryNameEqual : IEqualityComparer<Library>
    {

        public bool Equals(Library x, Library y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Library obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class LibraryVersionEqual : IEqualityComparer<Library>
    {

        public bool Equals(Library x, Library y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                x.VersionNo.Major == y.VersionNo.Major &&
                x.VersionNo.Minor == y.VersionNo.Minor);
        }

        public int GetHashCode(Library obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.Major ^ obj.VersionNo.Minor;
        }
    }

    public class LibraryRevisionEqual : IEqualityComparer<Library>
    {

        public bool Equals(Library x, Library y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                 x.VersionNo.Equals(y.VersionNo));
        }

        public int GetHashCode(Library obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.GetHashCode();
        }
    }

    
}