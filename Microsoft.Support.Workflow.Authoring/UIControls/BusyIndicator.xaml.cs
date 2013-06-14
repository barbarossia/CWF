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

namespace Microsoft.Support.Workflow.Authoring.UIControls
{
    /// <summary>
    /// Interaction logic for BusyIndicator.xaml
    /// </summary>
    public partial class BusyIndicator : UserControl
    {
        public BusyIndicator()
        {
            InitializeComponent();
        }

        #region Public Properties

        /// <summary>
        /// IsIndeterminate
        /// </summary>
        public bool IsIndeterminate
        {
            get
            {
                return progressBar.IsIndeterminate;
            }
            set
            {
                progressBar.IsIndeterminate = value;
            }
        }

        /// <summary>
        /// Text to be displayed
        /// </summary>
        public string Text
        {
            get
            {
                return textBlock.Text;
            }
            set
            {
                textBlock.Text = value;
            }
        }

        #endregion
    }
}
