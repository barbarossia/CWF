using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    public class LibraryKeys
    {
        /// <summary>
        /// Added as a result of the EDm to ER model conversion
        /// </summary>
        public Guid NEWActivityLibraryID { get; set; }
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Only valid in EDm model
        /// </summary>
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }
        public string MajorVersion { get { return (new System.Version(this.Version).Major.ToString()); } }
        public string MinorVersion { get { return (new System.Version(this.Version).Minor.ToString()); } }
        public string Build { get { return (new System.Version(this.Version).Build.ToString()); } }
        public string Revision { get { return (new System.Version(this.Version).Revision.ToString()); } }

    }

    [Serializable]
    public class LibraryList :  ICollection<LibraryKeys>
    {
        private IList<LibraryKeys> libList = new List<LibraryKeys>();

        public void Add(LibraryKeys item)
        {
            libList.Add(item);
        }

        public void Clear()
        {
            libList.Clear();
        }

        public bool Contains(LibraryKeys item)
        {
            int i = (from l in libList
                     where l.Id == item.Id &&
                         l.Name.Trim().Equals(item.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                         l.MajorVersion.Equals(item.MajorVersion) &&
                         l.MinorVersion.Equals(item.MinorVersion)
                     select l).Count();
            return (bool)(i > 0);
        }

        public bool Contains(Guid id)
        {
            int i = (from l in libList
                     where l.Id == id
                     select l.Id).Count();
            return (bool)(i > 0);
        }

        public void CopyTo(LibraryKeys[] array, int arrayIndex)
        {
            libList.CopyTo(array, arrayIndex);
            
        }

        public int Count
        {
            get {return libList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(LibraryKeys item)
        {
            return libList.Remove(item);
        }

        public IEnumerator<LibraryKeys> GetEnumerator()
        {
            return libList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
