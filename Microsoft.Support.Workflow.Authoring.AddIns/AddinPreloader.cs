using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Threading;
using System.Reflection;
using System.Security.Policy;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    public class AddinPreloader
    {
        public static AddinPreloader Current { get; private set; }
        private Dictionary<IDesignerContract, AppDomain> addinDomainMaps;
        private IDesignerContract addin;
        static AddinPreloader()
        {
            Current = new AddinPreloader();
        }

        private AddinPreloader()
        {
            addinDomainMaps = new Dictionary<IDesignerContract, AppDomain>();
        }

        public IDesignerContract Get()
        {
            return Create();
        }

        public void Unload(IDesignerContract addin)
        {
            AppDomain domain = addinDomainMaps[addin];
            addinDomainMaps.Remove(addin);
            addin.Close();
            AppDomain.Unload(domain);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private IDesignerContract Create()
        {
            Type addinType = typeof(DesignerAddIn);
            AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, new AppDomainSetup
            {
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            });
            domain.InitializeLifetimeService();
            addin = (IDesignerContract)domain.CreateInstanceAndUnwrap(addinType.Assembly.FullName, addinType.FullName);
            addinDomainMaps.Add(addin, domain);
            return addin;
        }

    }
}
