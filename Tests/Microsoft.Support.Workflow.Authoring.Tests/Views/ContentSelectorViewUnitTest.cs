using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using System.Activities.Statements;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    public class TestControl : UserControl
    {
        public static readonly RoutedEvent TestClickEvent = EventManager.RegisterRoutedEvent("TestClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ContentSelectorView));

        public event RoutedEventHandler TestClick
        {
            add { AddHandler(TestClickEvent, value); }
            remove { RemoveHandler(TestClickEvent, value); }
        }
    }

    public class TestGroup : CollectionViewGroup
    {
        public TestGroup(object name):base(name) { }

        public override bool IsBottomLevel { get { return true; } }
    }

    [TestClass]
    public class ContentSelectorViewUnitTest
    {
        [WorkItem(325746)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyDataContext()
        {
            var view = new ContentSelectorView();
            var dataContext = view.DataContext;
            Assert.IsNotNull(dataContext);
            Assert.IsTrue(dataContext is ContentSelectorViewModel);
        }

        [WorkItem(325743)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyContentListItem_MouseMove()
        {
            using (var contentSelectorView = new Implementation<ContentSelectorView>())
            {
                var e = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                contentSelectorView.Instance.GetButtonState(e);

                contentSelectorView.Register(inst => inst.GetButtonState(Argument<MouseEventArgs>.Any)).Return(MouseButtonState.Pressed);
                var view = contentSelectorView.Instance;

                using (var helper = new ImplementationOfType(typeof(DragDrop)))
                {
                    bool isDragDrop = false;
                    helper.Register(() => DragDrop.DoDragDrop(Argument<DependencyObject>.Any, Argument<object>.Any, DragDropEffects.Link)).Execute(() =>
                    {
                        isDragDrop = true;
                        return DragDropEffects.Link;
                    });

                    var element = new TestControl();
                    element.DataContext = new ContentItem() { Key = "key" };

                    e.RoutedEvent = Mouse.MouseDownEvent;
                    e.Source = element;

                    PrivateObject po = new PrivateObject(view);
                    po.Invoke("ContentListItem_MouseMove", null, e);
                    Assert.IsTrue(isDragDrop);
                }
            }
        }

        [WorkItem(325759)]
        [Owner("v-kason1")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Views_VerifyPanelBarItem_Expanded()
        {
            var view = new ContentSelectorView();

            RadPanelBarItem sender = new RadPanelBarItem();
            var dataContext = view.DataContext as ContentSelectorViewModel;
            var group = new TestGroup("mygroup") as CollectionViewGroup;
            sender.DataContext = group; 
            
            PrivateObject po = new PrivateObject(view);
            po.Invoke("PanelBarItem_Expanded", sender, null);
            Assert.AreEqual(dataContext.SelectedGroup, group);
        }

        [WorkItem(325760)]
        [Owner("v-kason1")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Views_VerifyPanelBarItem_Loaded()
        {
            var view = new ContentSelectorView();

            RadPanelBarItem sender = new RadPanelBarItem();
            var dataContext = view.DataContext as ContentSelectorViewModel;
            var group = new TestGroup("mygroup") as CollectionViewGroup;
            sender.DataContext = group;
            dataContext.SelectedGroup = group;

            PrivateObject po = new PrivateObject(view);
            po.Invoke("PanelBarItem_Loaded", sender, null);
            Assert.IsTrue(sender.IsExpanded);

            dataContext.SelectedGroup = new TestGroup("newgroup");
            po.Invoke("PanelBarItem_Loaded", sender, null);
            Assert.IsFalse(sender.IsExpanded);
        }

    }



}
