using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MicMute.Resources
{
    /// <summary>
    /// Provides static icons 
    /// </summary>
    internal static class ImageResources
    {
        internal static ResourceDictionary Resources { get; private set; }

        internal static Image Muted => Resources[nameof(Muted)] as Image;

        internal static Image Unmuted => Resources[nameof(Unmuted)] as Image;

        internal static Image IconMutedImage => Resources[nameof(IconMuted)] as Image;

        internal static Image IconUnmutedImage => Resources[nameof(IconUnmuted)] as Image;

        internal static System.Drawing.Icon IconMuted => IconMutedImage.ToIcon();

        internal static System.Drawing.Icon IconUnmuted => IconUnmutedImage.ToIcon();

        static ImageResources()
        {
            Resources = new ResourceDictionary();
            Resources.Source = new Uri("/MicMute;component/Resources/ImageResources.xaml", UriKind.RelativeOrAbsolute);
        }

        public static System.Drawing.Bitmap ToBitmap(this System.Windows.Controls.Image image)
        {
            BitmapSource srs = image.Source as BitmapSource;

            System.Drawing.Bitmap btm = null;
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = Marshal.AllocHGlobal(height * stride);
            srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
            btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr);
            return btm;
        }

        public static System.Drawing.Icon ToIcon(this System.Windows.Controls.Image image)
        {
            System.Drawing.Bitmap btm = ToBitmap(image);
            return System.Drawing.Icon.FromHandle(btm.GetHicon());
        }
    }
}
