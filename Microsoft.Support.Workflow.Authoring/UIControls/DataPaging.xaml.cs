using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace Microsoft.Support.Workflow.Authoring.UIControls
{
    /// <summary>
    /// Interaction logic for DataPaging.xaml
    /// </summary>
    public partial class DataPaging : UserControl
    {
        #region Private & Protected methods
        private int availablePagesCount = 5;
        #endregion

        #region Constructors & Dispose

        /// <summary>
        /// Constructor for initialization
        /// </summary>
        public DataPaging()
        {
            InitializeComponent();
            EnableDisableButtons(this);
            this.DataContext = this;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value of AvailablePagesCount
        /// </summary>
        public int AvailablePagesCount
        {
            get { return this.availablePagesCount; }
            set
            {
                this.availablePagesCount = value;
                if (this.availablePagesCount < 1)
                    this.availablePagesCount = 1;
            }
        }

        /// <summary>
        /// Gets a value indicate if the previous button is enable.
        /// </summary>
        public bool IsPreviousEnable { get { return this.btnPrev.IsEnabled; } }

        /// <summary>
        /// Gets a value indicate if the next button is enable
        /// </summary>
        public bool IsNextEnable { get { return this.btnNext.IsEnabled; } }

        /// <summary>
        /// Gets AvailablePages values
        /// </summary>
        public List<int> AvailablePages
        {
            get
            {
                return (this.listAvailablePage.Items) != null ? this.listAvailablePage.Items.Cast<int>().ToList() : new List<int>();
            }
        }

        /// <summary>
        /// Page Count from Dependency property 
        /// </summary>
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set
            {
                SetValue(PageCountProperty, value);
            }
        }

        /// <summary>
        ///  Current Page from Dependency property 
        /// </summary>
        public int PageNumber
        {
            get { return (int)GetValue(PageNumberProperty); }
            set { SetValue(PageNumberProperty, value); }
        }

        /// <summary>
        ///  Page Size from Dependency property 
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PageNumber" /> dependency property
        /// </summary>
        public static DependencyProperty PageNumberProperty = DependencyProperty.Register("PageNumber", typeof(int), typeof(DataPaging), new UIPropertyMetadata(1,
                                                                                                                                     OnPageNumberChanged));
        /// <summary>
        /// Identifies the <see cref="PageSize" /> dependency property
        /// </summary>
        public static DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(int), typeof(DataPaging), new UIPropertyMetadata(10,
                                                                                                                                     OnPageSizeChanged));
        /// <summary>
        /// Identifies the <see cref="PageCount" /> dependency property
        /// </summary>
        public static DependencyProperty PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(DataPaging), new UIPropertyMetadata(0,
                                                                                                                                  OnPageCountChanged));
        /// <summary>
        /// Identifies the <see cref="PageChangedCommand" /> dependency property
        /// </summary>
        public static DependencyProperty PageChangedCommandProperty = DependencyProperty.Register("PageChangedCommand", typeof(DelegateCommand), typeof(DataPaging));

        /// <summary>
        /// Delegate Command for Page Changed event
        /// </summary>
        public DelegateCommand PageChangedCommand
        {
            get { return (DelegateCommand)GetValue(PageChangedCommandProperty); }
            set { SetValue(PageChangedCommandProperty, value); }
        }

        #endregion

        #region Private & Protected methods

        /// <summary>
        /// generate page items
        /// </summary>
        private static void SetAvailablePages(DataPaging d)
        {
            if (d.PageNumber <= 0)
                return;
            int start = 1;
            int max = d.PageNumber;
            int x = d.PageNumber % d.AvailablePagesCount;
            int y = d.PageNumber / d.AvailablePagesCount;
            if (y > 0 && x > 0)
            {
                max = Math.Min(((y + 1) * d.AvailablePagesCount), d.PageCount);
                start = y * d.AvailablePagesCount + 1;
            }
            if (y == 0)
            {
                max = Math.Min(d.AvailablePagesCount, d.PageCount);
            }
            if (x == 0)
            {
                start = (y - 1) * d.AvailablePagesCount + 1;
            }

            d.listAvailablePage.Items.Clear();
            for (int i = start; i <= max; i++)
            {
                d.listAvailablePage.Items.Add(i);
            }
            d.listAvailablePage.SelectedValue = d.PageNumber;
        }

        /// <summary>
        /// Page Count Changed Event 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPageCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPaging objGrid = d as DataPaging;
            EnableDisableButtons(objGrid);
            SetAvailablePages(objGrid);
        }

        /// <summary>
        /// Current Page Changed Event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPageNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPaging objGrid = d as DataPaging;
            EnableDisableButtons(objGrid);
            SetAvailablePages(objGrid);
        }

        /// <summary>
        /// Page Size Changed Event
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPaging objGrid = d as DataPaging;
            EnableDisableButtons(objGrid);
            SetAvailablePages(objGrid);
        }

        /// <summary>
        /// Common Function to enable / disable paging buttons
        /// </summary>
        /// <param name="objGrid"></param>
        protected static void EnableDisableButtons(DataPaging objGrid)
        {
            if (null != objGrid.btnNext)
            {
                objGrid.btnNext.IsEnabled = true;
                objGrid.btnPrev.IsEnabled = true;
                if (objGrid.PageNumber <= 1)
                    objGrid.btnPrev.IsEnabled = false;
                if (objGrid.PageNumber >= objGrid.PageCount)
                    objGrid.btnNext.IsEnabled = false;
            }
        }

        /// <summary>
        /// Previous page button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GotoPreviousPage(object sender, RoutedEventArgs e)
        {
            if (PageCount > 0)
            {
                PageNumber = (PageNumber - 1) < 1 ? 1 : PageNumber - 1;
            }
            RaisePageChangedCommand();
            EnableDisableButtons(this);
        }

        /// <summary>
        /// Next page button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GotoNextPage(object sender, RoutedEventArgs e)
        {
            if (PageCount > 0)
            {
                PageNumber = (PageNumber + 1) > PageCount ? PageCount : PageNumber + 1;
            }
            RaisePageChangedCommand();
            EnableDisableButtons(this);
        }

        private void RaisePageChangedCommand()
        {
            if (PageChangedCommand != null)
            {
                PageChangedCommand.Execute();
            }
        }

        private void GotoPage_Click(object sender, MouseButtonEventArgs e)
        {
            ContentPresenter cp = sender as ContentPresenter;
            int page;
            if (int.TryParse(cp.Content.ToString(),out page))
            {
                PageNumber = page <= 0 ? 1 : page;
                RaisePageChangedCommand();
            }
        }
        #endregion


    }
}
