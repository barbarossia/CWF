using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    /// <summary>
    /// Enum for saving confirmation dialog
    /// </summary>
    [Flags]
    public enum SavingResult
    {
        /// <summary>
        /// do nothing
        /// </summary>
        DoNothing = 0,
        /// <summary>
        /// save workflow
        /// </summary>
        Save = 1,
        /// <summary>
        /// unlock workflow
        /// </summary>
        Unlock = 2,
    }
}
