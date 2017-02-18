using MySynch.Q.Common.Contracts;
using RabbitMQ.Client;
using Sciendo.Common.Logging;
using Sciendo.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySynch.Q.Sender
{
    public  class Publisher : IPublisher
    {
        public Publisher(ISenderQueue[] senderQueues, List<ConnectionFactory> connectionFactories, IMessageFeeder messageFeeder, long minFreeMemory)
        {
            LoggingManager.Debug("Constructing Publisher...");
            if(senderQueues==null || !senderQueues.Any())
                throw new ArgumentNullException(nameof(senderQueues));
            if(messageFeeder==null)
                throw new ArgumentNullException(nameof(messageFeeder));
            _senderQueues = senderQueues;
            _connectionFactories = (connectionFactories)?? new List<ConnectionFactory>();
            _messageFeeder = messageFeeder;
            _minFreeMemory = minFreeMemory;
            LoggingManager.Debug("Publisher Constructed.");
        }

        ISenderQueue[] _senderQueues;
        List<ConnectionFactory> _connectionFactories;

        public virtual void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Publisher...");
                
                foreach (var senderQueue in _senderQueues)
                {
                    var connectionFactory = _connectionFactories.FirstOrDefault(f => f.HostName == senderQueue.QueueElement.HostName);
                    if (connectionFactory == null)
                    {
                        connectionFactory = new ConnectionFactory
                        {
                            HostName = senderQueue.QueueElement.HostName,
                            UserName = senderQueue.QueueElement.UserName,
                            Password = senderQueue.QueueElement.Password
                        };
                        _connectionFactories.Add(connectionFactory);
                    }
                    senderQueue.StartChannel(connectionFactory);
                }
                LoggingManager.Debug("Publisher Initialized.");
            }
            catch(Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }

        public void TryStart()
        {
            if (!_messageFeeder.More)
                _messageFeeder.Initialize();
            this._messageFeeder.More = true;
            this._messageFeeder.PublishMessage = PublishMessage;
            this._messageFeeder.ShouldPublishMessage = ShouldPublishMessage;
        }

        internal virtual bool ShouldPublishMessage()
        {
            LoggingManager.Debug("Should Publish Message...");
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null && !q.Channel.IsClosed))
            {
                if (!senderQueue.ShouldSendMessage(_minFreeMemory))
                {
                    LoggingManager.Debug("Queue " + senderQueue.QueueElement.Name +" on " + senderQueue.QueueElement.HostName +" sais NO!");
                    return false;
                }
            }
            LoggingManager.Debug("All Queues can accespt messages");
            return true;
        }

        internal virtual void PublishMessage(BodyTransferMessage message)
        {
            LoggingManager.Debug("Publishing Message...");
            var tempMessage = Serializer.Serialize<BodyTransferMessage>(message);
            byte[] rawMessage = Encoding.UTF8.GetBytes(tempMessage);
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null))
            {
                if (senderQueue.Channel.IsClosed)
                {
                    LoggingManager.Debug("Channel closed reopening it...");
                    senderQueue.StartChannel(_connectionFactories.FirstOrDefault(f => f.HostName == senderQueue.QueueElement.HostName));
                }
                senderQueue.SendMessage(rawMessage);
            }
            LoggingManager.Debug("Message Published.");
        }
        public void Stop()
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

        private IMessageFeeder _messageFeeder;
        private long _minFreeMemory;
    }
}
