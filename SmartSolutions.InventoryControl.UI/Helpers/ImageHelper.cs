using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SmartSolutions.InventoryControl.UI.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage ToBitmapImage(this byte[] array)
        {
            return ByteArrayToBitmapImage(array);
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            BitmapImage image = null;
            try
            {
                if (array != null && array.Length > 0)
                {
                    using (var ms = new MemoryStream(array))
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                    }
                }
            }
            catch (Exception)
            {
                image = null;
            }
            return image;
        }

        public static byte[] ToByteArray(this BitmapSource bitmapImage)
        {
            byte[] byteArray = null;
            try
            {
                if (bitmapImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        pngEncoder.Save(ms);

                        // Return byte[]
                        byteArray = ms.GetBuffer();
                    }
                }
            }
            catch (Exception)
            {
                byteArray = null;
            }
            return byteArray;
        }

        public static byte[] ToByteArray(this Bitmap bitmap)
        {
            byte[] byteArray = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }
            catch (Exception)
            {
                byteArray = null;
            }
            return byteArray;
        }
    }
}

