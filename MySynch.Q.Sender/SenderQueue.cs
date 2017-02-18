using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common.Contracts.Management;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Sciendo.Common.Logging;
using System;
using System.Threading;

namespace MySynch.Q.Sender
{
    public class SenderQueue : ISenderQueue
    {

        public SenderQueue(QueueElement queueElement)
        {
            LoggingManager.Debug($"Constructing queue {queueElement.Name}...");
            QueueElement = queueElement;
        }
        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }

        private static object _lock = new object();
        public QueueElement QueueElement { get; set; }

        public void StartChannel(ConnectionFactory connectionFactory)
        {
            LoggingManager.Debug(QueueElement.Name + " on " + connectionFactory.HostName + " with user: " + connectionFactory.UserName + " Channel starting up...");
            try
            {
                lock (_lock)
                {
                    LoggingManager.Debug("Entering lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
                    if (Connection == null || !Connection.IsOpen)
                        Connection = connectionFactory.CreateConnection();
                    if (Channel == null || Channel.IsClosed)
                        Channel = Connection.CreateModel();
                    Channel.QueueDeclare(QueueElement.QueueName, true, false, true, null);
                    LoggingManager.Debug("Exiting lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
                }

                LoggingManager.Debug(QueueElement.Name + " Channel started up.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                LoggingManager.Debug(QueueElement.Name + " Channel NOT started up.");
            }
        }

        public void StopChannel()
        {
            LoggingManager.Debug(QueueElement.Name + " Channel shutting down...");
            lock (_lock)
            {
                LoggingManager.Debug("Entering lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
                if (Channel != null && !Channel.IsClosed)
                    Channel.Close();
                if (Connection != null && Connection.IsOpen)
                    Connection.Close();
                LoggingManager.Debug("Exeting lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
            }
            LoggingManager.Debug(QueueElement.Name + " Channel shutted down.");

        }

        public bool ShouldSendMessage(long minMem)
        {
            var url = @"http://" + QueueElement.HostName + ":15672/api/nodes/rabbit@" + QueueElement.HostName;
            LoggingManager.Debug("Using the api at: " + url);
            var nodeManagamentMessage = TryQuery(url);
            if (nodeManagamentMessage == null)
                return false;
            if (nodeManagamentMessage.disk_free_alarm || nodeManagamentMessage.mem_alarm)
                return false;
            if (nodeManagamentMessage.mem_limit - nodeManagamentMessage.mem_used <= minMem)
                return false;
            return true;
        }

        private NodeManagementMessage TryQuery(string url)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}",QueueElement.UserName,QueueElement.Password));
            var header = new AuthenticationHeaderValue(
                       "Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;

            using (var getTask = httpClient.GetStringAsync(new Uri(url))
                .ContinueWith(p => p).Result)
            {
                if (getTask.Status == TaskStatus.RanToCompletion || !string.IsNullOrEmpty(getTask.Result))
                {
                    return JsonConvert.DeserializeObject<NodeManagementMessage>(getTask.Result);
                }
                return null;
            }

        }

        public void SendMessage(byte[] message)
        {
            LoggingManager.Debug("Sending message to " + QueueElement.Name +"...");
            lock (_lock)
            {
                LoggingManager.Debug("Entering lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
                Channel.QueueDeclare(QueueElement.QueueName, true, false, true, null);
                Channel.BasicPublish("", QueueElement.QueueName, true, null, message);
                LoggingManager.Debug("Exiting lock for Thread: " + Thread.CurrentThread.ManagedThreadId);
            }
            LoggingManager.Debug("Message sent to " + QueueElement.Name + ".");
        }

    }
}
