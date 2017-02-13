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
    internal class Publisher
    {
        private SenderSection _senderConfig;

        internal Publisher()
        {
            LoggingManager.Debug("Constructing Publisher...");
            _senderConfig = ConfigurationManager.GetSection("sender") as SenderSection;
            _senderQueues =
                _senderConfig.Queues.Cast<QueueElement>()
                    .Select(
                        q =>
                            new SenderQueue
                            {
                                Name = q.Name,
                                QueueName = q.QueueName,
                                HostName = q.HostName,
                                UserName = q.UserName,
                                Password = q.Password
                            })
                    .ToArray();
            _connectionFactories = new List<ConnectionFactory>();
            LoggingManager.Debug("Publisher Constructed.");
        }

        SenderQueue[] _senderQueues;
        List<ConnectionFactory> _connectionFactories;

        internal void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Publisher...");
                
                foreach (var senderQueue in _senderQueues)
                {
                    var connectionFactory = _connectionFactories.FirstOrDefault(f => f.HostName == senderQueue.HostName);
                    if (connectionFactory == null)
                    {
                        connectionFactory = new ConnectionFactory { HostName = senderQueue.HostName, UserName = senderQueue.UserName, Password = senderQueue.Password };
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

        internal void TryStart()
        {
            if (_messageFeeder == null)
                _messageFeeder = new MessageFeeder();
            if (!_messageFeeder.More)
                _messageFeeder.Initialize(_senderConfig.LocalRootFolder);
            this._messageFeeder.More = true;
            this._messageFeeder.PublishMessage = PublishMessage;
            this._messageFeeder.ShouldPublishMessage = ShouldPublishMessage;
        }

        private bool ShouldPublishMessage()
        {
            LoggingManager.Debug("Should Publish Message...");
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null && !q.Channel.IsClosed))
            {
                if (!senderQueue.ShouldSendMessage(_senderConfig.MinFreeMemory))
                {
                    LoggingManager.Debug("Queue " + senderQueue.Name +" on " + senderQueue.HostName +" sais NO!");
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
            foreach (var senderQueue in _senderQueues.Where(q => q.Channel != null))
            {
                if (senderQueue.Channel.IsClosed)
                {
                    LoggingManager.Debug("Channel closed reopening it...");
                    senderQueue.StartChannel(_connectionFactories.FirstOrDefault());
                }
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

        private MessageFeeder _messageFeeder;
    }
}
