using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using CWF.BAL;
using Microsoft.Practices.Prism.Commands;
using CWF.WorkflowQueryService.Versioning;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Common
{

    public partial class VersionDisplay : UserControl, INotifyPropertyChanged
    {
        public const string DefaultVersionString = "1.0.0.0";
        public const string DefaultCaption = "Version";

        /// <summary>
        /// event for INotifyPropertyChanged, to notify listeners that a property has changed 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        // lazily gets a reference to the viewmodel
        private VersionDisplayViewModel viewModel;
        public VersionDisplayViewModel ViewModel
        {
            get
            {
                if (null == viewModel)
                    viewModel = Resources["ViewModel"] as VersionDisplayViewModel;

                return viewModel;
            }
        }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the Version property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty VersionProperty = DependencyProperty.RegisterAttached("Version",
                                                                                                 typeof(string),
                                                                                                 typeof(VersionDisplay),
                                                                                                 new PropertyMetadata(
                                                                                                                        DefaultVersionString,
                                                                                                                        (sender, e) =>
                                                                                                                        {
                                                                                                                            if (e.OldValue != e.NewValue)
                                                                                                                                (sender as VersionDisplay).Version = (string)e.NewValue;
                                                                                                                        }
                                                                                                                     ));


        /// <summary>
        ///  The version object that we are going to display and (potentially) allow the user to manipulate.
        /// </summary>
        public bool HasMajorChanged
        {
            get { return ViewModel.HasMajorChanged; }
            set
            {
                ViewModel.HasMajorChanged = value;
                PropertyChanged(this, new PropertyChangedEventArgs("HasMajorChanged"));
            }
        }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the Version property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty HasMajorChangedProperty = DependencyProperty.RegisterAttached("HasMajorChanged",
                                                                                                 typeof(bool),
                                                                                                 typeof(VersionDisplay),
                                                                                                 new PropertyMetadata(
                                                                                                                        false,
                                                                                                                        (sender, e) =>
                                                                                                                        {
                                                                                                                            if (e.OldValue != e.NewValue)
                                                                                                                                (sender as VersionDisplay).HasMajorChanged = (bool)e.NewValue;
                                                                                                                        }
                                                                                                                     ));


        /// <summary>
        ///  The version object that we are going to display and (potentially) allow the user to manipulate.
        /// </summary>
        public string Version
        {
            get { return ViewModel.Version; }
            set
            {
                ViewModel.Version = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Version"));
            }
        }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the Caption property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.RegisterAttached("Caption",
                                                                                          typeof(string),
                                                                                          typeof(VersionDisplay),
                                                                                          new PropertyMetadata(
                                                                                                                DefaultCaption,
                                                                                                                (sender, e) =>
                                                                                                                {
                                                                                                                    if (e.OldValue != e.NewValue)
                                                                                                                        (sender as VersionDisplay).Caption = (string)e.NewValue;
                                                                                                                }
                                                                                                              ));

        /// <summary>
        /// This is the text displayed above the version number sections.
        /// </summary>
        public string Caption
        {
            get { return ViewModel.Caption; }
            set
            {
                ViewModel.Caption = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Caption"));
            }
        }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the VersionFault property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty VersionFaultProperty = DependencyProperty.RegisterAttached("VersionFault",
                                                                                  typeof(VersionFault),
                                                                                  typeof(VersionDisplay),
                                                                                  new PropertyMetadata(
                                                                                                          null,
                                                                                                          (sender, e) =>
                                                                                                          {
                                                                                                              if (e.OldValue != e.NewValue)
                                                                                                                  (sender as VersionDisplay).VersionFault = (VersionFault)e.NewValue;
                                                                                                          }
                                                                                                       ));

        /// <summary>
        /// A fault reported on the version number. The scenario is likely that the user tried to save, but the version was invalid, and we got htis fault back.
        /// Binding to it here will allow us to update the display, perform actions on the front end, etc.
        /// </summary>
        public VersionFault VersionFault
        {
            get { return ViewModel.VersionFault; }
            set
            {
                ViewModel.VersionFault = value;
                PropertyChanged(this, new PropertyChangedEventArgs("VersionFault"));
            }
        }

        /// <summary>
        /// Set up commands, the data context, controls, etc. for this user control.
        /// </summary>
        public VersionDisplay()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                ViewModel.PropertyChanged += (sender1, e1) =>
                {
                    switch (e1.PropertyName)
                    {
                        case "Major":
                        case "Minor":
                        case "Build":
                        case "Revision":
                            SetValue(VersionProperty, ViewModel.Version);
                            break;
                        case "HasMajorChanged":
                            SetValue(HasMajorChangedProperty, ViewModel.HasMajorChanged);
                            break;
                    }
                };
            };
        }

    }
}
