using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfScreenHelper;

namespace ScreenShotANDJapaneseOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //https://stackoverflow.com/questions/15847637/take-screenshot-of-multiple-desktops-of-all-visible-applications-and-forms
            var bounds = new System.Drawing.Rectangle();
            bounds = Screen.AllScreens.Aggregate(bounds, (current, screen)
                => System.Drawing.Rectangle.Union(current, new System.Drawing.Rectangle(
                    (int)screen.Bounds.X, (int)screen.Bounds.Y, (int)screen.Bounds.Width, (int)screen.Bounds.Height)));
            var screenShot = Convert.ToBase64String(ScreenShotHelper.TakeAsByteArray(bounds));
            Trace.WriteLine(screenShot);
        }
    }
}
