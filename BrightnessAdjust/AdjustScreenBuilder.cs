using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace BrightnessAdjust
{
    public class AdjustScreenBuilder
    {
        /// <summary>
        /// 创建一个调整屏幕亮度的对象
        /// </summary>
        /// <returns>如果返回空，则代表此屏幕不支持</returns>
        public static IAdjustScreen? CreateAdjustScreen(IntPtr handle)
        {
            // 第一次尝试使用Dxva2方式
            AdjustScreenByDxva2 adjustScreenByDxva2 = new AdjustScreenByDxva2();
            short min = 0;
            short current = 0;
            short max = 0;
            var adjustScreenByDxva2Value = adjustScreenByDxva2.GetBrightness(handle, ref min, ref current, ref max);

            if (adjustScreenByDxva2Value)
            {
                return adjustScreenByDxva2;
            }
            else
            {
                // 如果不满足，则尝试使用Gdi32方式
                var adjustScreenByGdi32 = new AdjustScreenByGdi32();
                var adjustScreenByGdi32Value = adjustScreenByGdi32.GetBrightness(handle, ref min, ref current, ref max);
                if (adjustScreenByGdi32Value)
                    return adjustScreenByGdi32;
            }

            return null;
        }
    }

    public class AdjustScreenByDxva2 : IAdjustScreen
    {
        [DllImport("dxva2.dll")]
        public static extern bool SetMonitorBrightness(IntPtr hMonitor, short brightness);

        [DllImport("dxva2.dll")]
        public static extern bool GetMonitorBrightness(IntPtr hMonitor, ref short pdwMinimumBrightness,
            ref short pdwCurrentBrightness, ref short pdwMaximumBrightness);

        [DllImport("dxva2.dll")]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
            ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll")]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
            uint dwPhysicalMonitorArraySize, [Out] PhysicalMonitor[] pPhysicalMonitorArray);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

        /// <summary>
        /// internal类型不要修改，不希望外界构造。可通过<see cref="AdjustScreenBuilder.CreateAdjustScreen"/>创建
        /// 由于调整屏幕亮度有多种方案，不同的屏幕适配不同的方案。所以不需要外界做过多判断。
        /// </summary>
        internal AdjustScreenByDxva2()
        {
        }

        /// <summary>
        /// 设置屏幕亮度
        /// </summary>
        /// <param name="handle">所在屏幕窗口的句柄</param>
        /// <param name="brightness">亮度</param>
        /// <returns></returns>
        public bool SetBrightness(IntPtr handle, short brightness)
        {
            if (handle == IntPtr.Zero) return false;

            uint pdwNumberOfPhysicalMonitors = uint.MinValue;
            //获取屏幕所在的屏幕设备
            var hMonitor = MonitorFromWindow(handle, (uint)NativeConstantsEnum.MonitorDefaultToPrimary);
            GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
            var screen = new PhysicalMonitor[pdwNumberOfPhysicalMonitors];
            GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, screen);
            if (screen.Length <= 0) return false;
            return SetMonitorBrightness(screen[0].hPhysicalMonitor, brightness);
        }

        /// <summary>
        /// 设置屏幕亮度
        /// </summary>
        /// <param name="handle">所在屏幕窗口的句柄</param>
        /// <param name="minBrightness"></param>
        /// <param name="currentBrightness"></param>
        /// <param name="maxBrightness"></param>
        /// <returns></returns>
        public bool GetBrightness(IntPtr handle, ref short minBrightness,
            ref short currentBrightness,
            ref short maxBrightness)
        {
            if (handle == IntPtr.Zero) return false;
            uint pdwNumberOfPhysicalMonitors = uint.MinValue;
            //获取屏幕所在的屏幕设备
            var hMonitor = MonitorFromWindow(handle, (uint)NativeConstantsEnum.MonitorDefaultToPrimary);
            GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
            var screen = new PhysicalMonitor[pdwNumberOfPhysicalMonitors];
            GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, screen);
            if (screen.Length <= 0) return false;

            return GetMonitorBrightness(screen[0].hPhysicalMonitor, ref minBrightness,
                ref currentBrightness,
                ref maxBrightness);
        }
    }

    public class AdjustScreenByGdi32 : IAdjustScreen
    {
        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDc, ref Ramp lpRamp);

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDc, ref Ramp lpRamp);

        private double CalAllGammaVal(Ramp ramp)
        {
            return Math.Round(((CalColorGammaVal(ramp.Blue) + CalColorGammaVal(ramp.Red) +
                                CalColorGammaVal(ramp.Green)) / 3), 2);
        }

        /// <summary>
        /// internal类型不要修改，不希望外界构造。可通过<see cref="AdjustScreenBuilder.CreateAdjustScreen"/>创建
        /// 由于调整屏幕亮度有多种方案，不同的屏幕适配不同的方案。所以不需要外界做过多判断。
        /// </summary>
        internal AdjustScreenByGdi32()
        {
        }

        private double CalColorGammaVal(ushort[] line)
        {
            var max = line.Max();
            var min = line[0];
            var index = Array.FindIndex(line, n => n == max);
            var gamma = Math.Round((((double)(max - min) / index) / 255), 2);
            return gamma;
        }

        /// <summary>
        /// 读取屏幕亮度
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="minBrightness"></param>
        /// <param name="currentBrightness"></param>
        /// <param name="maxBrightness"></param>
        /// <returns></returns>
        public bool GetBrightness(IntPtr handle, ref short minBrightness, ref short currentBrightness,
            ref short maxBrightness)
        {
            handle = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
            //0-50 亮度变化太小，所以从50开始
            minBrightness = 50;
            maxBrightness = 100;
            var ramp = default(Ramp);
            var deviceGammaRamp = GetDeviceGammaRamp(handle, ref ramp);
            currentBrightness = (short)((deviceGammaRamp ? CalAllGammaVal(ramp) : 0.5) * 100);
            return deviceGammaRamp;
        }

        /// <summary>
        /// 设置屏幕亮度
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public bool SetBrightness(IntPtr handle, short brightness)
        {
            handle = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
            double value = (double)brightness / 100;
            Ramp ramp = default(Ramp);
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            for (int i = 1; i < 256; i++)
            {
                var tmp = (ushort)(i * 255 * value);
                ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, tmp));
            }

            var deviceGammaRamp = SetDeviceGammaRamp(handle, ref ramp);
            return deviceGammaRamp;
        }
    }

    public interface IAdjustScreen
    {
        bool GetBrightness(IntPtr handle, ref short minBrightness,
            ref short currentBrightness,
            ref short maxBrightness);

        bool SetBrightness(IntPtr handle, short brightness);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Ramp
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Red;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Green;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Blue;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PhysicalMonitor
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    public enum NativeConstantsEnum
    {
        MonitorDefaultToNull,
        MonitorDefaultToPrimary,
        MonitorDefaultToNearest
    }

    class AdjustScreenByWmi
    {
        // Store array of valid level values
        private readonly byte[] _brightnessLevels;
        // Define scope (namespace)
        readonly ManagementScope _scope = new ManagementScope("root\\WMI");
        // Define querys
        readonly SelectQuery _query = new SelectQuery("WmiMonitorBrightness");
        readonly SelectQuery _queryMethods = new SelectQuery("WmiMonitorBrightnessMethods");

        public bool IsSupported { get; set; }

        public AdjustScreenByWmi()
        {
            //get the level array for this system
            _brightnessLevels = GetBrightnessLevels();
            if (_brightnessLevels.Length == 0)
            {
                //"WmiMonitorBrightness" is not supported by the system
                IsSupported = false;
            }
            else
            {
                IsSupported = true;
            }
        }

        public void IncreaseBrightness()
        {
            if (IsSupported)
            {
                StartupBrightness(GetBrightness() + 10);
            }
        }

        public void DecreaseBrightness()
        {
            if (IsSupported)
            {
                StartupBrightness(GetBrightness() - 10);
            }
        }

        /// <summary>
        /// Returns the current brightness setting
        /// </summary>
        /// <returns></returns>
        private int GetBrightness()
        {
            using ManagementObjectSearcher searcher = new(_scope, _query);
            using ManagementObjectCollection objCollection = searcher.Get();

            byte curBrightness = 0;
            foreach (var o in objCollection)
            {
                var obj = (ManagementObject) o;
                curBrightness = (byte)obj.GetPropertyValue("CurrentBrightness");
                break;
            }

            return curBrightness;
        }

        /// <summary>
        /// Convert the brightness percentage to a byte and set the brightness using SetBrightness()
        /// </summary>
        /// <param name="iPercent"></param>
        private void StartupBrightness(int iPercent)
        {
            // XXX: ...
            if (iPercent < 0)
            {
                iPercent = 0;
            }
            else if (iPercent > 100)
            {
                iPercent = 100;
            }

            // iPercent is in the range of brightnessLevels
            if (iPercent <= _brightnessLevels[^1])
            {
                // Default level 100
                byte level = 100;
                foreach (byte item in _brightnessLevels)
                {
                    // 找到 brightnessLevels 数组中与传入的 iPercent 接近的一项
                    if (item >= iPercent)
                    {
                        level = item;
                        break;
                    }
                }
                SetBrightness(level);
            }
        }

        /// <summary>
        /// Set the brightness level to the targetBrightness
        /// </summary>
        /// <param name="targetBrightness"></param>
        private void SetBrightness(byte targetBrightness)
        {
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, _queryMethods);
            using ManagementObjectCollection objectCollection = searcher.Get();
            foreach (var o in objectCollection)
            {
                var mObj = (ManagementObject) o;
                // Note the reversed order - won't work otherwise!
                mObj.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, targetBrightness });
                // Only work on the first object
                break;
            }
        }

        private byte[] GetBrightnessLevels()
        {
            // Output current brightness
            using ManagementObjectSearcher mos = new(_scope, _query);

            byte[] bLevels = Array.Empty<byte>();
            try
            {
                using ManagementObjectCollection moc = mos.Get();
                foreach (var managementBaseObject in moc)
                {
                    var o = (ManagementObject) managementBaseObject;
                    bLevels = (byte[])o.GetPropertyValue("Level");
                    // Only work on the first object
                    break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return bLevels;
        }
    }
}
