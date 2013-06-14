using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// View model for PrintCustomization window
    /// </summary>
    public class PrintCustomizationViewModel : NotificationObject
    {
        private bool isSettingEnabled = true;
        private PrintQueue currentPrinter;
        private PaperSize paperSize;
        private PageOrientation pageOrientation;
        private PrintViewMode viewMode;
        private bool includeXaml = true;
        private DelegateCommand openPrinterOption;
        private DelegateCommand print;
        private DelegateCommand back;

        private bool isPrinting;
        private int elementCount;
        private int loadedElementCount;
        private int horizontalPageCount;
        private int verticalPageCount;
        private double pageHeight;
        private double pageWidth;

        private double hardMarginX
        {
            get { return PrintHelper.Printers[CurrentPrinter].DefaultPageSettings.HardMarginX * PrintHelper.DPI; }
        }
        private double hardMarginY
        {
            get { return PrintHelper.Printers[CurrentPrinter].DefaultPageSettings.HardMarginY * PrintHelper.DPI; }
        }
        /// <summary>
        /// Helper for elements dragging
        /// </summary>
        public DraggingWidgetHelper DraggingHelper { get; private set; }
        /// <summary>
        /// Helper for print
        /// </summary>
        public PrintHelper PrintHelper { get; private set; }
        /// <summary>
        /// PrintCustomization window
        /// </summary>
        public Window View { get; set; }

        /// <summary>
        /// Is settings available
        /// </summary>
        public bool IsSettingEnabled
        {
            get { return isSettingEnabled; }
            set
            {
                isSettingEnabled = value;
                RaisePropertyChanged(() => IsSettingEnabled);
            }
        }
        /// <summary>
        /// Selected printer
        /// </summary>
        public PrintQueue CurrentPrinter
        {
            get { return currentPrinter; }
            set
            {
                currentPrinter = value;
                RaisePropertyChanged(() => CurrentPrinter);

                OnPrintOptionsChanged();
                OpenPrinterOption.RaiseCanExecuteChanged();
                Print.RaiseCanExecuteChanged();
            }
        }
        /// <summary>
        /// Selected paper size
        /// </summary>
        public PaperSize PaperSize
        {
            get { return paperSize; }
            set
            {
                paperSize = value;
                RaisePropertyChanged(() => PaperSize);
            }
        }
        /// <summary>
        /// Selected page orientation
        /// </summary>
        public PageOrientation PageOrientation
        {
            get { return pageOrientation; }
            set
            {
                pageOrientation = value;
                RaisePropertyChanged(() => PageOrientation);
            }
        }
        /// <summary>
        /// Selected view mode
        /// </summary>
        public PrintViewMode ViewMode
        {
            get { return viewMode; }
            set
            {
                viewMode = value;
                RaisePropertyChanged(() => ViewMode);

                RefreshPanelScale();
            }
        }
        /// <summary>
        /// Available view modes
        /// </summary>
        public PrintViewModeOnUI[] ViewModes
        {
            get
            {
                return ((PrintViewMode[])Enum.GetValues(typeof(PrintViewMode))).Select(m => new PrintViewModeOnUI(m)).ToArray();
            }
        }
        /// <summary>
        /// If xaml should be included
        /// </summary>
        public bool IncludeXaml
        {
            get { return includeXaml; }
            set
            {
                includeXaml = value;
                RaisePropertyChanged(() => IncludeXaml);

                if (IsSettingEnabled)
                    SetPanelSize(false);
            }
        }
        /// <summary>
        /// Command for opening printer option
        /// </summary>
        public DelegateCommand OpenPrinterOption
        {
            get
            {
                if (openPrinterOption == null)
                {
                    openPrinterOption = new DelegateCommand(() =>
                    {
                        try
                        {
                            if (PrintHelper.OpenPrintOptions(View, CurrentPrinter))
                            {
                                OnPrintOptionsChanged();
                            }
                        }
                        catch (Exception ex)
                        {
                            AddInMessageBoxService.PrintFailed(ex.Message);
                        }
                    }, () => { return IsSettingEnabled && CurrentPrinter != null; });
                }
                return openPrinterOption;
            }
        }
        /// <summary>
        /// Command for printing
        /// </summary>
        public DelegateCommand Print
        {
            get
            {
                if (print == null)
                {
                    print = new DelegateCommand(() =>
                    {
                        if (!isPrinting && AddInMessageBoxService.PrintConfirmation(horizontalPageCount * verticalPageCount, PaperSize.PaperName, CurrentPrinter.FullName))
                        {
                            isPrinting = true;
                            Print.RaiseCanExecuteChanged();
                            IsSettingEnabled = false;
                            OpenPrinterOption.RaiseCanExecuteChanged();
                            RemovePageGrid();
                            DraggingHelper.Close();

                            try
                            {
                                List<Rectangle> rects = new List<Rectangle>();
                                for (int j = 0; j < verticalPageCount; j++)
                                {
                                    for (int i = 0; i < horizontalPageCount; i++)
                                    {
                                        VisualBrush brush = new VisualBrush(DraggingHelper.Panel);
                                        brush.Viewbox = new Rect((double)i / horizontalPageCount, (double)j / verticalPageCount, 1.0 / horizontalPageCount, 1.0 / verticalPageCount);

                                        TransformGroup group = new TransformGroup();
                                        if (PageOrientation == PageOrientation.Landscape)
                                        {
                                            group.Children.Add(new RotateTransform(90));
                                            group.Children.Add(new TranslateTransform(pageHeight, 0));
                                        }
                                        group.Children.Add(new TranslateTransform(hardMarginX, hardMarginY));
                                        Rectangle rect = new Rectangle()
                                        {
                                            Height = pageHeight,
                                            Width = pageWidth,
                                            RenderTransform = group,
                                            Fill = brush
                                        };
                                        rects.Add(rect);
                                    }
                                }
                                PrintHelper.Print(CurrentPrinter, rects);
                            }
                            catch (Exception ex)
                            {
                                AddInMessageBoxService.PrintFailed(ex.Message);
                            }

                            BuildPageGrid();
                            isPrinting = false;
                            Print.RaiseCanExecuteChanged();
                        }
                    }, () => { return CurrentPrinter != null && !isPrinting; });
                }
                return print;
            }
        }
        /// <summary>
        /// Command for back
        /// </summary>
        public DelegateCommand Back
        {
            get
            {
                if (back == null)
                {
                    back = new DelegateCommand(() =>
                    {
                        if (!IsSettingEnabled || AddInMessageBoxService.PrintReselectConfirmation())
                        {
                            View.Close();
                        }
                    });
                }
                return back;
            }
        }

        /// <summary>
        /// Initialize the view model
        /// </summary>
        public PrintCustomizationViewModel()
        {
            PrintHelper = CreatePrintHelperFunc();
        }

        public static Func<PrintHelper> CreatePrintHelperFunc = () =>
        {
            return new PrintHelper();
        };

        /// <summary>
        /// Initialize the dragging function
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="designers"></param>
        public void InitializeDragging(Canvas panel, List<ActivityDesigner> designers)
        {
            if (SynchronizationContext.Current != null && !panel.IsLoaded)
            {
                throw new ApplicationException("Print page container has not loaded.");
            }

            DraggingHelper = new DraggingWidgetHelper(panel);
            DraggingHelper.WidgetDragged += OnWidgetDragged;

            BooleanToVisibilityConverter converter = new BooleanToVisibilityConverter();
            Binding binding = new Binding("IncludeXaml");
            binding.Mode = BindingMode.TwoWay;
            binding.Converter = converter;

            foreach (ActivityDesigner d in designers)
            {
                Rectangle rect = new Rectangle()
                {
                    Height = d.ActualHeight,
                    Width = d.ActualWidth,
                    Fill = new VisualBrush(d)
                    {
                        Viewbox = new Rect(0, 0, 1, 1)
                    },
                };
                Border border = new Border()
                {
                    Child = new TextBlock()
                    {
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Text = XamlService.SerializeToXamlWithCleanup(d.ModelItem.GetActivity())
                    }
                };
                border.SetBinding(FrameworkElement.VisibilityProperty, binding);

                rect.Loaded += OnWidgetLoaded;
                border.Loaded += OnWidgetLoaded;

                panel.Children.Add(rect);
                panel.Children.Add(border);

                DraggingHelper.AddWidget(rect);
                DraggingHelper.AddWidget(border);
            }
            elementCount = panel.Children.Count;

            CurrentPrinter = PrintHelper.Printers.FirstOrDefault(p => p.Value.IsDefaultPrinter).Key;
        }

        /// <summary>
        /// Refresh the scale of page container
        /// </summary>
        public void RefreshPanelScale()
        {
            if (DraggingHelper != null)
            {
                double scale = 1;
                if (ViewMode == PrintViewMode.FitToWindow)
                {
                    DependencyObject obj = DraggingHelper.Panel;
                    do
                    {
                        obj = VisualTreeHelper.GetParent(obj);
                    } while (obj != null && !(obj is ScrollViewer));
                    ScrollViewer sv = (ScrollViewer)obj;
                    double horizontalMargin = DraggingHelper.Panel.Margin.Left + DraggingHelper.Panel.Margin.Right;
                    double verticalMargin = DraggingHelper.Panel.Margin.Top + DraggingHelper.Panel.Margin.Bottom;
                    double scaleX = (sv.ActualWidth - horizontalMargin) / DraggingHelper.Panel.Width;
                    double scaleY = (sv.ActualHeight - verticalMargin) / DraggingHelper.Panel.Height;
                    scale = Math.Min(scaleX, scaleY);
                }
                DraggingHelper.Panel.LayoutTransform = new ScaleTransform(scale, scale);
            }
        }

        public void OnWidgetLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= OnWidgetLoaded;

            loadedElementCount++;
            if (loadedElementCount == elementCount)
            {
                InitElementPosition();
                SetPanelSize(false);
            }
        }

        private void OnPrintOptionsChanged()
        {
            PaperSize = PrintHelper.GetPaperSize(CurrentPrinter);
            PageOrientation = PrintHelper.GetPageOrientation(CurrentPrinter);

            if (DraggingHelper != null)
            {
                PageMediaSize size = PrintHelper.GetPageMediaSize(CurrentPrinter);
                if (PageOrientation == PageOrientation.Portrait)
                {
                    pageHeight = size.Height.Value - hardMarginY * 2;
                    pageWidth = size.Width.Value - hardMarginX * 2;
                }
                else if (PageOrientation == PageOrientation.Landscape)
                {
                    pageHeight = size.Width.Value - hardMarginX * 2;
                    pageWidth = size.Height.Value - hardMarginY * 2;
                }
                SetPanelSize(true);
            }

            RefreshPanelScale();
        }

        private void OnWidgetDragged(object sender, EventArgs e)
        {
            SetPanelSize(false);
        }

        private void InitElementPosition()
        {
            const int margin = 50;
            double totalHeight = 0;
            for (int i = 0; i < DraggingHelper.ElementsRightBottom.Count; i++)
            {
                FrameworkElement element = DraggingHelper.ElementsRightBottom.Keys.Skip(i).First();

                if (element.IsActivity() && (i > 0))
                {
                    totalHeight += margin;
                }
                else if (!element.IsActivity() && (element.ActualWidth > pageWidth))
                {
                    element.Width = pageWidth;
                    element.UpdateLayout();
                }

                Canvas.SetLeft(element, 0);
                Canvas.SetTop(element, totalHeight);

                totalHeight += element.ActualHeight;
                DraggingHelper.ElementsRightBottom[element] = new Point(element.ActualWidth, totalHeight);
            }
        }

        private void SetPanelSize(bool forceRefresh)
        {
            int horizontalCount = 1;
            int verticalCount = 1;
            foreach (var p in DraggingHelper.ElementsRightBottom)
            {
                if (p.Key.Visibility == Visibility.Visible)
                {
                    horizontalCount = Math.Max((int)Math.Ceiling(p.Value.X / pageWidth), horizontalCount);
                    verticalCount = Math.Max((int)Math.Ceiling(p.Value.Y / pageHeight), verticalCount);
                }
            }

            if (forceRefresh || horizontalPageCount != horizontalCount || verticalPageCount != verticalCount)
            {
                horizontalPageCount = horizontalCount;
                verticalPageCount = verticalCount;
                DraggingHelper.Panel.Width = pageWidth * horizontalCount;
                DraggingHelper.Panel.Height = pageHeight * verticalCount;

                RemovePageGrid();
                BuildPageGrid();
                RefreshPanelScale();
            }
        }

        private void BuildPageGrid()
        {
            for (int j = 0; j < verticalPageCount; j++)
            {
                for (int i = 0; i < horizontalPageCount; i++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Height = pageHeight,
                        Width = pageWidth,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection() { 2, 2 },
                    };
                    DraggingHelper.Panel.Children.Add(rect);
                    Canvas.SetTop(rect, j * pageHeight);
                    Canvas.SetLeft(rect, i * pageWidth);
                }
            }
        }

        private void RemovePageGrid()
        {
            DraggingHelper.Panel.Children.RemoveRange(elementCount,
                DraggingHelper.Panel.Children.Count - elementCount);
        }
    }
}
