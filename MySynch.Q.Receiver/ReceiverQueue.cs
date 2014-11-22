using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Q.Receiver
{
    public class ReceiverQueue:ReceiverSection
    {

        public void StartChannel()
        {
            LoggingManager.Debug(Name + " Channel starting up...");

            try
            {
               
                Connection = new ConnectionFactory { HostName = HostName,UserName=UserName,Password=Password }.CreateConnection();
                Channel = Connection.CreateModel();
                Channel.QueueDeclare(QueueName, true, false, false, null);

                Consumer = new QueueingBasicConsumer(Channel);
                Channel.BasicConsume(QueueName, true, Consumer);

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

        internal void StopChannel()
        {
            LoggingManager.Debug(Name + " Channel shutting down...");

            if (Channel != null && !Channel.IsClosed)
                Channel.Close();
            if (Connection != null && Connection.IsOpen)
                Connection.Close();
            LoggingManager.Debug(Name + " Channel shutted down.");
        }

        public QueueingBasicConsumer Consumer { get; set; }
    }
}
