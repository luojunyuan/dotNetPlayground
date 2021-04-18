using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using RestSharp;

namespace dotNetPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] temp = Process.GetProcessesByName("DeepL");
            if (temp.Length == 0)
                return;

            var handle = temp[0].MainWindowHandle;
            NativeMethods.SwitchToThisWindow(handle);

            if (NativeMethods.GetForegroundWindow() != handle)
            {
                Console.WriteLine("Try open");
                Process.Start(temp[0].MainModule?.FileName);
            }
        }
    }
}
