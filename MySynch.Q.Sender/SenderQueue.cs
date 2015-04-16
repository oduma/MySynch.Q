using RabbitMQ.Client;
using Sciendo.Common.Logging;
using System;

namespace MySynch.Q.Sender
{
    public class SenderQueue:QueueElement
    {

        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }


        public void StartChannel(ConnectionFactory connectionFactory)
        {
            LoggingManager.Debug(Name + " on " + connectionFactory.HostName + " with user: " + connectionFactory.UserName + " Channel starting up...");
            try
            {
                Connection = connectionFactory.CreateConnection();
                Channel = Connection.CreateModel();
                Channel.QueueDeclare(QueueName, true, false, true, null);
                LoggingManager.Debug(Name + " Channel started up.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                LoggingManager.Debug(Name + " Channel NOT started up.");
            }
        }

        public void StopChannel()
        {
            LoggingManager.Debug(Name + " Channel shutting down...");

            if (Channel != null && !Channel.IsClosed)
                Channel.Close();
            if (Connection != null && Connection.IsOpen)
                Connection.Close();
            LoggingManager.Debug(Name + " Channel shutted down.");

        }

        public void SendMessage(byte[] message)
        {
            LoggingManager.Debug("Sending message to " + Name +"...");
            Channel.BasicPublish("", QueueName, true, null, message);
            LoggingManager.Debug("Message sent to " + Name + ".");
        }

    }
}
