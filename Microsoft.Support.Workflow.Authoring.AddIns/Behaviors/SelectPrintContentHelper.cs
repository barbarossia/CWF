using System;
using System.Activities;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Support.Workflow.Authoring.Behaviors
{
    /// <summary>
    /// Help class of print behavior
    /// </summary>
    public class SelectPrintContentHelper
    {
        /// <summary>
        /// FrameworkElement which presents the workflow designer control
        /// </summary>
        private FrameworkElement designerView;

        /// <summary>
        /// the zoom of workflow designer control
        /// </summary>
        private double zoomFactor;

        /// <summary>
        /// the root of workflow designer control
        /// </summary>
        private ActivityDesigner rootActivityDesigner;

        /// <summary>
        /// Initializes a new instance of the SelectPrintContentHelper class.
        /// </summary>
        /// <param name="designerView">workflow designer view</param>
        /// <param name="zoomFactor">zoom factor of workflow designer</param>
        /// <param name="rootActivityDesigner">the root activity designer</param>
        public SelectPrintContentHelper(FrameworkElement designerView, double zoomFactor, ActivityDesigner rootActivityDesigner)
        {
            Contract.Requires(designerView != null);
            Contract.Requires(rootActivityDesigner != null);

            this.designerView = designerView;
            this.zoomFactor = zoomFactor;
            this.rootActivityDesigner = rootActivityDesigner;
        }

        /// <summary>
        /// Specifies the selection state two rectangle.
        /// </summary>
        internal enum Selection
        {
            /// <summary>
            /// rectangle is entirely contained by the rectangle 
            /// </summary>
            In,

            /// <summary>
            /// rectangle is not entirely contained by the rectangle
            /// </summary>
            Out,

            /// <summary>
            /// the specified rectangle intersects with the current rectangle
            /// </summary>
            Intersect,
        }

        /// <summary>
        /// Search all DependencyObject members in start DependencyObject
        /// </summary>
        /// <param name="startObject">start DependencyObject</param>
        /// <param name="filterType">return DependencyObject's type</param>
        /// <returns>return DependencyObject which match the filter type</returns>
        public static DependencyObject SearchDependencyObject(DependencyObject startObject, Type filterType)
        {
            Contract.Requires(startObject != null);
            Contract.Requires(filterType != null);

            return SearchDependencyObjects(startObject, filterType).FirstOrDefault();
        }

        /// <summary>
        /// Get relative offset
        /// </summary>
        /// <param name="child">child visual</param>
        /// <param name="parent">parent visual</param>
        /// <returns>the relative offset</returns>
        public static Point GetRelativeOffset(Visual child, Visual parent)
        {
            Contract.Requires(child != null);
            Contract.Requires(parent != null);

            Point relativePoint = child.TransformToAncestor(parent)
                              .Transform(new Point(0, 0));
            return relativePoint;
        }

        /// <summary>
        /// Get activity max depth
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static int GetActivityMaxDepth(Activity root)
        {
            int depth = 0;
            int max = 0;

            if (root == null)
            {
                return 0;
            }

            foreach (var child in WorkflowInspectionServices.GetActivities(root))
            {
                depth = GetActivityMaxDepth(child);
                if (depth > max)
                {
                    max = depth;
                }
            }

            return max + 1;
        }

        /// <summary>
        /// Gets the selected activity designers
        /// </summary>
        /// <param name="selection">selection rectangle</param>
        /// <returns>list of activity designers</returns>
        public List<ActivityDesigner> FilterActivityDesigners(Rect selection)
        {
            Contract.Requires(!selection.IsEmpty);

            List<ActivityDesigner> foundItems = new List<ActivityDesigner>();
            this.FilterActivityDesigners(this.rootActivityDesigner, foundItems, selection);
            return foundItems;
        }

        private static IEnumerable<DependencyObject> SearchDependencyObjects(DependencyObject startObject, Type filterType)
        {
            Queue<DependencyObject> searchQueues = new Queue<DependencyObject>();
            searchQueues.Enqueue(startObject);

            while (searchQueues.Count > 0)
            {
                DependencyObject currentObj = searchQueues.Dequeue();
                if (currentObj == null)
                {
                    continue;
                }

                int count = VisualTreeHelper.GetChildrenCount(currentObj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject obj = VisualTreeHelper.GetChild(currentObj, i);
                    if (filterType.IsAssignableFrom(obj.GetType()))
                    {
                        yield return obj;
                    }
                    else
                    {
                        searchQueues.Enqueue(obj);
                    }
                }
            }
        }

        private void FilterActivityDesigners(ActivityDesigner rootDesigner, List<ActivityDesigner> foundItems, Rect selection)
        {
            Selection sel = this.IsInSection(selection, rootDesigner);
            if (sel == Selection.In)
            {
                foundItems.Add(rootDesigner);
            }
            else if (sel == Selection.Intersect)
            {
                IEnumerable<DependencyObject> subDesigners = SearchDependencyObjects(rootDesigner, typeof(ActivityDesigner));
                if (subDesigners.Any())
                {
                    foreach (var sub in subDesigners)
                    {
                        this.FilterActivityDesigners((ActivityDesigner)sub, foundItems, selection);
                    }
                }
            }
        }

        private Selection IsInSection(Rect selection, ActivityDesigner designer)
        {
            Rect activityDesignerRect = this.GetActivityDesignerRect(designer);
            if (selection.Contains(activityDesignerRect))
            {
                return Selection.In;
            }
            else if (selection.IntersectsWith(activityDesignerRect))
            {
                return Selection.Intersect;
            }
            else
            {
                return Selection.Out;
            }
        }

        private Rect GetActivityDesignerRect(ActivityDesigner designer)
        {
            Point offset = GetRelativeOffset(designer, this.designerView);
            double designerWidth = this.GetActualSize(designer.ActualWidth);
            double designerHeight = this.GetActualSize(designer.ActualHeight);

            return new Rect(offset, new Size(designerWidth, designerHeight));
        }

        private double GetActualSize(double size)
        {
            return size * this.zoomFactor;
        }
    }
}
