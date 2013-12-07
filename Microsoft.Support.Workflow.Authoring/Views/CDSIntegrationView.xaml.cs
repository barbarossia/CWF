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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Support.Workflow.Authoring.Views {
    /// <summary>
    /// Interaction logic for CDSIntegrationView.xaml
    /// </summary>
    public partial class CDSIntegrationView : Page, IOptionPage {
        private CDSIntegrationViewModel vm;

        public event EventHandler HasSavedChanged;

        public CDSIntegrationView() {
            InitializeComponent();
            
            vm = new CDSIntegrationViewModel();
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
