using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Support.Workflow.Authoring.Behaviors
{
    /// <summary>
    /// Help class of zoom factor value
    /// </summary>
    public class ZoomFactorHelper
    {
        /// <summary>
        /// predefined a value - calculated on assumption that maximum zoom value is 400%
        /// </summary>
        private const double MAXZOOMVALUE = 0.15;

        /// <summary>
        /// predefined c value - calculated on assumption that minimum zoom value is 25% 
        /// </summary>
        private const double MINZOOMVALUE = 25;

        /// <summary>
        /// FrameworkElement include zoom control
        /// </summary>
        private FrameworkElement element;

        /// <summary>
        /// Initializes a new instance of the ZoomFactorHelper class.
        /// </summary>
        /// <param name="element">FrameworkElement include zoom control</param>
        public ZoomFactorHelper(FrameworkElement element)
        {
            this.element = element;
        }

        /// <summary>
        /// Gets zoom factor
        /// </summary>
        public double ZoomFactor
        {
            get
            {
                StatusBar statusBar = (StatusBar)SelectPrintContentHelper.SearchDependencyObject(this.element, typeof(StatusBar));
                Slider zoomSlider = (Slider)SelectPrintContentHelper.SearchDependencyObject(statusBar, typeof(Slider));

                return CalculateZoom(zoomSlider.Value) / 100.0;
            }
        }

        private static double CalculateZoom(double value)
        {
            return ((value * value) * MAXZOOMVALUE) + MINZOOMVALUE;
        }
    }
}
