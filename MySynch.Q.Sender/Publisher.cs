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
            LoggingManager.Debug("Publisher Constructed.");
        }

        QueueElementCollection _senderQueues;

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Publisher...");
                SenderSection senderConfig = ConfigurationManager.GetSection("sender") as SenderSection;
                _senderQueues = senderConfig.Queues;
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
            foreach(QueueElement senderQueue in _senderQueues)
            {
                LoggingManager.Debug("Send to " + senderQueue.Name + " on " + senderQueue.HostName);
            }
            LoggingManager.Debug("Message Published.");
        }
        internal void Stop()
        {
            LoggingManager.Debug("Stoping Publisher...");
            MessageFeeder.More = false;
            Thread.Sleep(2000);
            LoggingManager.Debug("Publisher Stopped.");
        }

        public MessageFeeder MessageFeeder { get; set; }
    }
}
