// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageDialog.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Views
{
    #region References
    using System;
    using System.Windows;
    using System.Drawing;
    using System.Windows.Media;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using System.Media;

    #endregion References

    /// <summary>
    /// Interaction logic for ErrorMessageDialog.xaml
    /// </summary>
    public partial class ErrorMessageDialog : Window
    {
        static Bitmap bitmap = SystemIcons.Hand.ToBitmap();
        static IntPtr hBitmap = bitmap.GetHbitmap();
        static ImageSource errorIcon = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        public ErrorMessageDialog()
        {
            InitializeComponent();
        }

        private ErrorMessageDialog(string message, string details)
        {
            InitializeComponent();
            Message.Text = message;
            if (string.IsNullOrEmpty(details))
            {
                DetailsExpander.Visibility = Visibility.Collapsed;
            }
            else
            {
                Details.Text = details;
            }
            Icon = errorIcon;
            Img.Source = errorIcon;
            OK.Click += (s, e) => Close();
            SystemSounds.Hand.Play();
        }

        /// <summary>
        /// seperate from the Show for unit test
        /// </summary>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static ErrorMessageDialog GetErrorMessageDialog(string message, string details, Window owner)
        {
            return new ErrorMessageDialog(message, details) { Owner = owner };
        }

        public static void Show(string message, string details, Window owner)
        {
            GetErrorMessageDialog(message, details, owner).ShowDialog();
        }
    }
}
