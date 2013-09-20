using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Views
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly RoutedEvent DocumentChangedEvent =
            EventManager.RegisterRoutedEvent("DocumentChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RichTextBoxHelper));

        public static void AddDocumentChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(RichTextBoxHelper.DocumentChangedEvent, handler);
            }
        }

        public static void RemoveDocumentChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(RichTextBoxHelper.DocumentChangedEvent, handler);
            }
        }


        public static readonly DependencyProperty DocumentXamlProperty =
              DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox)obj;

                        // Parse the XAML to a document (or use XamlReader.Parse())
                        var xaml = GetDocumentXaml(richTextBox);
                        var doc = new FlowDocument();
                        //var range = new TextRange(doc.ContentStart, doc.ContentEnd);

                        //range.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)),
                        //  DataFormats.Text);
                        doc.Blocks.Add(new Paragraph(new Run(xaml)));
                        richTextBox.Document = doc;

                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        richTextBox.RaiseEvent(new RoutedEventArgs(RichTextBoxHelper.DocumentChangedEvent, richTextBox));

                        ////// When the document changes update the source
                        //range.Changed += (obj2, e2) =>
                        //{
                        //    if (richTextBox.Document == doc)
                        //    {
                        //        MemoryStream buffer = new MemoryStream();
                        //        range.Save(buffer, DataFormats.Xaml);
                        //        SetDocumentXaml(richTextBox,
                        //          Encoding.UTF8.GetString(buffer.ToArray()));
                        //        richTextBox.RaiseEvent(new RoutedEventArgs(RichTextBoxHelper.DocumentChangedEvent, richTextBox));
                        //    }
                        //};
                    }
                });
    }
}
