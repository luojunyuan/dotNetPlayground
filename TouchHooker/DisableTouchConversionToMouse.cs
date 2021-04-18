using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using AutoHotkey.Interop;

namespace TouchHooker
{
    public class DisableTouchConversionToMouse : IDisposable
    {
        static readonly LowLevelMouseProc hookCallback = HookCallback;
        static IntPtr _hookId = IntPtr.Zero;

        public DisableTouchConversionToMouse()
        {
            _hookId = SetHook(hookCallback);
        }

        static IntPtr SetHook(LowLevelMouseProc proc)
        {
            var moduleHandle = UnsafeNativeMethods.GetModuleHandle(null);

            var setHookResult = UnsafeNativeMethods.SetWindowsHookEx(WH_MOUSE_LL, proc, moduleHandle, 0);
            if (setHookResult == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return setHookResult;
        }

        delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private  static AutoHotkeyEngine _ahk = AutoHotkeyEngine.Instance;

        static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var info = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                uint extraInfo = (uint)info.dwExtraInfo.ToInt64();
                if ((extraInfo & MOUSEEVENTF_FROMTOUCH) == MOUSEEVENTF_FROMTOUCH)
                {
                    //Trace.WriteLine($"{info.pt.x} {info.pt.y} {wParam}"); // 514 517
                    //ahk.ExecRaw("MouseClick, left");
                    if ((int)wParam == 514)
                    {
                        Task.Run(
                            () =>
                            {
                                _ahk.ExecRaw("Click, Down");
                                _ahk.ExecRaw("Sleep 50");
                                _ahk.ExecRaw("Click, Up");
                            });

                    }

                    return UnsafeNativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    //return new IntPtr(1);
                }
            }

            return UnsafeNativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        bool disposed;

        public void Dispose()
        {
            if (disposed) return;

            UnsafeNativeMethods.UnhookWindowsHookEx(_hookId);
            disposed = true;
            GC.SuppressFinalize(this);
        }

        ~DisableTouchConversionToMouse()
        {
            Dispose();
        }

        #region Interop

        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local

        const uint MOUSEEVENTF_FROMTOUCH = 0xFF515700;
        const int WH_MOUSE_LL = 14;

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {

            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [SuppressUnmanagedCodeSecurity]
        static class UnsafeNativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod,
                uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        // ReSharper restore InconsistentNaming
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local

        #endregion
    }
}