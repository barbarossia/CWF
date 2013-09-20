using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace AuthoringToolTests.Services
{
    /// <summary>
    /// When you want to run a unit test without affecting the actual contents of Caching.ActivityAssemblyItems or Caching.AssemblyItems
    /// use this class to "remember" the dependencies. Note that it does not isolate the underlying filesystem so files may still be generated
    /// in the "Assemblies" subdirectory by the tests.
    /// </summary>
    class CachingIsolator : IDisposable
    {
        ObservableCollection<ActivityAssemblyItem> oldActivityAssemblyItems;
        ObservableCollection<ActivityItem> oldActivityItems;

        /// <summary>
        /// Clear cache for test and optionally re-initialize it to a set of test assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to add</param>
        public CachingIsolator(params ActivityAssemblyItem[] assemblies)
        {
            // preserve the whole ObservableCollection object in addition to its contents,
            // just in case someone cares about the notifications set up on it
            this.oldActivityAssemblyItems = Caching.ActivityAssemblyItems;
            this.oldActivityItems = Caching.ActivityItems;

            Caching.ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(assemblies);
            Caching.ActivityItems = new ObservableCollection<ActivityItem>(assemblies.SelectMany(asm => asm.ActivityItems));
        }

        public void Dispose()
        {
            Caching.ActivityAssemblyItems = this.oldActivityAssemblyItems;
            Caching.ActivityItems = this.oldActivityItems;
        }
    }
}
