using System;
using System.Windows;

namespace BrightnessAdjust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        IAdjustScreen? _brightnessHelper;

        public void ScreenExample()
        {
            // 通过窗口方法，获取handle
            IntPtr handle = IntPtr.Zero;

            // 建立helper实例
            _brightnessHelper = AdjustScreenBuilder.CreateAdjustScreen(handle);
            if (_brightnessHelper is not null)
            {
                // Dxv2
            }
            else
            {
                // Gdi32
            }
        }

        public void ControlBrightness(IntPtr handle, short value)
        {
            // 设置亮度
            _brightnessHelper?.SetBrightness(IntPtr.Zero, value);

            // 初始化三个亮度指标
            short min = 0;
            short max = 0;
            short cur = 0;
            // 通过GetBrightness后，min max cur 获取当前亮度状况。
            _brightnessHelper?.GetBrightness(handle, ref min, ref cur, ref max);
        }

        public void WmiWay()
        {
            AdjustScreenByWmi wmiBrightnessHelper = new();
            // 通过这个属性来看电脑是否支持wmi调整亮度，设计的不是很好。
            if (wmiBrightnessHelper.IsSupported)
            {
                // 我在内部写死了10点10点的变化
                wmiBrightnessHelper.IncreaseBrightness();
                wmiBrightnessHelper.DecreaseBrightness();
            }
        }
    }
}
