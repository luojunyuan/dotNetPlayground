using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NetworkChecker
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(StopwatchWrapper(CheckNet().ToString));
            Console.WriteLine(StopwatchWrapper(IsInternetAvailable().ToString));
            Console.WriteLine(StopwatchWrapper(IsConnected().ToString));

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += (sender, eventArgs) =>
            {
                if (eventArgs.IsAvailable)
                    Console.WriteLine("Network connected!");
                else
                    Console.WriteLine("Network dis-connected!");
            };
            Console.ReadKey();
        }

        /// <summary>Always True</summary>
        private static bool CheckNet()
        {
            var stats = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            return stats;
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        private static bool IsInternetAvailable()
        {
            return InternetGetConnectedState(out _, 0);
        }

        private static readonly NETWORKLIST.INetworkListManager _networkListManager = new NETWORKLIST.NetworkListManager();

        private static bool IsConnected() => _networkListManager.IsConnectedToInternet;

        private static string StopwatchWrapper(Func<string> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            var ret = func();
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);
            return ret;
        }
    }
}
