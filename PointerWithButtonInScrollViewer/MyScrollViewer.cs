using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PointerWithButtonInScrollViewer
{
    public class MyScrollViewer : ScrollViewer
    {
        //protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
        //{
        //    Trace.WriteLine("Starting");
        //}

        //protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        //{
        //    Trace.WriteLine("Delta");
        //}

        //protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        //{
        //    Trace.WriteLine("InertiaStarting");

        //}


        private int count = 1;

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            Trace.WriteLine("Completed");



            //RaiseMouseUpEventInCode();
            ((MainWindow)(Application.Current.MainWindow)).FunctionInWindow();
            //Application.Current.MainWindow.Activate();
            // 运行一次
            // 失去焦点 —— 变化大小？ —— 切换到explore窗口
            //if (count > 0)
            //{
            //    Application.Current.MainWindow.Width = 500;
            //    count--;
            //}

            // IsHitTestVisible = false 触发 OnManipulationCompleted 同时触发之后也没有触摸
            // 设置这个false，触发completed，触发完了触摸消失、?
            // IsManipulationEnabled 为 false 会触发 OnManipulationCompleted 事件? 相当于 设置PanningMode None时效果一样？
        }

        private void RaiseMouseUpEventInCode()
        {
            int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;
            MouseButton mouseButton = MouseButton.Left;

            var mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, mouseButton)
            {
                RoutedEvent = MouseEnterEvent,
                Source = this,
            };

            RaiseEvent(mouseUpEvent);
            Trace.WriteLine("尝试主动触发事件");
        }


        protected override void OnTouchDown(TouchEventArgs e)
        {
            Trace.WriteLine("Touch down");
        }
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            Trace.WriteLine("Started");

            //var result = VisualTreeHelper.HitTest(this, e.ManipulationOrigin);
            //if (result?.VisualHit != null)
            //{
            //    var parent  = VisualTreeHelper.GetParent(result.VisualHit);
            //}
        }

        private bool HasButtonParent(DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);

            if ((parent != null) && (parent is ButtonBase) == false)
            {
                return HasButtonParent(parent);
            }

            return parent != null;
        }
    }
}