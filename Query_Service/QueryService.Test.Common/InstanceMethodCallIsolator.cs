using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Service.Test.Common
{
    /// <summary>
    /// Defines an instance method call isolator for a generic type T.
    /// </summary>
    /// <typeparam name="T">Type that contains the method to provide dynamic implementation with.</typeparam>
    public  class InstanceMethodCallIsolator<T> : Implementation, IDisposable
    {
        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <param name="methodName">Method name that should be provided with a dynamic implementation.</param>
        /// <param name="implementation">Delegate that provides the expected implementation.</param>
        public InstanceMethodCallIsolator(string methodName, ImplementationCallback implementation)
        {
            this.Register(typeof(T).GetMethod(methodName), null, null, null, RegistrationOptions.None, implementation);
        }
    }
}
