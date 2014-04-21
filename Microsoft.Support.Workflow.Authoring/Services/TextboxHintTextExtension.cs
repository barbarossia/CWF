

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    public class TextboxHintTextExtension : Adorner
    {
        #region Dependency Property
        
        /// <summary>
        /// Text attached property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(TextboxHintTextExtension),
            new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        #endregion


        # region Property

        //Indicated the place holder text is visible
        private bool isVisible;

        #endregion


        #region Constructors

        //Initializes the static number
        static TextboxHintTextExtension()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(TextboxHintTextExtension), new FrameworkPropertyMetadata(false));
            ClipToBoundsProperty.OverrideMetadata(typeof(TextboxHintTextExtension), new FrameworkPropertyMetadata(true));
        }

        public TextboxHintTextExtension() : base(new TextBox()) { }

        public TextboxHintTextExtension(TextBox textbox, string hintText)
            : base(textbox)
        {
            textbox.GotFocus += TextboxGotFocus;
            textbox.LostFocus += TextboxLostFocus;
            textbox.TextChanged += TextboxContentChanged;
            if (!string.IsNullOrEmpty(hintText))
                textbox.SetValue(TextProperty, hintText);
        }

       
        #endregion

        #region Event Handlers     

        /// <summary>
        /// Draws the content during the render pass of element
        /// </summary>
        /// <param name="drawingContext">object to draw</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var textbox = AdornedElement as TextBox;
            if (textbox == null || textbox.IsFocused || !string.IsNullOrEmpty(textbox.Text) || string.IsNullOrEmpty((string)textbox.GetValue(TextProperty)))
                isVisible = false;
            else
            {
                isVisible = true;
                var formattedText = new FormattedText(
                       (string)textbox.GetValue(TextProperty),
                       CultureInfo.CurrentCulture,
                       textbox.FlowDirection,
                       new Typeface(textbox.FontFamily, FontStyles.Italic, textbox.FontWeight, textbox.FontStretch),
                       textbox.FontSize,
                       Brushes.Gray);

                formattedText.TextAlignment = TextAlignment.Left;
                formattedText.MaxTextHeight = textbox.RenderSize.Height - 2.0;
                formattedText.MaxTextWidth = textbox.RenderSize.Width - 8.0;
                drawingContext.DrawText(formattedText, new Point(5.0, 5.0));
            }
        }

        /// <summary>
        /// Invoke whenever text attached property is changed
        /// </summary>
        /// <param name="sender">The object the event handler is attached</param>
        /// <param name="e">Data of the event</param>
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue.ToString() != e.NewValue.ToString())
            {
                var textbox = sender as TextBox;
                var adornerLayer = AdornerLayer.GetAdornerLayer(textbox);
                if (adornerLayer != null)
                    adornerLayer.Add(new TextboxHintTextExtension(textbox, string.Empty));
            }
        }
  
        /// <summary>
        /// Event handler for GotFocus
        /// </summary>
        private void TextboxGotFocus(object sender, RoutedEventArgs e)
        {
            if (isVisible)
                InvalidateVisual();
        }

        /// <summary>
        /// Event handler for LostFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">data about event</param>
        private void TextboxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!isVisible && string.IsNullOrEmpty((sender as TextBox).Text))
                InvalidateVisual();
        }

        /// <summary>
        /// Event handler for ContentChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">data about event</param>
        private void TextboxContentChanged(object sender, RoutedEventArgs e)
        {
            if (isVisible ^ string.IsNullOrEmpty((sender as TextBox).Text))
                InvalidateVisual();
        }

        #endregion
       
    }
}
