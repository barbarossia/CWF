using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Interaction logic for PrintCustomization.xaml
    /// </summary>
    public partial class PrintCustomization : Window
    {
        private PrintCustomizationViewModel vm;
        private List<ActivityDesigner> designers;

        /// <summary>
        /// Initialize the print window
        /// </summary>
        /// <param name="designers"></param>
        public PrintCustomization(List<ActivityDesigner> designers)
        {
            InitializeComponent();

            this.designers = designers;

            vm = new PrintCustomizationViewModel();
            vm.View = this;
            DataContext = vm;

            Loaded += PrintCustomizationLoaded;
            SizeChanged += PrintCustomizationSizeChanged;
        }

        private void PrintCustomizationLoaded(object sender, RoutedEventArgs e)
        {
            vm.InitializeDragging(pageContainer, designers);
        }

        private void PrintCustomizationSizeChanged(object sender, SizeChangedEventArgs e)
        {
            vm.RefreshPanelScale();
        }
    }
}
