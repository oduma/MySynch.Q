using RabbitMQ.Client.Events;
using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Receiver
{
    internal class Consummer
    {
        private ReceiverQueue _receiverQueue;

        internal Consummer()
        {
            LoggingManager.Debug("Constructing Consummer...");
            var receiverConfig = ConfigurationManager.GetSection("receiver") as ReceiverSection;
            _receiverQueue = new ReceiverQueue 
                {   Name = receiverConfig.Name, 
                    QueueName = receiverConfig.QueueName, 
                    HostName = receiverConfig.HostName 
                };
            LoggingManager.Debug("Consummer Constructed.");

        }

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Consummer ...");
                _receiverQueue.StartChannel();
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
            _receiverQueue.StopChannel();
            LoggingManager.Debug("Publisher Stopped.");
        }

        internal void TryStart(object obj)
        {
            More = true;
            while(More)
            {
                new MessageApplyer().ApplyMessage(((BasicDeliverEventArgs)_receiverQueue.Consumer.Queue.Dequeue()).Body);
            }
        }

        public bool More { get; set; }
    }
}
