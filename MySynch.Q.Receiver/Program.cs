using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace MySynch.Q.Receiver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<ReceiverService>(
                    s =>
                    {
                        s.ConstructUsing(name => new ReceiverService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenShutdown(tc => tc.Shutdown());
                        s.WhenContinued(tc => tc.Continue());
                        s.WhenPaused(tc => tc.Pause());
                    });
                x.RunAsLocalSystem();

#if DEBUG
                x.SetServiceName("Sciendo Synch Receiver (Debug)");
                x.SetDisplayName("Sciendo Synch Receiver (Debug)");
                x.SetDescription("Receives messages from  a queue and persists files to folder. (Debug)");
#else
                x.SetServiceName("MySynch.Q.Receiver");
                x.SetDisplayName("MySynch Queue Receiver");
                x.SetDescription("Receives messages from  a queue and persists files to folder.");
#endif
            });
        }
    }
}

