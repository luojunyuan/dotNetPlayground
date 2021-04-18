using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WindowsInput.Events;
using WindowsInput.Events.Sources;
using AutoHotkey.Interop;

namespace TouchHooker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly HookProc _mouseHook;
        //private IntPtr _hMouseHook;

        public MainWindow()
        {
            InitializeComponent();
        }

        private DisableTouchConversionToMouse hook;
        private double _x = 1500;
        private double _y = 300;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            hook.Dispose();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            hook = new DisableTouchConversionToMouse();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw("MouseClick, left, 1500, 300");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw("Click, 1500 300");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;
            ahk.ExecRaw("MouseClick, left, 1500, 300");
            ahk.ExecRaw("Send {Enter}");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw("SendEvent {Click 1500 300}");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw($"MouseMove, {_x}, {_y}");
            ahk.ExecRaw("Click, Down"); // 在心空的记忆中单独这一个就作用很好 但肯定不能单独用
            ahk.ExecRaw("Sleep 50"); // 20ms ok 但是按100次会有几次不行的感觉 、、 50很好
            ahk.ExecRaw("Click, Up");
        }

        IMouseEventSource H = WindowsInput.Capture.Global.MouseAsync();

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // WindowsInput 区分不出触摸与鼠标的任何差异 结合使用？
            //H.MouseEvent += H_MouseEvent;

            //H.ButtonClick += H_ButtonClick;
            H.ButtonDown += H_ButtonDown;
            //H.ButtonUp += H_ButtonUp;
            //H.ButtonScroll += H_ButtonScroll;
            // 增加Touch钩子后这些全响应了就离谱 down 514 ...(down) ... Task
            //H.ButtonDoubleClick += H_ButtonDoubleClick;
            //H.ButtonClickHold += H_ButtonClickHold;
        }

        private void H_ButtonClickHold(object sender, EventSourceEventArgs<ButtonClickHold> e)
        {
            Trace.WriteLine("Hold");
        }

        private void H_ButtonDoubleClick(object sender, EventSourceEventArgs<ButtonDoubleClick> e)
        {
            Trace.WriteLine("Double");
        }

        private void H_ButtonScroll(object sender, EventSourceEventArgs<ButtonScroll> e)
        {
            Trace.WriteLine("Button Scroll");
        }

        private void H_ButtonClick(object sender, WindowsInput.Events.Sources.EventSourceEventArgs<WindowsInput.Events.ButtonClick> e)
        {
            Trace.WriteLine("Button Click");
        }

        private void H_ButtonDown(object sender, WindowsInput.Events.Sources.EventSourceEventArgs<WindowsInput.Events.ButtonDown> e)
        {
            Trace.WriteLine("down");
        }

        private void H_ButtonUp(object sender, EventSourceEventArgs<ButtonUp> e)
        {
            Trace.WriteLine("up");
        }
        private static void H_MouseEvent(object sender, EventSourceEventArgs<MouseEvent> e)
        {
            //if (e.Data.ButtonDown is not null)
            //{
            //    Trace.WriteLine("正确的");
            //}
            //if (e.Data.ButtonClick is not null)
            //{
            //    Trace.WriteLine("正确的");
            //}
            //if (e.Data.ButtonUp is not null)
            //{
            //    Trace.WriteLine("正确的");
            //}
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            H.MouseEvent -= H_MouseEvent;
            H.ButtonClick -= H_ButtonClick;
            H.ButtonDown -= H_ButtonDown;
            H.ButtonUp -= H_ButtonUp;
            H.ButtonScroll -= H_ButtonScroll;
            H.ButtonDoubleClick -= H_ButtonDoubleClick;
            H.ButtonClickHold -= H_ButtonClickHold;
            Application.Current.Shutdown();
        }

        private async void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw($"MouseMove, {_x}, {_y}");
            //await WindowsInput.Simulate.Events().Click().Invoke();// 不行
            await WindowsInput.Simulate.Events().Hold(KeyCode.LButton).Wait(50).Release(KeyCode.LButton).Invoke();// 不行
            //await WindowsInput.Simulate.Events().ClickChord(KeyCode.LWin).Invoke(); // 正常
            Trace.WriteLine("a");
        }

        private async void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var ahk = AutoHotkeyEngine.Instance;

            ahk.ExecRaw($"MouseMove, {_x}, {_y}");
            // make sure the are at same point
            NativeMethods.MouseLeftDown();
            await Task.Delay(50);
            NativeMethods.MouseLeftUp();
            Trace.WriteLine("aaa");
        }
    }
}
