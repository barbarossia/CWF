using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using form = System.Windows.Forms;
using System.IO;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    /// <summary>
    /// HelpProvider class provides data for customized tooltip control 
    /// </summary>
    public static class HelpProvider
    {
        private const string HelpFilename = @".\Resources\Help.chm";
        private const string HelpKeyPropertyName = "HelpKey";
        private const string HelpTitlePropertyName = "HelpTitle";
        private const string HelpActionPropertyName = "HelpAction";

        //Keyword for searching help content in help doc
        public static readonly DependencyProperty HelpKeyProperty = DependencyProperty.RegisterAttached(HelpKeyPropertyName, typeof(string), typeof(HelpProvider));

        //Title displayed at the left top of the help toolptip control
        public static readonly DependencyProperty HelpTitleProperty = DependencyProperty.RegisterAttached(HelpTitlePropertyName, typeof(string), typeof(HelpProvider));

        //Action which user can do displayed below the Title in the help tooltip control
        public static readonly DependencyProperty HelpActionProperty =
            DependencyProperty.RegisterAttached(HelpActionPropertyName, typeof(string), typeof(HelpProvider));


        //Help Key that used to search help details in Help.chm
        public static string GetHelpKey(DependencyObject obj)
        {
            if (obj != null)
            {
                return (string)obj.GetValue(HelpKeyProperty);
            }
            else
            { return string.Empty; }
        }

        public static void SetHelpKey(DependencyObject obj, string value)
        {
            if (obj != null)
            {
                obj.SetValue(HelpKeyProperty, value);
            }
        }

        //Help title
        public static string GetHelpTitle(DependencyObject obj)
        {
            if (obj != null)
            {
                return (string)obj.GetValue(HelpTitleProperty);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Set help title
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHelpTitle(DependencyObject obj, string value)
        {
            if (obj != null)
            {
                obj.SetValue(HelpTitleProperty, value);
            }
        }

        
        //help action
        public static string GetHelpAction(DependencyObject obj)
        {
            if (obj != null)
            {
                return (string)obj.GetValue(HelpActionProperty);
            }
            else
            { return string.Empty; }
        }

        /// <summary>
        /// Set help action
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHelpAction(DependencyObject obj, string value)
        {
            if (obj != null)
            {
                obj.SetValue(HelpActionProperty, value);
            }
        }

        
        static HelpProvider()
        {
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement),
             new CommandBinding(ApplicationCommands.Help,
               new ExecutedRoutedEventHandler(Executed),
               new CanExecuteRoutedEventHandler(CanExecute)));
        }

        /// <summary>
        /// Validate if can open the help doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            if (File.Exists(HelpFilename))
                args.CanExecute = true;
            else
                args.CanExecute = false;
        }

        /// <summary>
        /// Open help doc and search content according the provided keyword
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Executed(object sender, ExecutedRoutedEventArgs args)
        {
            string keyword = HelpProvider.GetHelpKey(sender as DependencyObject);

            //No keyword,Don't display search page in Help doc.
            if (string.IsNullOrEmpty(keyword))
                form.Help.ShowHelp(null, HelpFilename);
            else
                form.Help.ShowHelp(null, HelpFilename, form.HelpNavigator.Index, keyword);
        }

    }
}
