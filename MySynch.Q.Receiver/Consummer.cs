using MySynch.Q.Common.Contracts;
using RabbitMQ.Client.Events;
using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Sciendo.Common.Serialization;

namespace MySynch.Q.Receiver
{
    internal class Consummer
    {
        private ReceiverQueue _receiverQueue;
        private string _rootPath;

        internal Consummer()
        {
            LoggingManager.Debug("Constructing Consummer...");
            var receiverConfig = ConfigurationManager.GetSection("receiver") as ReceiverSection;
            _rootPath = receiverConfig.LocalRootFolder;
            _receiverQueue = new ReceiverQueue 
                {   Name = receiverConfig.Name, 
                    QueueName = receiverConfig.QueueName, 
                    HostName = receiverConfig.HostName,
                    UserName=receiverConfig.UserName,
                    Password=receiverConfig.Password
                };
            LoggingManager.Debug("Consummer Constructed.");

        }

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Consummer ...");
                _receiverQueue.StartChannels();
                LoggingManager.Debug("Consummer Initialized.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }

        internal void Stop()
        {
            LoggingManager.Debug("Stoping Consumer...");
            More = false;
            Thread.Sleep(2000);
            _receiverQueue.StopChannels();
            LoggingManager.Debug("Publisher Stopped.");
        }

        internal void TryStart(object obj)
        {
            var messageApplyer = new MessageApplyer(_rootPath);
            More = true;
            LoggingManager.Debug("More: " + More);
            while(More)
            {
                messageApplyer.ApplyMessage(((BasicDeliverEventArgs)_receiverQueue.Consumer.Queue.Dequeue()).Body);
            }
        }

        public bool More { get; set; }
    }
}
