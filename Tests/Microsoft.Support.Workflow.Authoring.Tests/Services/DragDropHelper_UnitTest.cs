using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Telerik.Windows.Controls;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.DynamicImplementations;
using System.Windows;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using Helper = System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class DragDropHelper_UnitTest
    {
        #region test data

        private static Sequence s1 = new Sequence();
        private static WriteLine w1 = new WriteLine();
        private Sequence s2 = new Sequence()
        {
            Activities = { s1, w1 }
        };

        private ModelItem s2item;
        private ModelItem s1item;
        private ModelItem witem;

        [TestInitialize]
        public void TestInitialize()
        {
            //s2item = validWorkflowItem.WorkflowDesigner.Context.Services.GetService<ModelService>().Root;
            Helper.EditingContext ec = new Helper.EditingContext();
            ModelTreeManager manager = new ModelTreeManager(ec);
            manager.Load(s2);
            s2item = manager.Root;
            ModelItemCollection collection = s2item.Properties["Activities"].Value as ModelItemCollection;
            s1item = collection.ElementAt(0);
            witem = collection.ElementAt(1);
        }

        #endregion

        bool isDoCopy = false;
        bool isDoPaste = false;
        bool isDoCut = false;
        private ImplementationOfType MockCutCopyPasteHelper()
        {
            isDoCopy = false;
            isDoPaste = false;
            isDoCut = false;
            var cutcopypaste = new ImplementationOfType(typeof(Helper.CutCopyPasteHelper));

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.DoCut(Argument<Helper.EditingContext>.Any))
                .Execute(() => { isDoCut = true; });

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.DoCopy(Argument<Helper.EditingContext>.Any))
                .Execute(() => { isDoCopy = true; });

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.DoPaste(Argument<Helper.EditingContext>.Any))
                .Execute(() => { isDoPaste = true; });

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.CanPaste(Argument<Helper.EditingContext>.Any))
                .Execute(() => { return true; });

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.CanCopy(Argument<Helper.EditingContext>.Any))
                .Execute(() => { return true; });

            cutcopypaste.Register(() => Helper.CutCopyPasteHelper.CanCut(Argument<Helper.EditingContext>.Any))
                .Execute(() => { return true; });
            return cutcopypaste;

        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [Description("Test MouseMove method")]
        public void DragDropHelper_CheckMouseMove()
        {
            //Dragging Item is null
            DragDropHelper helper = new DragDropHelper(new RadTreeView());
            helper.DraggingItem = null;
            helper.MouseMove();

            //Dragging item is not null
            helper.DraggingItem = new RadTreeViewItem();
            helper.TargetItem = new RadTreeViewItem();


            using (var cutcopypaste = this.MockCutCopyPasteHelper())
            {
                using (var DragDropImp = new ImplementationOfType(typeof(DragDrop)))
                {
                    //when DragDropEffects is not in allowedEffects, Don't move item
                    DragDropImp.Register(() => DragDrop.DoDragDrop(Argument<DependencyObject>.Any, Argument<object>.Any, Argument<DragDropEffects>.Any))
                        .Return(DragDropEffects.None);

                    helper.MouseMove();
                    Assert.IsNotNull(helper.TargetItem);

                    //when source and target both are null, Don't move item
                    DragDropImp.Register(() => DragDrop.DoDragDrop(Argument<DependencyObject>.Any, Argument<object>.Any, Argument<DragDropEffects>.Any))
                       .Return(DragDropEffects.Copy);
                    helper.MouseMove();
                    Assert.IsNotNull(helper.TargetItem);

                    //Can move item
                    helper.DraggingItem.Header = new WorkflowOutlineNode(witem);
                    helper.TargetItem.Header = new WorkflowOutlineNode(s1item);
                    helper.MouseMove();
                    Assert.IsFalse(isDoCopy);
                }
            }
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [Description("Test DragOver method")]
        public void DragDropHelper_CheckDragOver()
        {
            using (var cutcopypaste = this.MockCutCopyPasteHelper())
            {
                DragDropHelper helper = new DragDropHelper(new RadTreeView());

                //draggingitem is null
                RadTreeViewItem item = new RadTreeViewItem();
                helper.DraggingItem = new RadTreeViewItem();

                DragDropEffects effect = helper.DragOver(item);
                Assert.AreEqual(effect, DragDropEffects.None);

                //draggingitem is not null
                helper.DraggingItem.Header = new WorkflowOutlineNode(witem); ;
                item.Header = new WorkflowOutlineNode(witem);
                effect = helper.DragOver(item);
                Assert.AreEqual(effect, DragDropEffects.Move);
            }
        }


        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [Description("Test Drop method")]
        public void DragDropHelper_CheckDrop()
        {
            DragDropHelper helper = new DragDropHelper(new RadTreeView());

            //target is null
            RadTreeViewItem item = new RadTreeViewItem();

            DragDropEffects effect = helper.Drop(new UIElement());
            Assert.AreEqual(effect, DragDropEffects.None);

            //dragging item is null
            effect = helper.Drop(item);
            Assert.AreEqual(effect, DragDropEffects.None);

            //valid dragging item and target item
            helper.DraggingItem = new RadTreeViewItem();
            helper.DraggingItem.Header = new WorkflowOutlineNode(witem); ;
            item.Header = new WorkflowOutlineNode(witem);
            effect = helper.Drop(item);
            Assert.AreEqual(effect, DragDropEffects.Move);
        }

    }
}
