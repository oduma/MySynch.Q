using RabbitMQ.Client;
using Sciendo.Common.Logging;
using System;
using MySynch.Q.Receiver.Configuration;
using RabbitMQ.Client.Events;

namespace MySynch.Q.Receiver
{
    public class ReceiverQueue:ReceiverElement
    {

        public void StartChannels()
        {
            LoggingManager.Debug(Name + " Channel starting up...");

            try
            {
                if (Connection == null || !Connection.IsOpen)
                    Connection = new ConnectionFactory { HostName = HostName, UserName = UserName, Password = Password }.CreateConnection();
                if (Channel == null || !Channel.IsOpen)
                    Channel = Connection.CreateModel();

                Channel.QueueDeclare(QueueName, true, false, true, null);

                _consumer = new QueueingBasicConsumer(Channel);
                Channel.BasicConsume(QueueName, true, _consumer);
                LoggingManager.Debug(Name + " Channel started up.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                LoggingManager.Debug(Name + " Channel NOT started up.");
            }

        }

        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }

        internal void StopChannels()
        {
            LoggingManager.Debug(Name + " Channel shutting down...");

            if (Channel != null && !Channel.IsClosed)
                Channel.Close();
            if (Connection != null && Connection.IsOpen)
                Connection.Close();
            LoggingManager.Debug(Name + " Channel shutted down.");
        }

        private QueueingBasicConsumer _consumer;

        public byte[] GetMessage()
        {
            Channel.QueueDeclare(QueueName, true, false, true, null);
            return ((BasicDeliverEventArgs)_consumer.Queue.Dequeue()).Body;
        } 
    }
}
