using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Behaviors {
    /// <summary>
    /// This class is used to apply filters to TextBox such as preventing special characters.
    /// </summary>
    public static partial class TextBoxFilters
    {
        #region Private Fields

        // List of allowed keys. Put them here if you want to allow that key to pressed
        private static readonly List<Key> controlKeys = new List<Key>
                                                            {
                                                                Key.Back,
                                                                Key.CapsLock,
                                                                Key.Down,
                                                                Key.End,
                                                                Key.Enter,
                                                                Key.Escape,
                                                                Key.Home,
                                                                Key.Insert,
                                                                Key.Left,
                                                                Key.PageDown,
                                                                Key.PageUp,
                                                                Key.Right,
                                                                Key.LeftShift,
                                                                Key.RightShift,
                                                                Key.Tab,
                                                                Key.Up,
                                                            };

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies pasting text with Alphanumeric regular expression and allows if it is alphanumeric
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DataObjectPastingEventArgs</param>
        private static void CancelCommand(object sender, DataObjectPastingEventArgs e)
        {
            bool isAlphaNumeric = false;
            string AlphaNumberRegEx = "^[a-zA-Z0-9]+$"; // Regular expression to check AlphaNumeric values
            string value = string.Empty;
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                // Get the copied value to the clipboard
                value = e.DataObject.GetData(typeof(string)).ToString();

                // Remove spaces in the text so that we allow text with spaces too
                // But the actual data is pasted with spaces
                value = value.Replace(TextResources.Space, string.Empty).Trim();

                // Verify with Alphanumeric Regular expression
                isAlphaNumeric = System.Text.RegularExpressions.Regex.IsMatch(value, AlphaNumberRegEx);
            }

            if (!isAlphaNumeric)
            {
                // Dont allow to paste
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Verifies pasting text with Alphanumeric regular expression and allows if it is alphanumeric
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DataObjectPastingEventArgs</param>
        private static void NumericCancelCommand(object sender, DataObjectPastingEventArgs e)
        {
            bool isAlphaNumeric = false;
            string AlphaNumberRegEx = "^[0-9]+$"; // Regular expression to check AlphaNumeric values
            string value = string.Empty;
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                // Get the copied value to the clipboard
                value = e.DataObject.GetData(typeof(string)).ToString();

                // Verify with Alphanumeric Regular expression
                isAlphaNumeric = System.Text.RegularExpressions.Regex.IsMatch(value, AlphaNumberRegEx);
            }

            if (!isAlphaNumeric)
            {
                // Dont allow to paste
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Verifies pressed key is a Digit or not
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if it is Digit</returns>
        private static bool IsDigit(Key key)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool retVal;
            if (key >= Key.D0 && key <= Key.D9 && !shiftKey)
            {
                retVal = true;
            }
            else
            {
                retVal = key >= Key.NumPad0 && key <= Key.NumPad9;
            }
            return retVal;
        }

        /// <summary>
        /// Verifies pressed key is a valid letter or not
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if it is valid letter</returns>
        private static bool IsLetter(Key key)
        {
            bool retVal = false;
            if (key >= Key.A && key <= Key.Z)
            {
                retVal = true;
            }
            return retVal;
        }

        private static void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            
            bool handled = true;

            // Check if it is valid key, digit, letter
            if (controlKeys.Contains(e.Key) || IsDigit(e.Key) || IsLetter(e.Key))
            {
                // Allow to press
                handled = false;
            }

            // Dont allow to press
            e.Handled = handled;
        }

        private static void NumericTextBoxKeyDown(object sender, KeyEventArgs e)
        {

            bool handled = true;

            // Check if it is valid key, digit, letter
            if (controlKeys.Contains(e.Key) || IsDigit(e.Key))
            {
                // Allow to press
                handled = false;
            }

            // Dont allow to press
            e.Handled = handled;
        }

        private static void FilterSpaceKeyDown(object sender, KeyEventArgs e)
        {
            bool handled = false;
            if (e.Key == Key.Space)
            {
                handled = true;
            }
            e.Handled = handled;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets IsAlphaNumericFilterProperty
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool GetIsAlphaNumericFilter(DependencyObject src)
        {
            return (bool)src.GetValue(IsAlphaNumericFilterProperty);
        }

       

        /// <summary>
        /// Sets IsAlphaNumericFilterProperty
        /// </summary>
        /// <param name="src"></param>
        /// <param name="value"></param>
        public static void SetIsAlphaNumericFilter(DependencyObject src, bool value)
        {
            src.SetValue(IsAlphaNumericFilterProperty, value);
        }

        /// <summary>
        /// Gets IsAlphaNumericFilterProperty
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool GetIsNumericFilter(DependencyObject src)
        {
            return (bool)src.GetValue(IsAlphaNumericFilterProperty);
        }

        /// <summary>
        /// Sets IsAlphaNumericFilterProperty
        /// </summary>
        /// <param name="src"></param>
        /// <param name="value"></param>
        public static void SetIsNumericFilter(DependencyObject src, bool value)
        {
            src.SetValue(IsAlphaNumericFilterProperty, value);
        }
        
        
        /// <summary>
        /// The event occurs on IsAlphaNumericFilterProperty changed
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        public static void IsAlphaNumericFilterChanged(DependencyObject src, DependencyPropertyChangedEventArgs args)
        {
            if (src != null && src is TextBox)
            {
                TextBox textBox = src as TextBox;

                InputMethod.SetIsInputMethodEnabled(src, false);

                if ((bool)args.NewValue)
                {
                    textBox.KeyDown += TextBoxKeyDown;
                    DataObject.AddPastingHandler(textBox, CancelCommand);
                }
            }
        }

        /// <summary>
        /// The event occurs on IsNumericFilterProperty changed
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        public static void IsNumericFilterChanged(DependencyObject src, DependencyPropertyChangedEventArgs args)
        {
            if (src != null && src is TextBox)
            {
                TextBox textBox = src as TextBox;

                InputMethod.SetIsInputMethodEnabled(src, false);

                if ((bool)args.NewValue)
                {
                    textBox.PreviewKeyDown += FilterSpaceKeyDown;
                    textBox.KeyDown += NumericTextBoxKeyDown;
                    DataObject.AddPastingHandler(textBox, NumericCancelCommand);
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Property indicates if the text is alphanumeric
        /// </summary>
        public static DependencyProperty IsAlphaNumericFilterProperty =
            DependencyProperty.RegisterAttached(
            "IsAlphaNumericFilter", typeof(bool), typeof(TextBoxFilters),
            new PropertyMetadata(false, IsAlphaNumericFilterChanged));

        /// <summary>
        /// The Property indicates if the text is numeric
        /// </summary>
        public static DependencyProperty IsNumericFilterProperty =
            DependencyProperty.RegisterAttached(
            "IsNumericFilter", typeof(bool), typeof(TextBoxFilters),
            new PropertyMetadata(false, IsNumericFilterChanged));
        #endregion

    }
}
