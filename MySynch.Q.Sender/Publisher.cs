using MySynch.Q.Common.Contracts;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Sender
{
    internal class Publisher
    {

        private readonly string _minFreeMemory;

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public CancellationToken CancellationToken { get; set; }

        internal Publisher(SenderQueue[] senderQueues, string minFreeMemory, MessageFeeder messageFeeder)
        {
            LoggingManager.Debug("Constructing Publisher...");
            _senderQueues = senderQueues;
            _minFreeMemory = minFreeMemory;
            _messageFeeder = messageFeeder;
            LoggingManager.Debug("Publisher Constructed.");
        }

        private readonly SenderQueue[] _senderQueues;

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Publisher...");
                
                foreach (var senderQueue in _senderQueues)
                {
                    senderQueue.StartChannel();
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
            _messageFeeder.Initialize(true, PublishMessage, ShouldPublishMessage);
        }

        private bool ShouldPublishMessage()
        {
            LoggingManager.Debug("Should Publish Message...");
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null && !q.Channel.IsClosed))
            {
                if (!senderQueue.ShouldSendMessage(_minFreeMemory))
                {
                    LoggingManager.Debug("Queue " + senderQueue.Name + " on " + senderQueue.HostName + " sais NO!");
                    return false;
                }
            }
            LoggingManager.Debug("All Queues can accespt messages");
            return true;
        }

        internal void PublishMessage(BodyTransferMessage message)
        {
            LoggingManager.Debug("Publishing Message...");
            var tempMessage = Serializer.Serialize<BodyTransferMessage>(message);
            byte[] rawMessage = Encoding.UTF8.GetBytes(tempMessage);
            foreach (var senderQueue in _senderQueues)
            {
                senderQueue.SendMessage(rawMessage);
            }
            LoggingManager.Debug("Message Published.");
        }
        internal void Stop()
        {
            LoggingManager.Debug("Stoping Publisher...");
            _messageFeeder.More = false;
            Thread.Sleep(2000);
            foreach (var senderQueue in _senderQueues)
            {
                senderQueue.StopChannel();
            }

            LoggingManager.Debug("Publisher Stopped.");
        }

        private readonly MessageFeeder _messageFeeder;
    }
}
