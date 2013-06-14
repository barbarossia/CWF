using System;
using Microsoft.Support.Workflow.Authoring.AddIns;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.UIControls
{
    public class CustomToolTip : ToolTip
    {
        private const string TitlePropertyName = "Title";
        private const string ActionPropertyName = "Action";

        /// <summary>
        /// TitleProperty difinition
        /// </summary>
        public static DependencyProperty TitleProperty =
            DependencyProperty.Register(
                TitlePropertyName,
                typeof(string),
                typeof(CustomToolTip));

        /// <summary>
        /// ActionProperty definition
        /// </summary>
        public static DependencyProperty ActionProperty =
            DependencyProperty.Register(
                ActionPropertyName,
                typeof(string),
                typeof(CustomToolTip));

        /// <summary>
        /// Gets or sets Title for custom tooltip control
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets Title for custom tooltip control
        /// </summary>
        public string Action
        {
            get { return (string)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        static CustomToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomToolTip), new FrameworkPropertyMetadata(typeof(CustomToolTip)));
        }

        public CustomToolTip()
            : base()
        {
            this.Opened += new RoutedEventHandler(this.MyToolTip_Opened);
            this.StaysOpen = true;
        }

        /// <summary>
        /// check if can open help doc
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool CanExecute(object sender)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            return !string.IsNullOrEmpty(HelpProvider.GetHelpTitle(senderElement)) || !string.IsNullOrEmpty(HelpProvider.GetHelpAction(senderElement));
        }

        /// <summary>
        /// When tooltip Opened, Set the Title and Action property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyToolTip_Opened(object sender, RoutedEventArgs e)
        {
            ToolTip tip = sender as ToolTip;
            if (tip != null && tip.PlacementTarget != null)
            {
                ToolTipService.SetShowDuration(tip.PlacementTarget, int.MaxValue);
                this.Title = HelpProvider.GetHelpTitle(tip.PlacementTarget as FrameworkElement);
                this.Action = HelpProvider.GetHelpAction(tip.PlacementTarget as FrameworkElement);
            }
        }
    }
}
