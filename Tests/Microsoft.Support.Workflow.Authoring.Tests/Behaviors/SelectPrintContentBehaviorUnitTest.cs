using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using System.Windows.Controls;
using System.Activities.Statements;
using Telerik.Windows.Controls.Map.WPFBingSearchService;
using System.Activities.Presentation.View;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Windows.Media;
using Microsoft.DynamicImplementations;
using System.Windows.Documents;
using System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows.Controls.Primitives;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.Behaviors
{
    [TestClass]
    [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
    public class SelectPrintContentBehaviorUnitTest
    {
        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Ignore]
        public void aaaTest_SelectPrintOnLoad()
        {
            DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs(SelectPrintContentBehavior.PrintProperty, PrintAction.NoneAction, PrintAction.PrintAll);
            Canvas canvas = new Canvas();            
            AdornerDecorator adornerDecorator = new AdornerDecorator();
            canvas.Children.Add(adornerDecorator);

            using (var imgrid = new Implementation<Grid>())
            {
                
                imgrid.Register("get_ActualHeight").Return(50);
                imgrid.Register("get_ActualWidth").Return(50);
                Grid grid = (Grid)imgrid.Instance;

                ScrollViewer scrollViewer = new ScrollViewer();
                grid.Children.Add(scrollViewer);
                grid.Children.Add(new ActivityDesigner());
                adornerDecorator.Child = grid;
                using (var mainWindow = new ImplementationOfType(typeof(SelectPrintContentBehavior_Accessor)))
                {
                    bool print = false;
                    mainWindow.Register(() => SelectPrintContentBehavior_Accessor.PrintAll()).Execute(() =>
                        {
                            print = true;
                        });
                        
                    SelectPrintContentBehavior_Accessor.OnLoad(canvas, e);
                    
                    Assert.IsTrue(print);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.workflowViewMask);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.closeButton); 
                    //setmain = false;
                    e = new DependencyPropertyChangedEventArgs(SelectPrintContentBehavior.PrintProperty, PrintAction.PrintAll, PrintAction.PrintUserSelection);
                    SelectPrintContentBehavior_Accessor.OnLoad(canvas, e);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.rootActivityDesigner);
                    //setmain = false;
                    e = new DependencyPropertyChangedEventArgs(SelectPrintContentBehavior.PrintProperty, PrintAction.PrintUserSelection, PrintAction.NoneAction);
                    SelectPrintContentBehavior_Accessor.OnLoad(canvas, e);
                    Assert.IsNull(SelectPrintContentBehavior_Accessor.closeButton);

                }
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_PrintAll()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            WorkflowDesigner designer = new WorkflowDesigner();
            designer.Load(new Sequence());
            var ms = designer.Context.Services.GetService<ModelService>();            
            SelectPrintContentBehavior_Accessor.rootActivityDesigner = new ActivityDesigner() { ModelItem=ms.Root};
            SelectPrintContentBehavior_Accessor.workflowView = new Grid();
            SelectPrintContentBehavior_Accessor.scrollViewer = new ScrollViewer();

            using (var helper = new ImplementationOfType(typeof(SelectPrintContentHelper)))
            {               
                helper.Register(() => SelectPrintContentHelper.GetActivityMaxDepth(Argument<System.Activities.Activity>.Any))
                    .Return(10);

                SelectPrintContentBehavior_Accessor.PrintAll();
                SelectPrintContentBehavior_Accessor.animateVerticalScrollTimer.Stop();
                Assert.IsFalse(SelectPrintContentBehavior_Accessor.isCancel);

                helper.Register(() => SelectPrintContentHelper.GetActivityMaxDepth(Argument<System.Activities.Activity>.Any))
                    .Return(26);
                using (var message = new ImplementationOfType(typeof(AddInMessageBoxService)))
                {
                    bool flag = false;
                    message.Register(() => AddInMessageBoxService.PrintOverflowWorkflow(Argument<string>.Any))
                        .Execute(() =>
                        {
                            flag = true;
                        });
                    SelectPrintContentBehavior_Accessor.workflowItem = new WorkflowEditorViewModel(cancellationToken);
                    SelectPrintContentBehavior_Accessor.PrintAll();
                    Assert.IsTrue(flag);
                }
            }
            
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_TickDragScroll()
        {
            SelectPrintContentBehavior_Accessor.isDragging = true;
            SelectPrintContentBehavior_Accessor.workflowView = new Grid();
            SelectPrintContentBehavior_Accessor.selectedGeometry = new RectangleGeometry();
            SelectPrintContentBehavior_Accessor.workflowViewMask = new System.Windows.Shapes.Rectangle();
            SelectPrintContentBehavior_Accessor.drawingBrush = new DrawingBrush();
            //SelectPrintContentBehavior_Accessor.scrollViewer = new ScrollViewer();
            using (var scroll = new Implementation<ScrollViewer>())
            {
                scroll.Register("get_ScrollableHeight").Return(10.0);
                scroll.Register("get_ScrollableWidth").Return(10.0);
                SelectPrintContentBehavior_Accessor.scrollViewer = scroll.Instance;
                SelectPrintContentBehavior_Accessor.scrollViewer.RenderSize = new Size(10, 10);
                SelectPrintContentBehavior_Accessor.workflowView.Children.Add(SelectPrintContentBehavior_Accessor.scrollViewer);
                SelectPrintContentBehavior_Accessor.currentPosition = new Point(12, 12);
                using (var helpers = new ImplementationOfType(typeof(SelectPrintContentHelper)))
                {
                    helpers.Register(() => SelectPrintContentHelper.GetRelativeOffset(Argument<Visual>.Any, Argument<Visual>.Any))
                        .Return(new Point(10, 20));
                    SelectPrintContentBehavior_Accessor.TickDragScroll(null, null);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.workflowViewMask.Fill);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.rectangleSize.Height);
                }
            }
            SelectPrintContentBehavior_Accessor.isDragging = false;
            using (var drag = new Implementation<DispatcherTimer>())
            {
                bool flag = false;
                drag.Register(inst => inst.Stop())
                    .Execute(() =>
                    {
                        flag = true;
                    });
                SelectPrintContentBehavior_Accessor.dragScrollTimer = drag.Instance;
                SelectPrintContentBehavior_Accessor.TickDragScroll(null, null);
                Assert.IsTrue(flag);
                Assert.IsNull(SelectPrintContentBehavior_Accessor.dragScrollTimer);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_WorkflowViewMouseDown()
        {
            SelectPrintContentBehavior_Accessor.workflowView = new Grid();
            using (var grid = new Implementation<Grid>())
            {
                SelectPrintContentBehavior_Accessor.isMouseCapture = false;
                using (var helpers = new ImplementationOfType(typeof(SelectPrintContentHelper)))
                {
                    helpers.Register(() => SelectPrintContentHelper.GetRelativeOffset(Argument<Visual>.Any, Argument<Visual>.Any))
                         .Return(new Point(10, 20));
                    //SelectPrintContentBehavior_Accessor.WorkflowView_MouseDownMethod(typeof(SelectPrintContentBehavior), new Point(0, 0));
                    SelectPrintContentBehavior_Accessor.WorkflowView_MouseDownMethod(new Point(0, 0));
                    Assert.AreEqual(Cursors.Cross, SelectPrintContentBehavior_Accessor.workflowView.Cursor);
                    Assert.IsNotNull(SelectPrintContentBehavior_Accessor.startPosition);
                }
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_WorkflowViewMouseMove()
        {
            SelectPrintContentBehavior_Accessor.isMouseCapture = true;
            SelectPrintContentBehavior_Accessor.mouseDown = true;
            SelectPrintContentBehavior_Accessor.isDragging = false;
            using (var dispathtimer = new Implementation<DispatcherTimer>())
            {
                bool flag = false;
                dispathtimer.Register(inst => inst.Start())
                    .Execute(() =>
                    {
                        flag = true;
                    });
                SelectPrintContentBehavior_Accessor.CreateDispatcherTimerFun = () =>
                {
                    return dispathtimer.Instance;
                };
                SelectPrintContentBehavior_Accessor.dragScrollTimer = null;
                SelectPrintContentBehavior_Accessor.WorkflowView_MouseMoveMethod(new Point(0, 0));
                Assert.IsTrue(SelectPrintContentBehavior_Accessor.isDragging);
                Assert.IsTrue(flag);
            }

        }

        //[TestMethod]
        //[TestCategory("Unit-Dif")]
        //[Owner("v-jillhu")]
        //public void aaaTest_WorkflowViewMouseUp()
        //{
        //    SelectPrintContentBehavior_Accessor.isMouseCapture = true;
        //    SelectPrintContentBehavior_Accessor.workflowView = new Grid();
        //    SelectPrintContentBehavior_Accessor.startPosition = new Point(0, 0);
        //    SelectPrintContentBehavior_Accessor.endPosition = new Point(10, 10);
        //    SelectPrintContentBehavior_Accessor.rootActivityDesigner = new ActivityDesigner();
        //    SelectPrintContentBehavior_Accessor.selectedGeometry = new RectangleGeometry();
        //    SelectPrintContentBehavior_Accessor.drawingBrush = new DrawingBrush();
        //    SelectPrintContentBehavior_Accessor.workflowViewMask = new System.Windows.Shapes.Rectangle();
        //    SelectPrintContentBehavior_Accessor.workflowItem = new WorkflowEditorViewModel();
        //    using (var helpers = new ImplementationOfType(typeof(SelectPrintContentHelper)))
        //    {
        //        helpers.Register(() => SelectPrintContentHelper.GetRelativeOffset(Argument<Visual>.Any, Argument<Visual>.Any))
        //            .Return(new Point(10, 20));
        //        helpers.Register(() => SelectPrintContentHelper.SearchDependencyObject(Argument<DependencyObject>.Any, typeof(StatusBar)))
        //            .Return(null);
        //        Slider slider = new Slider();
        //        slider.Value = 100.0;
        //        helpers.Register(() => SelectPrintContentHelper.SearchDependencyObject(Argument<DependencyObject>.Any, typeof(Slider)))
        //            .Return(slider);

        //        #region if patch                
        //        using (var popupwindow = new ImplementationOfType(typeof(SelectPrintContentBehavior)))
        //        {
        //            bool popup = false;
        //            popupwindow.Register(() => SelectPrintContentBehavior_Accessor.PopUpPrintWindow(Argument<List<ActivityDesigner>>.Any))
        //                .Execute(() =>
        //                {
        //                    popup = true;
        //                });
        //            SelectPrintContentBehavior_Accessor.WorkflowView_MouseUpMethod(new Point(0,0));                   
        //            Assert.IsTrue(popup);    
        //        }
        //        #endregion                   
        //    }

        //}

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_VerticalScrolBarIsChanged()
        {
            SelectPrintContentBehavior_Accessor.workflowViewMask = new System.Windows.Shapes.Rectangle();
            using (var scroll = new Implementation<ScrollViewer>())
            {                
                using (var grid = new Implementation<Grid>())
                {
                    scroll.Register("get_ComputedVerticalScrollBarVisibility").Return(Visibility.Visible);
                    SelectPrintContentBehavior_Accessor.scrollViewer = scroll.Instance;
                    grid.Register("get_ActualWidth").Return(50);
                    SelectPrintContentBehavior_Accessor.workflowView = grid.Instance;
                    SelectPrintContentBehavior_Accessor.VerticalScrollBarIsChanged(null,null);
                    Assert.IsTrue(SelectPrintContentBehavior_Accessor.workflowViewMask.Width>0);
                    scroll.Register("get_ComputedVerticalScrollBarVisibility").Return(Visibility.Hidden);
                    SelectPrintContentBehavior_Accessor.VerticalScrollBarIsChanged(null,null);
                    Assert.AreEqual(50.0, SelectPrintContentBehavior_Accessor.workflowViewMask.Width);
                }
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-jillhu")]
        public void Test_HorizontalScrollBarIsChanged()
        {
            SelectPrintContentBehavior_Accessor.workflowViewMask = new System.Windows.Shapes.Rectangle();
            using (var scroll = new Implementation<ScrollViewer>())
            {
                using (var grid = new Implementation<Grid>())
                {
                    scroll.Register("get_ComputedHorizontalScrollBarVisibility").Return(Visibility.Visible);
                    SelectPrintContentBehavior_Accessor.scrollViewer = scroll.Instance;
                    grid.Register("get_ActualHeight").Return(50);
                    SelectPrintContentBehavior_Accessor.workflowView = grid.Instance;
                    SelectPrintContentBehavior_Accessor.HorizontalScrollBarIsChanged(null, null);
                    Assert.IsTrue(SelectPrintContentBehavior_Accessor.workflowViewMask.Height > 0);
                    scroll.Register("get_ComputedHorizontalScrollBarVisibility").Return(Visibility.Hidden);
                    SelectPrintContentBehavior_Accessor.HorizontalScrollBarIsChanged(null, null);
                    Assert.AreEqual(50.0, SelectPrintContentBehavior_Accessor.workflowViewMask.Height);
                }
            }
        }       

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-jillhu")]
        public void Test_CloseButton_Click()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            SelectPrintContentBehavior_Accessor.workflowItem = new WorkflowEditorViewModel(cancellationToken);
            SelectPrintContentBehavior_Accessor.CloseButton_Click(null,null);
            Assert.AreEqual(PrintAction.NoneAction, SelectPrintContentBehavior_Accessor.workflowItem.ShouldBePrint);
        }
    }
}