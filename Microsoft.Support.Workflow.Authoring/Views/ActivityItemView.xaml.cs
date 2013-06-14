// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityItemView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace Microsoft.Support.Workflow.Authoring.Views
{
    using Services;
    using ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ActivityItemView.xaml
    /// </summary>
    public partial class ActivityItemView : UserControl
    {
        TextboxHintTextExtension descriptionTextboxHintTextExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemView"/> class.
        /// </summary>
        public ActivityItemView()
        {
            InitializeComponent();
            Loaded += ActivityItemView_Loaded;
        }

        private void ActivityItemView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            descriptionTextboxHintTextExtension = new TextboxHintTextExtension(uxDescription, "Please supply a description");
        }


        private ActivityItemViewModel ViewModel
        {
            get { return Resources["ViewModel"] as ActivityItemViewModel; }
        }

        public string SelectedStatus
        {
            get { return ViewModel.SelectedStatus; }
            set { ViewModel.SelectedStatus = value; }
        }

        public string SelectedCategory
        {
            get { return ViewModel.SelectedCategory; }
            set { ViewModel.SelectedCategory = value; }
        }
    }
}