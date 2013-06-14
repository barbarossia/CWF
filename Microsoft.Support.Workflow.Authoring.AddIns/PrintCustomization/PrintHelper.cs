using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Printing.Interop;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Helper for printing
    /// </summary>
    public class PrintHelper
    {
        private List<PaperSize> paperSizes = new List<PaperSize>();

        /// <summary>
        /// The DPI of printing
        /// </summary>
        public const double DPI = 96.0 / 100;
        /// <summary>
        /// All printers on the machine
        /// </summary>
        public Dictionary<PrintQueue, PrinterSettings> Printers { get; private set; }

        /// <summary>
        /// Initialize the print helper
        /// </summary>
        public PrintHelper()
        {
            PrinterSettings settings = new PrinterSettings();
            foreach (PaperSize ps in settings.PaperSizes)
            {
                paperSizes.Add(ps);
            }

            using (PrintServer printServer = new PrintServer())
            {
                Printers = printServer.GetPrintQueues().ToDictionary(p => p,
                    p => new PrinterSettings() { PrinterName = p.FullName });
            }
        }

        /// <summary>
        /// Print all pages
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="visuals"></param>
        public void Print(PrintQueue printer, List<Rectangle> visuals)
        {
            PageMediaSize pageSize = GetPageMediaSize(printer);
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(pageSize.Width.Value, pageSize.Height.Value);
            foreach (Rectangle rect in visuals)
            {
                FixedPage page = new FixedPage()
                {
                    Height = pageSize.Height.Value,
                    Width = pageSize.Width.Value,
                };
                page.Children.Add(rect);

                PageContent pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(page);
                document.Pages.Add(pageContent);
            }

            PrintDialog printDialog = CreatePrintDialogFunc();
            printDialog.PrintQueue = printer;
            printDialog.PrintTicket = printer.UserPrintTicket;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            printDialog.PrintDocument(document.DocumentPaginator, string.Empty);
        }

        /// <summary>
        /// Invoke the printer option window
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="printer"></param>
        /// <returns></returns>
        public bool OpenPrintOptions(Window parent, PrintQueue printer)
        {
            if (parent == null)
                throw new ArgumentNullException("Parent window of printer dialog is null.");

            using (PrintTicketConverter ptc = new PrintTicketConverter(printer.FullName, printer.ClientPrintSchemaVersion))
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(parent).Handle;
                byte[] myDevMode = ptc.ConvertPrintTicketToDevMode(printer.UserPrintTicket, BaseDevModeType.UserDefault);
                GCHandle pinnedDevMode = GCHandle.Alloc(myDevMode, GCHandleType.Pinned);
                IntPtr pDevMode = pinnedDevMode.AddrOfPinnedObject();
                int result = DocumentPropertiesFunc(mainWindowPtr, IntPtr.Zero, printer.FullName, pDevMode, pDevMode, 14);
                if (result == 1)
                {
                    printer.UserPrintTicket = ptc.ConvertDevModeToPrintTicket(myDevMode);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the page media size
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public PageMediaSize GetPageMediaSize(PrintQueue printer)
        {
            return printer.UserPrintTicket.PageMediaSize;
        }

        /// <summary>
        /// Get the paper size
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public PaperSize GetPaperSize(PrintQueue printer)
        {
            PageMediaSize pageMediaSize = GetPageMediaSize(printer);
            double widthInInch = Math.Round(pageMediaSize.Width.Value / DPI);
            double heightInInch = Math.Round(pageMediaSize.Height.Value / DPI);
            return paperSizes.FirstOrDefault(p => p.Width == widthInInch && p.Height == heightInInch);
        }

        /// <summary>
        /// Get the page orientation
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public PageOrientation GetPageOrientation(PrintQueue printer)
        {
            return printer.UserPrintTicket.PageOrientation.HasValue ?
                printer.UserPrintTicket.PageOrientation.Value : PageOrientation.Portrait;
        }

        public static Func<IntPtr, IntPtr, string, IntPtr, IntPtr, int, int> DocumentPropertiesFunc = (IntPtr hwnd, IntPtr hPrinter, string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode) =>
        {
            return DocumentProperties(hwnd, hPrinter, pDeviceName, pDevModeOutput, pDevModeInput,fMode);
        };

        public static Func<PrintDialog> CreatePrintDialogFunc = () =>
        {
            return new PrintDialog();
        };

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);
    }
}
