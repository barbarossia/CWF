using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static partial class FlowDocumentExtension
    {
        private static Color highlightColor = Color.FromArgb(125, 51, 153, 255);
        private static SolidColorBrush highlightBrush = new SolidColorBrush(highlightColor);
        private static Run run;
        private static TextPointer end;
        #region Search in FlowDocuments


        public static void ClearTextDecorations(DependencyObject document)
        {
            var flow = document as FlowDocument;
            var txt = flow.Parent as RichTextBox;
            var textrange = txt.Selection;
            textrange.Select(flow.ContentStart, flow.ContentEnd);
            textrange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
            textrange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
        }

        public static void HighLightText(RichTextBox txt, TextRange range, Brush brush, Brush background)
        {
            var textrange = txt.Selection;
            try
            {
                textrange.Select(range.Start, range.End);
            }
            catch (ArgumentException)
            {
                return;
            }
            textrange.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            textrange.ApplyPropertyValue(TextElement.BackgroundProperty, background);
            ScrollIfNeeded(txt, range.Start, range.End);
        }

        public static TextRange GetNextMatching(DependencyObject document, string pattern, TextPointer currentPointer)
        {
            TextPointer start = currentPointer;

            if (run == null || currentPointer == null)
            {
                run = LogicalTreeUtility.GetChildren<Run>(document, true).Single();
                end = run.ContentEnd;
                start = run.ContentStart;
            }

            return GetNextMatching(start, end, pattern, notIgnoreConnectedChars: false);
        }

        public static TextRange GetNextMatching(TextPointer currentPointer, TextPointer endPointer, string pattern, bool notIgnoreConnectedChars)
        {
            int actualPatternLength;

            TextPointer nextPointer = null;
            actualPatternLength = pattern.Length;

            while (currentPointer != null && currentPointer.CompareTo(endPointer) < 0)
            {
                if (currentPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = currentPointer.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
                    if (indexInRun >= 0)
                    {
                        nextPointer = currentPointer.GetPositionAtOffset(indexInRun);
                        break;
                    }
                }

                currentPointer = currentPointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            if (nextPointer == null)
            {
                return null;
            }

            TextPointer endMachingPosition = nextPointer.GetPositionAtOffset(actualPatternLength, LogicalDirection.Forward);

            return new TextRange(nextPointer, endMachingPosition);
        }

        private static void ClearSelction(TextSelection selection)
        {
            selection.ApplyPropertyValue(TextElement.BackgroundProperty, null);
        }

        public static void ClearSelction(this RichTextBox rtfControl, int offset, int length)
        {
            var textRange = rtfControl.Selection;
            var start = rtfControl.Document.ContentStart;

            TextPointer startPos;
            if (offset == 0)
                startPos = start.GetPositionAtOffset(offset + 2);
            else
                startPos = start.GetPositionAtOffset(offset + 4);
            TextPointer endPos = startPos.GetPositionAtOffset(length);
            if (endPos == null)
                endPos = rtfControl.Document.ContentEnd;
            textRange.Select(startPos, endPos);
            ClearSelction(textRange);
        }

        public static void HighlightSelection(this RichTextBox rtfControl, int offset, int length)
        {
            var textRange = rtfControl.Selection;
            ClearSelction(textRange);
            var start = rtfControl.Document.ContentStart;

            var startPos = start.GetPositionAtOffset(offset + 2);
            var endPos = startPos.GetPositionAtOffset(length);

            if (endPos == null)
                endPos = rtfControl.Document.ContentEnd;
            textRange.Select(startPos, endPos);
            SetSelectionColor(textRange);
            rtfControl.ScrollIfNeeded(startPos, endPos);
        }

        private static void SetSelectionColor(TextSelection textRange)
        {
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, highlightBrush);
        }

        private static void ScrollIfNeeded(this RichTextBox textBox, TextPointer startPos, TextPointer endPos)
        {
            var start = startPos.GetCharacterRect(LogicalDirection.Forward);
            var end = endPos.GetCharacterRect(LogicalDirection.Forward);
            if (start != Rect.Empty && end != Rect.Empty)
            {
                textBox.ScrollToVerticalOffset((start.Top - 20) + textBox.VerticalOffset);
            }
        }


        #endregion

    }
}
