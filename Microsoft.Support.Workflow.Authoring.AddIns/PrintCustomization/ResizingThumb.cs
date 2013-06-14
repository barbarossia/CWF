using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Thumb for resizing
    /// </summary>
    public class ResizingThumb : Thumb
    {
        /// <summary>
        /// Is the thumb at top
        /// </summary>
        public bool IsTop { get; private set; }
        /// <summary>
        /// Is the thumb at left
        /// </summary>
        public bool IsLeft { get; private set; }

        /// <summary>
        /// Initialize the ResizingThumb
        /// </summary>
        /// <param name="isTop"></param>
        /// <param name="isLeft"></param>
        public ResizingThumb(bool isTop, bool isLeft)
        {
            IsTop = isTop;
            IsLeft = isLeft;
        }
    }
}
