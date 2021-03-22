using System;
using System.Diagnostics;

namespace Benchmark
{
    public class SystemProcess
    {
        internal SystemProcess()
        {
            var sw = new Stopwatch();
            // warm-up
            _ = Process.GetProcesses();
            _ = Process.GetProcessesByName("notepad");


            sw.Restart();
            var allProc = Process.GetProcesses();
            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms {sw.ElapsedTicks}");

            sw.Restart();
            var namedProc = Process.GetProcessesByName("notepad");
            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms {sw.ElapsedTicks}");

        }
    }
}