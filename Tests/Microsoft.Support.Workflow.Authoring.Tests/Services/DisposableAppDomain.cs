using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    /// <summary>
    /// Creates an instance of an AppDomain and destroys it when Dispose() method is called.
    /// </summary>
    public class DisposableAppDomain : IDisposable
    {
        private AppDomain _appDomain;

        public AppDomain AppDomain
        {
            get
            {
                return this._appDomain;
            }
        }

        public DisposableAppDomain()
        {
            _appDomain = AppDomain.CreateDomain("DisposableAppDomain");
        }

        public void Dispose()
        {
            AppDomain.Unload(this._appDomain);
        }
    }
}
