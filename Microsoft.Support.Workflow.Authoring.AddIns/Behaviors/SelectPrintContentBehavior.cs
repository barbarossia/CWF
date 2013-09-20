using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Behaviors
{
    /// <summary>
    /// Help class of print selection
    /// </summary>
    public partial class SelectPrintContentBehavior : FrameworkElement
    {
        #region Dependency Property

        /// <summary>
        /// Dependency Property for the Print "event"
        /// </summary>
        public static readonly DependencyProperty PrintProperty
            = DependencyProperty.RegisterAttached(
                PRINTPROPERTYNAME, 
                typeof(PrintAction), 
                typeof(SelectPrintContentBehavior), 
                new PropertyMetadata(new PropertyChangedCallback(OnLoad)));

        #endregion

        #region Print static Property

        /// <summary>
        /// Gets the print from a Dependency Property
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static PrintAction GetPrint(DependencyObject sender)
        {
            return (PrintAction)sender.GetValue(PrintProperty);
        }

        /// <summary>
        /// Sets the print to a Dependency Property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        public static void SetPrint(DependencyObject sender, PrintAction value)
        {
            sender.SetValue(PrintProperty, value);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// milliseconds
        /// </summary>
        private const int DRAGPOLLINGINTERVAL = 10;
        private const int CLOSEBUTTONCONTENTSIZE = 18;
        private const double FRICTION = 5;
        private const double PRINTFRICTION = 50;
        private const double OPACITY = 0.3;
        private const string PRINTPROPERTYNAME = "Print";
        private const string CLOSEBUTTONCONTENT = "X";
        private const string CLOSEBUTTONTOOLTIP = "Exit Print";
        private static Point startPosition;
        private static Point currentPosition;
        private static Point endPosition;
        private static bool mouseDown;
        private static bool isDragging;
        private static WorkflowEditorViewModel workflowItem;
        private static Grid workflowView;
        private static DispatcherTimer dragScrollTimer;
        private static ScrollViewer scrollViewer;
        private static bool isMouseCapture;
        private static DesignerView designerView;
        private static Size rectangleSize;
        private static Brush backgroundBrush;
        private static Rectangle workflowViewMask;
        private static ActivityDesigner rootActivityDesigner;
        private static Vector rootActivityDesignerOffset;
        private static RectangleGeometry selectedGeometry;
        private static DrawingBrush drawingBrush;
        private static Button closeButton;
        private static Size closeButtonSize = new Size(30, 30);
        private static Thickness closeButtonMargin = new Thickness(10, 10, 20, 20);
        private static DispatcherTimer animateVerticalScrollTimer;
        private static DispatcherTimer animateHorizonScrollTimer;
        private static bool isCancel;

        /// <summary>
        /// Specifies the vertical overflow state of sroll bar
        /// </summary>
        internal enum ScrollViewVerticalOverflow
        {
            /// <summary>
            /// Not overflow
            /// </summary>
            NotOverflow = 0,

            /// <summary>
            /// Up the scroll view
            /// </summary>
            UpOverflow = -1,

            /// <summary>
            /// Down the scroll view
            /// </summary>
            DownOverflow = 1,
        }

        /// <summary>
        /// Specifies the horizon overflow state of sroll bar
        /// </summary>
        internal enum ScrollViewHorizonOverflow
        {
            /// <summary>
            /// Not overflow
            /// </summary>
            NotOverflow = 0,

            /// <summary>
            /// Left over the scroll view
            /// </summary>
            LeftOverflow = -1,

            /// <summary>
            /// Right over the scroll view
            /// </summary>
            RightOverflow = 1,
        }

        #endregion

        #region Events

        private static void VerticalScrollBarIsChanged(object sender, EventArgs e)
        {
            double width = scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible ? workflowView.ActualWidth - SystemParameters.VerticalScrollBarWidth : workflowView.ActualWidth;
            workflowViewMask.Width = width;
        }

        private static void HorizontalScrollBarIsChanged(object sender, EventArgs e)
        {
            double height = scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible ? workflowView.ActualHeight - SystemParameters.HorizontalScrollBarHeight : workflowView.ActualHeight;
            workflowViewMask.Height = height;
        }

        

        private static void WorkflowView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WorkflowView_MouseDownMethod(e.MouseDevice.GetPosition(workflowView));
        }

        private static void WorkflowView_MouseDownMethod(Point startPoint)
        {
            if (!isMouseCapture)
            {
                BeginDrag();
                startPosition = startPoint;
                GetRootActivityDesignerOffset(startPosition);
            }
        }

        private static void WorkflowView_MouseMove(object sender, MouseEventArgs e)
        {
            WorkflowView_MouseMoveMethod(e.MouseDevice.GetPosition(workflowView));
        }

        private static void WorkflowView_MouseMoveMethod(Point current)
        {
            if (isMouseCapture)
            {
                currentPosition = current;

                if (mouseDown && !isDragging)
                {
                    isDragging = true;
                    DragScroll();
                }
            }
        }

        private static void WorkflowView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WorkflowView_MouseUpMethod(e.MouseDevice.GetPosition(workflowView));
        }

        private static void WorkflowView_MouseUpMethod(Point end)
        {
            if (isMouseCapture)
            {
                endPosition = end;
                Print();
                CancelDrag();
            }
        }

        private static void OnLoad(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(dependencyObject is FrameworkElement);

            FrameworkElement element = (FrameworkElement)dependencyObject;
            PrintAction userAction = (PrintAction)e.NewValue;
            switch (userAction)
            {
                case PrintAction.PrintAll:
                    if (Load(element))
                    {
                        PrintAll();
                    }

                    break;
                case PrintAction.PrintUserSelection:
                    Load(element);
                    break;
                case PrintAction.NoneAction:
                    UnLoad();
                    break;
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!isDragging)
            {
                DrawSelection();
            }
        }

        #endregion

        private static bool Load(FrameworkElement element)
        {
            workflowItem = LoadWorkflowItem(element);
            designerView = LoadDeignerView(element);

            AdornerDecorator decorator = LoadAdornerDecorator(element);
            workflowView  = LoadWorkflowView(decorator);
            RegisterWorkflowViewEvents(workflowView);

            scrollViewer = LoadScrollViewer(decorator);
            RegisterScrollViewChanged(scrollViewer);

            workflowViewMask = CreateWorkflowViewMask(workflowView);
            closeButton = CreateCloseButton(workflowView);
            rootActivityDesigner = LoadRootActivityDesigner(workflowView);

            if (rootActivityDesigner == null)
            {
                AddInMessageBoxService.PrintNoneActivityMessage();
                ExitPrint();
                return false;
            }

            return true;
        }

        private static void UnLoad()
        {
            UnRegisterWorkflowViewEvents(workflowView);
            UnRegisterScrollViewChanged(scrollViewer);
            UnLoadWorkflowViewMask(workflowView);
            UnLoadCloseButton(workflowView);
        }

        private static void UnLoadWorkflowViewMask(Grid grid)
        {
            if (grid.Children.Contains(workflowViewMask))
            {
                grid.Children.Remove(workflowViewMask);
            }

            workflowViewMask = null;
        }

        private static void UnLoadCloseButton(Grid grid)
        {
            if (grid.Children.Contains(closeButton))
            {
                grid.Children.Remove(closeButton);
            }

            closeButton.Click -= new RoutedEventHandler(CloseButton_Click);
            closeButton = null;
        }

        private static void ClearSelection()
        {
            startPosition = currentPosition = endPosition = new Point();
            rectangleSize = new Size();
            DrawMask(new Rect());
        }

        private static ActivityDesigner LoadRootActivityDesigner(FrameworkElement element)
        {
            return (ActivityDesigner)SelectPrintContentHelper.SearchDependencyObject(element, typeof(ActivityDesigner));
        }

        private static DesignerView LoadDeignerView(FrameworkElement element)
        {
           return (DesignerView)SelectPrintContentHelper.SearchDependencyObject(element, typeof(DesignerView));
        }

        private static WorkflowEditorViewModel LoadWorkflowItem(FrameworkElement element)
        {
            return element.DataContext as WorkflowEditorViewModel;
        }

        private static Rectangle CreateWorkflowViewMask(Grid grid)
        {
            Rectangle workflowViewMask = new Rectangle();
            double width = scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible ? workflowView.ActualWidth - SystemParameters.VerticalScrollBarWidth : workflowView.ActualWidth;
            double height = scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible ? workflowView.ActualHeight - SystemParameters.HorizontalScrollBarHeight : workflowView.ActualHeight;
            workflowViewMask.Width = width;
            workflowViewMask.Height = height;
            Rect background = new Rect(0, 0, workflowViewMask.Width, workflowViewMask.Height);
            backgroundBrush = new SolidColorBrush(SystemColors.AppWorkspaceColor);

            drawingBrush = new DrawingBrush();
            RectangleGeometry maskGeometry = new RectangleGeometry(background);
            selectedGeometry = new RectangleGeometry(new Rect());

            CombinedGeometry combinedGeometry = new CombinedGeometry(
                GeometryCombineMode.Exclude,
                maskGeometry,
                selectedGeometry);

            GeometryDrawing geometryDrawing = new GeometryDrawing(backgroundBrush, null, combinedGeometry);

            workflowViewMask.Opacity = OPACITY;
            drawingBrush.Drawing = geometryDrawing;
            workflowViewMask.Fill = drawingBrush;

            workflowViewMask.VerticalAlignment = VerticalAlignment.Top;
            workflowViewMask.HorizontalAlignment = HorizontalAlignment.Left;

            if (!grid.Children.Contains(workflowViewMask))
            {
                grid.Children.Add(workflowViewMask);
            }

            return workflowViewMask;
        }

        private static Button CreateCloseButton(Grid grid)
        {
            Button btn = new Button();
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.Width = closeButtonSize.Width;
            btn.Height = closeButtonSize.Height;
            btn.Content = CLOSEBUTTONCONTENT;
            btn.Foreground = new SolidColorBrush(Colors.Black);
            btn.FontSize = CLOSEBUTTONCONTENTSIZE;
            btn.Margin = closeButtonMargin;
            ToolTip toolTip = new ToolTip()
            {
                Content = CLOSEBUTTONTOOLTIP,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Right,
                PlacementTarget = btn,
            };

            btn.ToolTip = toolTip;
            btn.Click += new RoutedEventHandler(CloseButton_Click);

            if (!grid.Children.Contains(btn))
            {
                grid.Children.Add(btn);
            }

            return btn;
        }

        private static void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ExitPrint();
        }

        private static ScrollViewer LoadScrollViewer(DependencyObject dependencyObject)
        {
            return (ScrollViewer)SelectPrintContentHelper.SearchDependencyObject(dependencyObject, typeof(ScrollViewer));
        }

        private static Grid LoadWorkflowView(DependencyObject dependencyObject)
        {
            return (Grid)SelectPrintContentHelper.SearchDependencyObject(dependencyObject, typeof(Grid));
        }

        private static AdornerDecorator LoadAdornerDecorator(DependencyObject dependencyObject)
        {
            return (AdornerDecorator)SelectPrintContentHelper.SearchDependencyObject(dependencyObject, typeof(AdornerDecorator));
        }

        private static void RegisterWorkflowViewEvents(FrameworkElement element)
        {
            element.MouseLeftButtonDown += new MouseButtonEventHandler(WorkflowView_MouseDown);
            element.MouseMove += new MouseEventHandler(WorkflowView_MouseMove);
            element.MouseUp += new MouseButtonEventHandler(WorkflowView_MouseUp);
        }

        private static void UnRegisterWorkflowViewEvents(FrameworkElement element)
        {
            element.MouseLeftButtonDown -= new MouseButtonEventHandler(WorkflowView_MouseDown);
            element.MouseMove -= new MouseEventHandler(WorkflowView_MouseMove);
            element.MouseUp -= new MouseButtonEventHandler(WorkflowView_MouseUp);
        }

        private static void RegisterScrollViewChanged(ScrollViewer scrollViewer)
        {
            DependencyPropertyDescriptor horizontalScrollBarVisibilityDescriptor = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedHorizontalScrollBarVisibilityProperty, typeof(ScrollViewer));
            horizontalScrollBarVisibilityDescriptor.AddValueChanged(scrollViewer, new EventHandler(HorizontalScrollBarIsChanged));

            DependencyPropertyDescriptor verticalScrollBarVisibilityDescriptor = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));
            verticalScrollBarVisibilityDescriptor.AddValueChanged(scrollViewer, new EventHandler(VerticalScrollBarIsChanged));

            scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
        }

        private static void UnRegisterScrollViewChanged(ScrollViewer scrollViewer)
        {
            DependencyPropertyDescriptor horizontalScrollBarVisibilityDescriptor = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedHorizontalScrollBarVisibilityProperty, typeof(ScrollViewer));
            horizontalScrollBarVisibilityDescriptor.RemoveValueChanged(scrollViewer, new EventHandler(HorizontalScrollBarIsChanged));

            DependencyPropertyDescriptor verticalScrollBarVisibilityDescriptor = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));
            verticalScrollBarVisibilityDescriptor.RemoveValueChanged(scrollViewer, new EventHandler(VerticalScrollBarIsChanged));

            scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
        }

        private static void BeginDrag()
        {
            mouseDown = true;
            isMouseCapture = true;
            workflowView.CaptureMouse();
            workflowView.Cursor = Cursors.Cross;
        }

        private static void CancelDrag()
        {
            isDragging = false;
            mouseDown = false;
            isMouseCapture = false;

            workflowView.ReleaseMouseCapture();
            workflowView.Cursor = Cursors.Arrow;

            ClearSelection();
        }



        private static void DragScroll()
        {
            if (dragScrollTimer == null)
            {
                dragScrollTimer = CreateDispatcherTimerFun();
                dragScrollTimer.Tick += TickDragScroll;
                dragScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, DRAGPOLLINGINTERVAL);
                dragScrollTimer.Start();
            }
        }

        private static void TickDragScroll(object sender, EventArgs e)
        {
            if (isDragging)
            {
                GeneralTransform generalTransform = workflowView.TransformToVisual(scrollViewer);
                Point childToParentCoordinates = generalTransform.Transform(new Point(0.0, 0.0));
                Rect bounds = new Rect(childToParentCoordinates, scrollViewer.RenderSize);

                if (!bounds.Contains(currentPosition))
                {
                    ScrollViewVerticalOverflow verticalUpDown = ScrollViewVerticalOverflow.NotOverflow;
                    ScrollViewHorizonOverflow horizonUpDown = ScrollViewHorizonOverflow.NotOverflow;
                    if (currentPosition.Y > scrollViewer.ViewportHeight)
                    {
                        verticalUpDown = ScrollViewVerticalOverflow.DownOverflow;
                    }
                    else if (currentPosition.Y < scrollViewer.ViewportHeight)
                    {
                        verticalUpDown = ScrollViewVerticalOverflow.UpOverflow;
                    }

                    if (currentPosition.X > scrollViewer.ViewportWidth)
                    {
                        horizonUpDown = ScrollViewHorizonOverflow.RightOverflow;
                    }
                    else if (currentPosition.Y < scrollViewer.ViewportWidth)
                    {
                        horizonUpDown = ScrollViewHorizonOverflow.LeftOverflow;
                    }

                    PerformScroll(verticalUpDown, horizonUpDown);
                }

                if (!mouseDown)
                {
                    CancelDrag();
                }

                DrawSelection();
            }
            else
            {
                if (dragScrollTimer != null)
                {
                    dragScrollTimer.Tick -= TickDragScroll;
                    dragScrollTimer.Stop();
                    dragScrollTimer = null;
                }
            }
        }

        private static void PerformScroll(ScrollViewVerticalOverflow verticalUpDown, ScrollViewHorizonOverflow horizonUpDown)
        {
            double verticalFriction = FRICTION * (int)verticalUpDown;
            double horizonFriction = FRICTION * (int)horizonUpDown;

            double verticalOffset = Math.Max(0.0, scrollViewer.VerticalOffset + verticalFriction);
            double horizontalOffset = Math.Max(0.0, scrollViewer.HorizontalOffset + horizonFriction);

            if (verticalOffset <= scrollViewer.ScrollableHeight)
            {
                scrollViewer.ScrollToVerticalOffset(verticalOffset);
            }

            if (horizontalOffset <= scrollViewer.ScrollableWidth)
            {
                scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            }
        }

        private static void DrawSelection()
        {
            Rect selectedRect = GetSelectionRect(startPosition, currentPosition);
            Point actualLeftTopPos = new Point(Math.Max(0.0, selectedRect.Left), Math.Max(0.0, selectedRect.Top));
          
            double width = Math.Abs(selectedRect.Right - actualLeftTopPos.X);
            double height = Math.Abs(selectedRect.Bottom - actualLeftTopPos.Y);

            Rect displayRect = new Rect(actualLeftTopPos, new Size(width, height));

            DrawMask(displayRect);

            rectangleSize.Width = selectedRect.Width;
            rectangleSize.Height = selectedRect.Height;
        }

        private static void DrawMask(Rect selectedRect)
        {
            selectedGeometry.Rect = selectedRect;
            if (workflowViewMask != null)
            {
                workflowViewMask.Fill = drawingBrush;
            }
        }

        private static Rect GetSelectionRect(Point startPos, Point endPos)
        {
            Point offset = SelectPrintContentHelper.GetRelativeOffset(rootActivityDesigner, workflowView);
            Vector newRootActivityDesignerOffset = startPos - offset;

            if (newRootActivityDesignerOffset != rootActivityDesignerOffset)
            {
                startPos.Y = offset.Y + rootActivityDesignerOffset.Y;
                startPos.X = offset.X + rootActivityDesignerOffset.X;
            }

            if (isDragging)
            {
                return new Rect(startPos, endPos);
            }
            else
            {
                double left = startPos.X;
                double top = startPos.Y;
                if (endPos.Y < startPos.Y)
                {
                    top = startPos.Y - rectangleSize.Height;
                }

                if (endPos.X < startPos.X)
                {
                    left = startPos.X - rectangleSize.Width;
                }

                return new Rect(left, top, rectangleSize.Width, rectangleSize.Height);
            }
        }

        private static void GetRootActivityDesignerOffset(Point startPos)
        {
            Point offset = SelectPrintContentHelper.GetRelativeOffset(rootActivityDesigner, workflowView);
            rootActivityDesignerOffset = startPos - offset;
        }
      
        private static void Print()
        {
            ZoomFactorHelper zoomHelper = new ZoomFactorHelper(designerView);
            Rect selectionRect = GetSelectionRect(startPosition, endPosition);

            SelectPrintContentHelper helper = new SelectPrintContentHelper(workflowView, zoomHelper.ZoomFactor, rootActivityDesigner);            
            List<ActivityDesigner> selectedActivityDesigners = helper.FilterActivityDesigners(selectionRect);
            if (selectedActivityDesigners.Any())
            {
                PopUpPrintWindow(selectedActivityDesigners);
            }
            else
            {
                AddInMessageBoxService.PrintNoneSelectMessage();
            }
        }       

        private static void PopUpPrintWindow(List<ActivityDesigner> selectedActivityDesigners)
        {
            PrintCustomization.PrintCustomization printWindow = new PrintCustomization.PrintCustomization(selectedActivityDesigners); 
            printWindow.ShowDialog();
            bool? dialogResult = printWindow.DialogResult;
            if (dialogResult != true)
            {
                ExitPrint();
            }
        }

        private static void ExitPrint()
        {
            isCancel = true;
            workflowItem.ShouldBePrint = PrintAction.NoneAction;
        }

        private static void PrintAll()
        {
            int maxDepth = SelectPrintContentHelper.GetActivityMaxDepth(rootActivityDesigner.ModelItem.GetCurrentValue() as Activity);
            if (maxDepth > 25)
            {
                AddInMessageBoxService.PrintOverflowWorkflow(workflowItem.Name);
                ExitPrint();
                return;
            }

            isCancel = false;
            UnRegisterWorkflowViewEvents(workflowView);
            scrollViewer.ScrollToLeftEnd();
            scrollViewer.ScrollToTop();
            workflowView.Cursor = Cursors.Wait;
            animateVerticalScrollTimer = new DispatcherTimer();
            animateVerticalScrollTimer.Tick += AnimateVerticalScroll;
            animateVerticalScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, DRAGPOLLINGINTERVAL);
            animateVerticalScrollTimer.Start();
        }

        private static void AnimateVerticalScroll(object sender, EventArgs e)
        {
            if (!isCancel && scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + PRINTFRICTION);
                scrollViewer.ScrollToLeftEnd();
                animateHorizonScrollTimer = new DispatcherTimer();
                animateHorizonScrollTimer.Tick += ((s1, e1) =>
                {
                    if (!isCancel && scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth)
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + PRINTFRICTION);
                });
                animateHorizonScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, DRAGPOLLINGINTERVAL);
                animateHorizonScrollTimer.Start();
            }
            else
            {
                animateVerticalScrollTimer.Tick -= AnimateVerticalScroll;
                animateVerticalScrollTimer.Stop();
                animateVerticalScrollTimer = null;

                if (animateHorizonScrollTimer != null)
                {
                    animateHorizonScrollTimer.Stop();
                    animateHorizonScrollTimer = null;
                }

                workflowView.Cursor = Cursors.Arrow;

                if (workflowItem.ShouldBePrint == PrintAction.PrintAll)
                {
                    PopUpPrintAllWindow();
                }
            }
        }

        private static void PopUpPrintAllWindow()
        {
            PopUpPrintWindow(new[] { rootActivityDesigner }.ToList());
        }

        private static Func<DispatcherTimer> CreateDispatcherTimerFun = () =>
        {
            return new DispatcherTimer();
        };
    }
}
