using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenShotANDJapaneseOCR
{
    /// <summary>
    /// ScreenShotFakeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenShotFakeWindow : Window
    {
        public ScreenShotFakeWindow()
        {
            InitializeComponent();
        }

        public void GetScreenSnapshot()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            using (Bitmap bmp = new Bitmap((int)screenWidth,
                (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    bmp.Save("C:\\Screenshots\\" + filename);
                    Opacity = 1;
                }

            }
        }

        /// <summary>
        /// 获取本机分辨率
        /// </summary>
        private void GetScreenSize()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private Bitmap image;

        /// <summary>
        /// 初始化截图，截取把整个屏幕并显示
        /// </summary>
        private void InitializeImage()
        {
            GetScreenSize();
            image = new Bitmap(Convert.ToInt32(Width), Convert.ToInt32(Height));  //设置截图区域大小为整个屏幕
            using (Graphics g = Graphics.FromImage(image))
            {
                g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(Convert.ToInt32(Width), Convert.ToInt32(Height)));  //复制当前屏幕到画板上，即将截屏图片的内容设置为当前屏幕
            }
            BitmapSource bimage = BitmapConvert(image);
            ImageBrush b = new ImageBrush();
            b.ImageSource = bimage;
            b.Stretch = Stretch.None;
            this.Background = b;  //将截屏设为背景
            //mask.Height = Height;
            //mask.Width = Width;
            //InitializeMask();  //添加黑色遮罩
            //CompositionTarget.Rendering += UpdateSelection;  //注册窗体重绘事件
        }

        public static BitmapSource BitmapConvert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }
    }
}
