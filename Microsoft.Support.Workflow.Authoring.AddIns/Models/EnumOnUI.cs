using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Support.Workflow.Authoring.Models
{
    /// <summary>
    /// UI wrapper for Enum
    /// </summary>
    public class EnumOnUI<T> where T : struct
    {
        /// <summary>
        /// The Enum value
        /// </summary>
        public T Value { get; private set; }
        /// <summary>
        /// DisplayName for Enum
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Initialize the wrapper
        /// </summary>
        /// <param name="value"></param>
        public EnumOnUI(T value)
        {
            Value = value;
            DisplayName = GetDisplayName(value);
        }

        private string GetDisplayName(T viewMode)
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

        public static EnumOnUI<T>[] GetUIModels() {
            return ((T[])Enum.GetValues(typeof(T))).Select(m => new EnumOnUI<T>(m)).ToArray();
        }
    }
}
