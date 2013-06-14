using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// UI wrapper for PrintViewMode
    /// </summary>
    public class PrintViewModeOnUI
    {
        /// <summary>
        /// The PrintViewMode value
        /// </summary>
        public PrintViewMode ViewMode { get; private set; }
        /// <summary>
        /// DisplayName for PrintViewMode
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Initialize the wrapper
        /// </summary>
        /// <param name="viewMode"></param>
        public PrintViewModeOnUI(PrintViewMode viewMode)
        {
            ViewMode = viewMode;
            DisplayName = GetDisplayName(viewMode);
        }

        private string GetDisplayName(PrintViewMode viewMode)
        {
            string text = viewMode.ToString();

            int i = 1;
            while (i < text.Length)
            {
                if (char.IsUpper(text[i]))
                {
                    text = text.Insert(i, " ");
                    i++;
                }
                i++;
            }
            return text;
        }
    }
}
