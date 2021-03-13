using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NetworkChecker
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"{"System.Net.NetworkInformation module", -40} {StopwatchWrapper(CheckNet)}");
            Console.WriteLine($"{"WIN32 Api", -40} {StopwatchWrapper(IsInternetAvailable)}");
            Console.WriteLine($"{"COM Component", -40} {StopwatchWrapper(IsConnected)}");
            Console.WriteLine($"{"Ping", -40} {StopwatchWrapper(PingStatus)}");

            // .Net module, internet event
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += (sender, eventArgs) =>
            {
                if (eventArgs.IsAvailable)
                    Console.WriteLine("Network connected!");
                else
                    Console.WriteLine("Network dis-connected!");
            };
            Console.ReadKey();
        }

        /// <summary>
        /// 1. Always True
        /// </summary>
        private static bool CheckNet()
        {
            var stats = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            return stats;
        }

        /// <summary>
        /// 2. Win32 api
        /// </summary>
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        private static bool IsInternetAvailable()
        {
            return InternetGetConnectedState(out _, 0);
        }

        /// <summary>
        /// 3. COM Component
        /// </summary>
        private static readonly NETWORKLIST.INetworkListManager _networkListManager = new NETWORKLIST.NetworkListManager();

        private static bool IsConnected() => _networkListManager.IsConnectedToInternet;

        /// <summary>
        /// 4. Use ping
        /// </summary>
        private static bool PingStatus()
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new ();

                System.Net.NetworkInformation.PingReply result = ping.Send("www.baidu.com");

                if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static (string, string) StopwatchWrapper(Func<bool> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            var ret = func();
            sw.Stop();
            return (ret.ToString(), sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
