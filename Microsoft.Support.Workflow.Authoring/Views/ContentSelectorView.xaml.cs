// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSelectorView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Models;
    using ViewModels;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for ContentSelectorView.xaml
    /// </summary>
    public partial class ContentSelectorView : UserControl
    {
        #region Constructors and Destructors       

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSelectorView"/> class.
        /// </summary>
        public ContentSelectorView()
        {
            InitializeComponent();           

            // For some reason this method call causes problems if we called it in design mode inside VisualStudio.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new ContentSelectorViewModel();               
            }
        }        
       
        #endregion

        /// <summary>
        /// seperate from ContentListItem_MouseMove for unit test
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public MouseButtonState GetButtonState(MouseEventArgs e) 
        {
            return e.LeftButton;
        }

        /// <summary>
        /// The event handler for the mouse move in the list of content items. Verify to start a drag/drop operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentListItem_MouseMove(object sender, MouseEventArgs e)
        {
           
            var element = e.OriginalSource as FrameworkElement;            
           
            if (element != null && element.DataContext != null && element.DataContext is ContentItem && GetButtonState(e) == MouseButtonState.Pressed)
            {
                var selected = element.DataContext as ContentItem;
                ContentPanel.SelectedItem = selected;
                var keyExpression = string.Format("\"{0}\"", selected.Key);
               
                DragDrop.DoDragDrop(element, keyExpression, DragDropEffects.Link);
            }  
        }               

        private void PanelBarItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = sender as RadPanelBarItem;

            if (item != null)
            {
                var group = item.DataContext as CollectionViewGroup;
                if (group != null)
                {
                    (DataContext as ContentSelectorViewModel).SelectedGroup = group;
                }
            }            
        }

        private void PanelBarItem_Loaded(object sender, RoutedEventArgs e)
        {
            var item = sender as RadPanelBarItem;
            var group = item.DataContext as CollectionViewGroup;

            if ((item != null) && (group != null) && ((DataContext as ContentSelectorViewModel).SelectedGroup != null))
            {
                if ((DataContext as ContentSelectorViewModel).SelectedGroup.Name == group.Name)
                {
                    item.IsExpanded = true;
                }
                else
                {
                    item.IsExpanded = false;
                }
            }
        }
    }
}