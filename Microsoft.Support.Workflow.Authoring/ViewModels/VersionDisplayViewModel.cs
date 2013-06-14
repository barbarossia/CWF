using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.Common;
using CWF.WorkflowQueryService.Versioning;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{

    /// <summary>
    /// ViewModel for displaying verion numbers, allowing manipulation, etc.
    /// </summary>
    public class VersionDisplayViewModel : NotificationObject
    {
        private int major;
        private int minor;
        private int build;
        private int revision;
        private string caption;
        private bool changeMajorCanExecute = true;
        private VersionFault versionFault;
        public DelegateCommand ChangeMajorCommand { get; private set; }
        public string Version
        {
            get
            {
                return new Version(Major, Minor, Build, Revision).ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Version ver = new Version(value);
                    Major = ver.Major;
                    Minor = ver.Minor;
                    Build = ver.Build;
                    Revision = ver.Revision;
                }
                else
                {
                    Major = Minor = Build = Revision = 0;
                }
            }
        }

        /// <summary>
        /// A fault reported on the version number. The scenario is likely that the user tried to save, but the version was invalid, and we got htis fault back.
        /// Binding to it here will allow us to update the display, perform actions on the front end, etc.
        /// </summary>
        public VersionFault VersionFault
        {
            get { return versionFault; }
            set
            {
                versionFault = value;
                RaisePropertyChanged(() => VersionFault);
            }
        }

        private bool hasMajorChanged;
        public bool HasMajorChanged
        {
            get { return hasMajorChanged; }
            set
            {
                hasMajorChanged = value;
                RaisePropertyChanged(() => HasMajorChanged);
                RaisePropertyChanged(() => MajorContent);
            }
        }

        public string MajorContent
        {
            get { return HasMajorChanged ? "-" : "+"; }
        }

        /// <summary>
        /// Determines whether the user can increment the major number of the version through this class.
        /// </summary>
        public bool ChangeMajorCanExecute
        {
            get { return changeMajorCanExecute; }
            set
            {
                changeMajorCanExecute = value;
                RaisePropertyChanged(() => ChangeMajorCanExecute);
            }
        }

        /// <summary>
        /// The "major" portion of the Version we are manipulating.
        /// </summary>
        public int Major
        {
            get { return major; }
            set
            {
                if (major != value)
                {
                    major = value;
                    RaisePropertyChanged(() => Major);
                }
            }
        }

        /// <summary>
        /// The "minor" portion of the Version we are manipulating.
        /// </summary>
        public int Minor
        {
            get { return minor; }
            set
            {
                if (minor != value)
                {
                    minor = value;
                    RaisePropertyChanged(() => Minor);
                }
            }
        }

        /// <summary>
        /// The "build" portion of the Version we are manipulating.
        /// </summary>
        public int Build
        {
            get { return build; }
            set
            {
                if (build != value)
                {
                    build = value;
                    RaisePropertyChanged(() => Build);
                }
            }
        }

        /// <summary>
        /// The "revision" portion of the Version we are manipulating.
        /// </summary>
        public int Revision
        {
            get { return revision; }
            set
            {
                if (revision != value)
                {
                    revision = value;
                    RaisePropertyChanged(() => Revision);
                }
            }
        }

        /// <summary>
        /// This is the text displayed above the version number sections.
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                RaisePropertyChanged(() => Caption);
            }
        }

        public VersionDisplayViewModel()
        {
            Version = VersionDisplay.DefaultVersionString;
            ChangeMajorCommand = new DelegateCommand(ChangeMajorCommandCommandExecute);
            ChangeMajorCanExecute = false;
        }

        private void ChangeMajorCommandCommandExecute()
        {
            if (HasMajorChanged)
            {
                Major--;
            }
            else
            {
                Major++;
            }
            HasMajorChanged = !HasMajorChanged;
        }
    }
}
