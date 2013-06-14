using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    /// <summary>
    /// View model for intellisense popup
    /// </summary>
    public class IntellisensePopupViewModel : NotificationObject
    {
        private ObservableCollection<TreeNode> treeNodes;
        private TreeNode selectedItem;

        /// <summary>
        /// Tree nodes to display
        /// </summary>
        public ObservableCollection<TreeNode> TreeNodes
        {
            get { return treeNodes; }
            set
            {
                treeNodes = value;
                RaisePropertyChanged(() => TreeNodes);
            }
        }

        /// <summary>
        /// Selected index of tree node
        /// </summary>
        public int SelectedIndex
        {
            get { return TreeNodes.IndexOf(SelectedItem); }
            set
            {
                if (value >= 0 && value < ItemsCount)
                    SelectedItem = TreeNodes[value];
            }
        }

        /// <summary>
        /// Selected tree node
        /// </summary>
        public TreeNode SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        /// <summary>
        /// Nodes count
        /// </summary>
        public int ItemsCount
        {
            get { return TreeNodes.Count; }
        }
    }
}
