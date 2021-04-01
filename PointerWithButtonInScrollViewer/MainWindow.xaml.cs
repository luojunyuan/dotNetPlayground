using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace PointerWithButtonInScrollViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 1 复现方法
            // 运行程序后，是用手指滑动右侧空白区域，然后点击按钮会发现没有效果
            // 滑动ScrollViewer后点击一下窗口外使窗口失去焦点
            // 再回来滑动或点击按钮，一切工作正常

            // 2 行为
            // 滑动会触发 Started Completed
            // 点击空白会触发 Started
            // 点击按钮会触发 Started Clicked

            // 3 不正确的行为
            // 点击按钮仅触发 Started，却没有Clicked，冒泡到Touch便中断了，没有Mouse事件

            // 4 再度复现
            // 点击窗口外失去焦点后，直接回来继续滑动，还是会造成无法点击按钮的现象

            // 滑动后 右侧拖动条都无法点击到
            // 必须先激活按钮，之后就一切正常了额

            // 总结就是 手指第一次触碰窗口时，不可以直接滑动，需轻轻点一下不管你点哪儿。
            // 无效后的鼠标，左键右键滑动输入全部没效果了

            // 模拟鼠标也没用，因为他把整个鼠标都屏蔽掉了。
            //this.AddHandler(TouchDownEvent, new RoutedEventHandler(GetTouchDown));

            // 5 恢复
            // 触控直接随意在scrollview上滚动一下（任意位置），现在按钮无法点击了
            // 把鼠标移动出窗口外，再移动进来，再次直接随意滚动，现在按钮可以点击，一起恢复正常了

            Loaded += (sender, args) =>
            {
                //DisableWPFTabletSupport();
            };

        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Trace.WriteLine("Mouse enter");
        }

        public void FunctionInWindow()
        {
            // Unable to cast object of type 'System.Windows.Input.MouseEventHandler' to type 'System.Windows.Input.MouseButtonEventHandler'.”
            //RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            //{
            //    RoutedEvent = Mouse.MouseEnterEvent,
            //    Source = this,
            //});

            //MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            //arg.RoutedEvent = Button.MouseEnterEvent;
            //myWin.RaiseEvent(arg);

            //RaiseMouseEnterEventInCode();

            // 三个一样的错
            //GetTouchDown(null, new RoutedEventArgs());


            // Copied the snippet of code from the link above, just to help future readers
            MouseEventArgs mouseEventArgs = new MouseEventArgs(Mouse.PrimaryDevice, 0)
            {
                RoutedEvent = Mouse.MouseEnterEvent
            };

            //RaiseEvent(mouseEventArgs); // 确实引发了。。。。。。但是没有用 55555
            myWin.RaiseEvent(mouseEventArgs); // 一样。。。。


            //InputManager.Current.ProcessInput(mouseEventArgs);// 这个没用没法触发


            Trace.WriteLine("看到这儿保证代码是执行过的");
            // this 和 myWIn一样吗
        }

        private void RaiseMouseEnterEventInCode()
        {
            int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;
            MouseButton mouseButton = MouseButton.Left;

            var mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, mouseButton)
            {
                RoutedEvent = MouseEnterEvent,
                Source = this,
            };

            RaiseEvent(mouseUpEvent);
        }

        private void GetTouchDown(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaa");
            TouchEventArgs te = (TouchEventArgs)e;
            Point p = te.GetTouchPoint(this).Position;
            UIElement uiControl = (UIElement)this.InputHitTest(p);
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, te.Timestamp, MouseButton.Left);
            args.RoutedEvent = MouseEnterEvent;
            uiControl.RaiseEvent(args);
        }

        //protected override void OnPreviewTouchDown(TouchEventArgs e)
        //{
        //    base.OnPreviewTouchDown(e);
        //    Trace.WriteLine("XAML Preview Touch Down");

        //}

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    base.OnTouchDown(e);
        //    Trace.WriteLine("XAML Touch Down");
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    base.OnTouchEnter(e);
        //    Trace.WriteLine("XAML touch enter");
        //}

        protected override void OnActivated(EventArgs e)
        {
            //button.Focus();
            //Trace.WriteLine("first button focus");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("button clicked!!!");
        }



        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.  
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.  
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.  
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.  
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.  
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                    }
                }

            }
        }
    }
}
