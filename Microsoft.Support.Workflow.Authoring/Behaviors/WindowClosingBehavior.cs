// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowClosingBehavior.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Behaviors
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    #endregion References

    /// <summary>
    /// Defines a behavior for the Closing event that the ViewModel can use
    /// </summary>
    public class WindowClosingBehavior
    {
        #region ICommand static Property
        /// <summary>
        /// Gets the ICommand for the Closing from a Dependency Property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetClosing(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClosingProperty);
        }

        /// <summary>
        /// Sets the ICommand for the Closing to a Dependency Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetClosing(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosingProperty, value);
        }

        #endregion ICommand static Property

        #region Dependency Property

        /// <summary>
        /// Dependency Property for the Closing "event"
        /// </summary>
        public static readonly DependencyProperty ClosingProperty = DependencyProperty.RegisterAttached(
            "Closing", typeof(ICommand), typeof(WindowClosingBehavior),
            new UIPropertyMetadata(new PropertyChangedCallback(ClosingChanged)));

        /// <summary>
        /// Notfication that the behavior is requested
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private static void ClosingChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Window window = target as Window;

            if (window != null)
            {
                if (e.NewValue != null)
                {
                    window.Closing += Window_Closing;
                }
            }
        }
        
        #endregion Dependency Property
        
        #region Code behind style event handler

        /// <summary>
        /// Closing handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Window_Closing(object sender, CancelEventArgs e)
        {
            // The Value of e.Cancel is determined by CanExecute 
            // the reason is that since this is a Closing event 
            // the Command has already essentially been executed
            ICommand closing = GetClosing(sender as Window);
            if (closing != null)
            {
                e.Cancel = closing.CanExecute(null);
            }
        }

        #endregion Code behind style event handler
    }
}