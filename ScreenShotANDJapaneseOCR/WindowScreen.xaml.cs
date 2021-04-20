using System;
using System.Collections.Generic;
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
    /// WindowScreen.xaml 的交互逻辑
    /// </summary>
    public partial class WindowScreen : Window
    {
        public WindowScreen()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ImgScreen_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!_isMouseDown)
            //    return;

            //gridCover.Visibility = Visibility.Visible;

            ////计算鼠标选中区域
            //Point currentPoint = e.GetPosition(imgScreen);
            //Point borderPoint = e.GetPosition(borderSelect);

            //double xDelta = xDelta_BoderToImgScreen;
            //double yDelta = yDelta_BoderToImgScreen;

            //_rectImgCut = ImageHelper.ToRect(currentPoint, _startPoint);

            //Rect rectBoderCut = ImageHelper.ToRect(new Point(currentPoint.X + xDelta, currentPoint.Y + yDelta),
            //    new Point(_startPoint.X + xDelta, _startPoint.Y + yDelta));

            ////设置方框位置和大小
            //Thickness thickness = new Thickness(rectBoderCut.Left, rectBoderCut.Top, 0, 0);
            //borderSelect.SetValue(FrameworkElement.MarginProperty, thickness);
            //imgCut.SetValue(FrameworkElement.MarginProperty, thickness);

            //thickness = new Thickness(rectBoderCut.Left, 3, 0, 0);
            //txtCutInfo.SetValue(FrameworkElement.MarginProperty, thickness);

            //borderSelect.Width = Math.Abs(_startPoint.X - currentPoint.X);
            //borderSelect.Height = Math.Abs(_startPoint.Y - currentPoint.Y);
            //borderSelect.Visibility = Visibility.Visible;

            ////为了防止整个图 变暗，鼠标选中区域图像抠图，再在上层图像上显示
            //imgCut.Source = GetBitmapCut();

            //Int32Rect imgDestRect = GetCutRect();
            //txtCutInfo.Text = string.Format($"宽:{imgDestRect.Width} 高:{imgDestRect.Height}");
        }
    }
}
