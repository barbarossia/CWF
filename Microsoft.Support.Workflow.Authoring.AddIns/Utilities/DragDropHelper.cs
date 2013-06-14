using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Telerik.Windows.Controls;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    /// <summary>
    /// Helper class for drag and drop
    /// </summary>
    public class DragDropHelper
    {
        private readonly DragDropEffects defaultEffect = DragDropEffects.Move;

        private const DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Copy;
        private bool isShiftPressed
        {
            get
            {
                return ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
            }
        }

        private bool isCtrlPressed
        {
            get
            {
                return ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
            }
        }
        private DragDropEffects userActionEffects
        {
            get
            {
                switch (defaultEffect)
                {
                    case DragDropEffects.Move:
                        if (isCtrlPressed)
                            return DragDropEffects.Copy;
                        break;
                    case DragDropEffects.Copy:
                        if (isShiftPressed)
                            return DragDropEffects.Move;
                        break;
                }
                return defaultEffect;
            }
        }

        private RadTreeView treeView;
        private Dictionary<WorkflowOutlineNode, bool> dragCheckResults = new Dictionary<WorkflowOutlineNode, bool>();

        /// <summary>
        /// Item which is dragging
        /// </summary>
        public RadTreeViewItem DraggingItem { get; set; }

        public RadTreeViewItem TargetItem { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="treeView"></param>
        public DragDropHelper(RadTreeView treeView)
        {
            this.treeView = treeView;
        }

        /// <summary>
        /// Handle the mouse move action
        /// </summary>
        public void MouseMove()
        {
            //if (DraggingItem != null && treeView.SelectedValue != null)
            //{
            //    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(treeView, treeView.SelectedValue, allowedEffects);
            //    // checking target is not null and item is dragging(moving)
            //    if (((finalDropEffect & allowedEffects) != 0)
            //        && (TargetItem != null)
            //        && CheckDropTarget(TargetItem))
            //    {
            //        MoveItem(DraggingItem, TargetItem);
            //        TargetItem = null;
            //    }
            //    dragCheckResults.Clear();
            //}
        }

        /// <summary>
        /// Handle dragging an item over another.
        /// </summary>
        /// <param name="e"></param>
        public DragDropEffects DragOver(UIElement e)
        {
            // verify that this is a valid drop and then store the drop target
            RadTreeViewItem item = GetNearestContainer(e);
            if (CheckDropTarget(item))
            {
                return userActionEffects;
            }
            else
            {
                return DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handle drop action
        /// </summary>
        /// <param name="e"></param>
        public DragDropEffects Drop(UIElement e)
        {
            // verify that this is a valid drop and then store the drop target
            RadTreeViewItem targetItem = GetNearestContainer(e);
            if (targetItem != null && DraggingItem != null)
            {
                TargetItem = targetItem;
                if (DraggingItem != null && treeView.SelectedValue != null)
                {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(treeView, treeView.SelectedValue, allowedEffects);
                    // checking target is not null and item is dragging(moving)
                    if (((finalDropEffect & allowedEffects) != 0)
                        && (TargetItem != null)
                        && CheckDropTarget(TargetItem))
                    {
                        MoveItem(DraggingItem, TargetItem);
                        TargetItem = null;
                    }
                    dragCheckResults.Clear();
                }
                return userActionEffects;
            }
           
            return DragDropEffects.None;
        }

        private bool CheckDropTarget(RadTreeViewItem targetItem)
        {
            if (DraggingItem == targetItem)
            {
                return false;
            }

            WorkflowOutlineNode source = GetNode(DraggingItem);
            WorkflowOutlineNode target = GetNode(targetItem);

            if (source == null || target == null)
                return false;

            if (userActionEffects == DragDropEffects.Move && CheckIsChild(source, target))
            {
                return false;
            }

            if (!dragCheckResults.ContainsKey(target))
            {
                EditingContext ec = source.Model.GetEditingContext();
                CutCopyPasteHelper.DoCopy(ec);
                ec.Items.SetValue(new Selection(target.Model));
                dragCheckResults.Add(target, CutCopyPasteHelper.CanPaste(ec));
                ec.Items.SetValue(new Selection(source.Model));
            }
            return dragCheckResults[target];
        }

        private bool CheckIsChild(WorkflowOutlineNode source, WorkflowOutlineNode target)
        {
            if (source.Children != null && source.Children.Any())
            {
                foreach (var c in source.Children)
                {
                    if (c == target || CheckIsChild(c, target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void MoveItem(RadTreeViewItem sourceItem, RadTreeViewItem targetItem)
        {
            bool shouldCopy = (userActionEffects == DragDropEffects.Copy);

            WorkflowOutlineNode s = GetNode(sourceItem);
            WorkflowOutlineNode t = GetNode(targetItem);
            EditingContext ec = s.Model.GetEditingContext();

            if (shouldCopy)
            {
                CutCopyPasteHelper.DoCopy(ec);
            }
            else
            {
                CutCopyPasteHelper.DoCut(ec);
            }
            ec.Items.SetValue(new Selection(t.Model));
            CutCopyPasteHelper.DoPaste(ec);
        }

        private RadTreeViewItem GetNearestContainer(UIElement element)
        {
            // walk up the element tree to the nearest tree view item.
            RadTreeViewItem container = element as RadTreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as RadTreeViewItem;
            }
            return container;
        }

        private WorkflowOutlineNode GetNode(RadTreeViewItem item)
        {
            if (IsSentinelObject(item))
                return null;
            return (WorkflowOutlineNode)item.Header;
        }

        private bool IsSentinelObject(object dataContext)
        {
            return dataContext.GetType().FullName == "MS.Internal.NamedObject"
                   || dataContext.ToString() == "{DisconnectedItem}";
        }
    }
}
