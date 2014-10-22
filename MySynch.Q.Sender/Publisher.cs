using RabbitMQ.Client;
using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Sender
{
    internal class Publisher
    {
        internal Publisher()
        {
            LoggingManager.Debug("Constructing Publisher...");
            SenderSection senderConfig = ConfigurationManager.GetSection("sender") as SenderSection;
            _senderQueues = senderConfig.Queues.Cast<QueueElement>().Select(q=>new SenderQueue{Name=q.Name,QueueName=q.QueueName,HostName=q.HostName}).ToArray();
            LoggingManager.Debug("Publisher Constructed.");
        }

        SenderQueue[] _senderQueues;
        IEnumerable<ConnectionFactory> _connectionFactories;

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Publisher...");
                _connectionFactories = _senderQueues.Select(q => q.HostName).Distinct().Select((h) => new ConnectionFactory { HostName = h });
                foreach (var senderQueue in _senderQueues)
                {
                    senderQueue.StartChannel(_connectionFactories.FirstOrDefault(f => f.HostName == senderQueue.HostName));
                }
                LoggingManager.Debug("Publisher Initialized.");
            }
            catch(Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }

        internal void TryStart()
        {
            this.MessageFeeder.More = true;
            this.MessageFeeder.FeedMessagesTo(PublishMessage);
        }

        internal void PublishMessage(byte[] message)
        {
            LoggingManager.Debug("Publishing Message...");
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null && !q.Channel.IsClosed))
            {
                senderQueue.SendMessage(message);
            }
            LoggingManager.Debug("Message Published.");
        }
        internal void Stop()
        {
            LoggingManager.Debug("Stoping Publisher...");
            MessageFeeder.More = false;
            Thread.Sleep(2000);
            foreach (var senderQueue in _senderQueues)
            {
                senderQueue.StopChannel();
            }

            LoggingManager.Debug("Publisher Stopped.");
        }

        public MessageFeeder MessageFeeder { get; set; }
    }
}
