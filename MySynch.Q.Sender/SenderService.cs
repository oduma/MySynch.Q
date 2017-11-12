using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Sciendo.Common.Logging;
using System.Threading;
using MySynch.Q.Sender.Configuration;

namespace MySynch.Q.Sender
{
    public partial class SenderService
    {
        private List<Publisher> _publishers;
        public SenderService()
        {
            LoggingManager.Debug("Constructing Sender...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _publishers = LoadAllPublishers();
            LoggingManager.Debug("Sender constructed.");
        }

        private List<Publisher> LoadAllPublishers()
        {
            var publishers = new List<Publisher>();
            foreach (
                var senderConfig in
                ((SendersSection)ConfigurationManager.GetSection("sendersSection")).Senders.Cast<SenderElement>())
            {
                publishers.Add(
                    new Publisher(
                        senderConfig.Queues.Cast<QueueElement>()
                            .Select(
                                q =>
                                    new SenderQueue
                                    {
                                        Name = q.Name,
                                        HostName = q.HostName,
                                        UserName = q.UserName,
                                        Password = q.Password
                                    })
                            .ToArray(), senderConfig.MinFreeMemory, new MessageFeeder(senderConfig.LocalRootFolder)));
            }
            return publishers;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingManager.Debug("An unhandled exception has happened. See the exceptions log.");
            LoggingManager.LogSciendoSystemError(e.ExceptionObject as Exception);
        }

        public void Start()
        {
            LoggingManager.Debug("Starting Senders...");
            foreach (var publisher in _publishers)
            {
                publisher.Initialize();
                publisher.CancellationTokenSource=new CancellationTokenSource();
                publisher.CancellationToken = publisher.CancellationTokenSource.Token;
                publisher.CancellationToken.Register(publisher.Stop);
                Task currentSendTask= new Task(publisher.TryStart,publisher.CancellationToken);
                currentSendTask.Start();
            }
            LoggingManager.Debug("Senders started.");
        }

        public void Stop()
        {
            LoggingManager.Debug("Stoping Senders...");

            foreach (var publisher in _publishers)
            {
                publisher.CancellationTokenSource.Cancel();
            }
            LoggingManager.Debug("Senders stopped.");
        }

        public void Continue()
        {
            LoggingManager.Debug("Starting Senders...");
            Start();
            LoggingManager.Debug("Senders started.");
        }

        public void Pause()
        {
            LoggingManager.Debug("Stoping Senders...");
            Stop();
            LoggingManager.Debug("Senders stoped.");
        }

        public void Shutdown()
        {
            LoggingManager.Debug("Stoping Senders...");
            Stop();
            LoggingManager.Debug("Senders stopped.");
        }

    }
}
