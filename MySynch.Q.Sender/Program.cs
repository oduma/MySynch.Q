﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;
using RabbitMQ.Client;
using Topshelf;

namespace MySynch.Q.Sender
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var senderConfig = ConfigurationManager.GetSection("sender") as SenderSection;
            HostFactory.Run(x =>
            {
                x.Service<SenderService>(
                    s =>
                    {
                        s.ConstructUsing(name => new SenderService(new Publisher(senderConfig.Queues.Cast<QueueElement>()
                                .Select(
                                    q =>
                                        new SenderQueue
                                        (q)), new List<ConnectionFactory>(),
                            new MessageFeeder(senderConfig.MaxFileSize,
                                new DirectoryMonitor(senderConfig.LocalRootFolder), senderConfig.LocalRootFolder,new IOOperations()),
                            Convert.ToInt64(senderConfig.MinFreeMemory))));  
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenShutdown(tc => tc.Shutdown());
                        s.WhenContinued(tc => tc.Continue());
                        s.WhenPaused(tc => tc.Pause());
                    });
                x.RunAsLocalSystem();

#if DEBUG
                x.SetServiceName("SciendoSynchSenderDebug");
                x.SetDisplayName("Sciendo Synch Sender (Debug)");
                x.SetDescription("Sends messages when files located at change (Debug)");
#else
                x.SetServiceName("MySynch.Q.Sender");
                x.SetDisplayName("MySynch Queue Sender");
                x.SetDescription("Sends messages when files located at change");
#endif

            });
        }
    }
}
