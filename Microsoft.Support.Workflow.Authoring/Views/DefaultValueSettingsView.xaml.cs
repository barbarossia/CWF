using Microsoft.Support.Workflow.Authoring.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Support.Workflow.Authoring.Views {
    /// <summary>
    /// Interaction logic for DefaultValueSettingsView.xaml
    /// </summary>
    public partial class DefaultValueSettingsView : Page, IOptionPage {
        private DefaultValueSettingsViewModel vm;

        public event EventHandler HasSavedChanged;

        public DefaultValueSettingsView() {
            InitializeComponent();

            vm = new DefaultValueSettingsViewModel();
            DataContext = vm;

            vm.PropertyChanged += (s, e) => {
                if (e.PropertyName == "HasSaved" && HasSavedChanged != null)
                    HasSavedChanged(this, new EventArgs());
            };
        }

        public bool HasSaved {
            get { return vm.HasSaved; }
        }

        public void Save() {
            vm.Save();
        }
    }
}
