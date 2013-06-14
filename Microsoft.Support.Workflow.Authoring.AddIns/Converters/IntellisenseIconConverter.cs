using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Prop = Microsoft.Support.Workflow.Authoring.AddIns.Properties;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters
{
    public class IntellisenseIconConverter : IValueConverter
    {
        private static readonly Dictionary<TreeNodeType, int> iconIdMaps = new Dictionary<TreeNodeType, int>()
        {
            { TreeNodeType.Class, 0 },
            { TreeNodeType.ValueType, 0 },
            { TreeNodeType.Enum, 18 },
            { TreeNodeType.Event, 30 },
            { TreeNodeType.Field, 42 },
            { TreeNodeType.Interface, 48 },
            { TreeNodeType.Method, 72 },
            { TreeNodeType.Namespace, 90 },
            { TreeNodeType.Property, 102 },
            { TreeNodeType.Primitive, 108 },
        };
        private static readonly Dictionary<TreeNodeType, BitmapSource> imageMaps = new Dictionary<TreeNodeType, BitmapSource>();

        //Load Image
        private static Bitmap nodeIconImage = null;
        private static Dictionary<int, BitmapSource> iconCache = new Dictionary<int, BitmapSource>();

        static IntellisenseIconConverter()
        {
            nodeIconImage = Prop.Resources.Icons;
            nodeIconImage.MakeTransparent(Color.White);

            foreach (var p in iconIdMaps)
            {
                imageMaps.Add(p.Key, GetBitSource(p.Value));
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeNodeType type = (TreeNodeType)value;
            return imageMaps.ContainsKey(type) ? imageMaps[type] : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static BitmapSource GetBitSource(int bitmapId)
        {
            BitmapSource source;
            if (!iconCache.TryGetValue(bitmapId, out source))
            {
                IntPtr hBitmap = GetBitmap(bitmapId).GetHbitmap();
                source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                iconCache.Add(bitmapId, source);
            }
            return source;
        }

        private static Bitmap GetBitmap(int bitmapId)
        {
            //Set the coordinates and size of the intercepted 
            const int width = 16;
            const int height = 16;
            const int rowCount = 14;
            const int offsetWidth = 40;
            const int offsetHeight = 25;
            const int borderWidth = 2;
            const int borderHeight = 4;

            int startX = bitmapId % rowCount * offsetWidth + borderWidth;
            int startY = bitmapId / rowCount * offsetHeight + borderHeight;

            //Define the interception of a rectangle 
            Rectangle cropArea = new Rectangle(startX, startY, width, height);

            //Be cut 
            Bitmap bmpCrop = nodeIconImage.Clone(cropArea, nodeIconImage.PixelFormat);
            return bmpCrop;
        }
    }
}

