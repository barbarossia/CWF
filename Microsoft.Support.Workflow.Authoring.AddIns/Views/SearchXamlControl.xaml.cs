using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Microsoft.Support.Workflow.Authoring.AddIns.Views
{
    /// <summary>
    /// Interaction logic for SearchXamlControl.xaml
    /// </summary>
    public partial class SearchXamlControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void notifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Properties

        public string SearchText
        {
            get
            {
                return (string)this.GetValue(SearchTextProperty);
            }
            set
            {
                this.SetValue(SearchTextProperty, value);
            }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
   "SearchText", typeof(string), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public FlowDocument Document
        {
            get
            {
                return (FlowDocument)this.GetValue(DocumentProperty);
            }
            set
            {
                this.SetValue(DocumentProperty, value);
            }
        }
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
   "Document", typeof(FlowDocument), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public Brush TextBoxBackgroundBrush
        {
            get
            {
                return (Brush)this.GetValue(TextBoxBackgroundBrushProperty);
            }
            set
            {
                this.SetValue(TextBoxBackgroundBrushProperty, value);
            }
        }
        public static readonly DependencyProperty TextBoxBackgroundBrushProperty = DependencyProperty.Register(
   "TextBoxBackgroundBrush", typeof(Brush), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public Brush BackgroundTextBrush
        {
            get
            {
                return (Brush)this.GetValue(BackgroundTextBrushProperty);
            }
            set
            {
                this.SetValue(BackgroundTextBrushProperty, value);
            }
        }
        public static readonly DependencyProperty BackgroundTextBrushProperty = DependencyProperty.Register(
   "BackgroundTextBrush", typeof(Brush), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public string BackgroundText
        {
            get
            {
                return (string)this.GetValue(BackgroundTextProperty);
            }
            set
            {
                this.SetValue(BackgroundTextProperty, value);
            }
        }
        public static readonly DependencyProperty BackgroundTextProperty = DependencyProperty.Register(
   "BackgroundText", typeof(string), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public string NextButtonHintText
        {
            get
            {
                return (string)this.GetValue(NextButtonHintTextProperty);
            }
            set
            {
                this.SetValue(NextButtonHintTextProperty, value);
            }
        }
        public static readonly DependencyProperty NextButtonHintTextProperty = DependencyProperty.Register(
   "NextButtonHintText", typeof(string), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public string PreviousButtonHintText
        {
            get
            {
                return (string)this.GetValue(PreviousButtonHintTextProperty);
            }
            set
            {
                this.SetValue(PreviousButtonHintTextProperty, value);
            }
        }
        public static readonly DependencyProperty PreviousButtonHintTextProperty = DependencyProperty.Register(
   "PreviousButtonHintText", typeof(string), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public Brush HighlightedTextBrush
        {
            get
            {
                return (Brush)this.GetValue(HighlightedTextBrushProperty);
            }
            set
            {
                this.SetValue(HighlightedTextBrushProperty, value);
            }
        }
        public static readonly DependencyProperty HighlightedTextBrushProperty = DependencyProperty.Register(
   "HighlightedTextBrush", typeof(Brush), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public Brush HighlightedBackgroundBrush
        {
            get
            {
                return (Brush)this.GetValue(HighlightedBackgroundBrushProperty);
            }
            set
            {
                this.SetValue(HighlightedBackgroundBrushProperty, value);
            }
        }

        public static readonly DependencyProperty HighlightedBackgroundBrushProperty = DependencyProperty.Register(
   "HighlightedBackgroundBrush", typeof(Brush), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        public Brush NextPreviousButtonsForegroundBrush
        {
            get
            {
                return (Brush)this.GetValue(NextPreviousButtonsForegroundBrushProperty);
            }
            set
            {
                this.SetValue(NextPreviousButtonsForegroundBrushProperty, value);
            }
        }
        public static readonly DependencyProperty NextPreviousButtonsForegroundBrushProperty = DependencyProperty.Register(
   "NextPreviousButtonsForegroundBrush", typeof(Brush), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        private int currentFindedPatternPosition;
        private List<TextRange> findedPatterns;
        private DependencyObject highlightedParagraph;
        private string filterText = "";


        public Action RefreshHighLightedWordsHandler
        {
            get
            {
                return (Action)this.GetValue(RefreshHighLightedWordsHandlerProperty);
            }
            set
            {
                this.SetValue(RefreshHighLightedWordsHandlerProperty, value);
            }
        }
        public static readonly DependencyProperty RefreshHighLightedWordsHandlerProperty = DependencyProperty.Register(
   "RefreshHighLightedWordsHandler", typeof(Action), typeof(SearchXamlControl), new FrameworkPropertyMetadata());

        #endregion

        public SearchXamlControl()
        {
            InitializeComponent();
        }

        #region Methods

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "SearchText")
            {
                if (SearchText != null && SearchText.Length > 0)
                {
                    //FindButton.IsChecked = true;
                    setFindTextBoxVisibilty();
                    findTextBox.Text = SearchText;
                    DoSearch();
                }
            }
        }

        private void setFindTextBoxVisibilty()
        {
            SearchText = "";
            filterText = "";
            currentFindedPatternPosition = -1;
            findedPatterns = null;
            findTextBox.Focus();
            CreateButtonToolTips();
            notifyPropertyChanged("RefreshSearchHandler");
            EnableOrDisableBtns(false, false);
            if (Document != null)
            {
                FlowDocumentExtension.ClearTextDecorations(Document);
            }
        }

        private void CreateButtonToolTips()
        {
            if (NextButtonHintText != null && NextButtonHintText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                TextBlock textBlock = new TextBlock();
                textBlock.Text = NextButtonHintText;
                tooltip.Content = textBlock;
                nextButton.ToolTip = tooltip;
            }

            if (PreviousButtonHintText != null && PreviousButtonHintText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                TextBlock textBlock = new TextBlock();
                textBlock.Text = PreviousButtonHintText;
                tooltip.Content = textBlock;
                previousButton.ToolTip = tooltip;
            }
        }

        private void HighLightCurrentPattern()
        {
            if (Document != null)
                FlowDocumentExtension.ClearTextDecorations(Document);
            if (findedPatterns[currentFindedPatternPosition] != null)
                if (this.Document != null && this.Document.Parent is RichTextBox)
                    FlowDocumentExtension.HighLightText(Document.Parent as RichTextBox, findedPatterns[currentFindedPatternPosition], HighlightedTextBrush, HighlightedBackgroundBrush);
            EnableOrDisableBtns(currentFindedPatternPosition > 0, currentFindedPatternPosition < findedPatterns.Count - 1);
        }

        private void EnableOrDisableBtns(bool perviousBtn, bool nextBtn)
        {
            previousButton.IsEnabled = perviousBtn;
            nextButton.IsEnabled = nextBtn;
        }

        private void BringIntoViewNextPattern()
        {
            if (currentFindedPatternPosition >= findedPatterns.Count - 1)
            {
                TextPointer lastPointer = findedPatterns.Last().End;
                var pattern = FlowDocumentExtension.GetNextMatching(Document, filterText, lastPointer);
                if (pattern != null)
                {
                    findedPatterns.Add(pattern);
                }
            }

            if (findedPatterns == null || findedPatterns.Count == 0)
            {
                EnableOrDisableBtns(false, false);
                return;
            }
            if (findedPatterns.Count <= currentFindedPatternPosition)
            {
                currentFindedPatternPosition = findedPatterns.Count - 1;
            }
            HighLightCurrentPattern();
        }

        private void BringIntoViewPreviousPattern()
        {
            currentFindedPatternPosition--;
            if (findedPatterns == null || currentFindedPatternPosition < 0)
            {
                currentFindedPatternPosition = 0;
                return;
            }

            HighLightCurrentPattern();

        }

        private void DoSearch()
        {
            if (Document == null)
            {
                return;
            }

            if ((filterText == findTextBox.Text && findedPatterns != null))
            {
                BringIntoViewNextPattern();
                return;
            }
            //new search
            if (filterText != findTextBox.Text)
            {
                FlowDocumentExtension.ClearTextDecorations(Document);
                EnableOrDisableBtns(false, false);
            }
            filterText = findTextBox.Text;

            findedPatterns = new List<TextRange>();
            var pattern = FlowDocumentExtension.GetNextMatching(Document, filterText, null);
            if (pattern != null)
            {
                findedPatterns.Add(pattern);
                currentFindedPatternPosition = 0;
                BringIntoViewNextPattern();
            }
        }

        #endregion

        #region Events

        private void FindTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.findTextBox.Text))
            {
                setFindTextBoxVisibilty();
            }
            else
            {
                DoSearch();
            }

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (filterText != findTextBox.Text)
            {
                DoSearch();
            }
            else
            {
                currentFindedPatternPosition++;
                BringIntoViewNextPattern();
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (filterText != findTextBox.Text)
            {
                DoSearch();
            }
            else
            {
                BringIntoViewPreviousPattern();
            }
        }

        #endregion
    }
}
