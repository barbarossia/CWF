using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    public interface IOptionPage
    {
        /// <summary>
        /// 
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 
        /// </summary>
        bool HasSaved { get; }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler HasSavedChanged;

        /// <summary>
        /// 
        /// </summary>
        void Save();
    }
}
