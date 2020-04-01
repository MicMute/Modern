using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        static ImageResources()
        {
            Resources = new ResourceDictionary();
            Resources.Source = new Uri("/MicMute;component/Resources/ImageResources.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
