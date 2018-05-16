using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;

namespace DirectoryMonitorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryMonitor directoryMonitor =
                new DirectoryMonitor(@"C:\Code\work\Sciendo\MySynch.Q\Source-Debug\Music");
            directoryMonitor.Start();
            Console.WriteLine("Press a key to exit finish and send the events:");
            Console.ReadKey();
            directoryMonitor.FireEventChangeEvents();
            Console.ReadKey();
        }
    }
}