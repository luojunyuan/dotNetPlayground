using System;
using System.Runtime.InteropServices;

namespace TouchHooker
{
    public static class NativeMethods
    {
        public const uint MOUSEEVENTF_FROMTOUCH = 0xFF515700;

        public static void MouseLeftDown()
        {
            MouseEvent(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        public static void MouseLeftUp()
        {
            MouseEvent(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void MouseEventClick()
        {
            MouseEvent(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void MouseEventRight()
        {
            MouseEvent(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [DllImport("user32.dll", EntryPoint = "mouse_event", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseEvent(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
    }
}