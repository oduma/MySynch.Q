using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using Topshelf;
using Sciendo.Common.Logging;

namespace MySynch.Q.Sender
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<SenderService>(
                    s =>
                    {
                        s.ConstructUsing(name => new SenderService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenShutdown(tc => tc.Shutdown());
                        s.WhenContinued(tc => tc.Continue());
                        s.WhenPaused(tc => tc.Pause());

                    });
                x.RunAsLocalSystem();
            });
        }
    }
}
