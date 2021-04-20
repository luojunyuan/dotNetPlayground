using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenShotANDJapaneseOCR
{
    public static class ScreenShotHelper
    {
        private static Bitmap CopyFromScreen(Rectangle bounds)
        {
            try
            {
                var image = new Bitmap(bounds.Width, bounds.Height);
                using var graphics = Graphics.FromImage(image);
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                return image;
            }
            catch (Win32Exception)
            {//When screen saver is active
                return null;
            }
        }

        public static Image Take(Rectangle bounds)
        {
            return CopyFromScreen(bounds);

        }

        public static byte[] TakeAsByteArray(Rectangle bounds)
        {
            using var image = CopyFromScreen(bounds);
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public static void TakeAndSave(string path, Rectangle bounds, ImageFormat imageFormat)
        {
            using var image = CopyFromScreen(bounds);
            image.Save(path, imageFormat);
        }
    }
}