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
using MySynch.Q.Sender.Configuration;

namespace MySynch.Q.Sender
{
    public class SenderQueue:QueueElement
    {

        public IConnection Connection { get; set; }

        public IModel Channel { get; set; }

        private static object _lock = new object();

        private ConnectionFactory _connectionFactory;

        public void StartChannel()
        {
            try
            {
                LoggingManager.Debug(Name + " on " + _connectionFactory.HostName + " with user: " + _connectionFactory.UserName + " Channel starting up...");
                if (_connectionFactory == null)
                {
                    _connectionFactory = new ConnectionFactory { HostName = HostName, UserName = UserName, Password = Password };
                }
                if (Connection == null || !Connection.IsOpen)
                    Connection = _connectionFactory.CreateConnection();
                if (Channel == null || Channel.IsClosed)
                    Channel = Connection.CreateModel();
                Channel.QueueDeclare(QueueName, true, false, true, null);
                LoggingManager.Debug(Name + " Channel started up.");
            }
            catch (Exception exception)
            {
                LoggingManager.LogSciendoSystemError(exception);
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

        public bool ShouldSendMessage(string minMem)
        {
            var url = @"http://" + HostName + ":15672/api/nodes/rabbit@" + HostName;
            LoggingManager.Debug("Using the api at: " + url);
            var nodeManagamentMessage = TryQuery(url);
            if (nodeManagamentMessage == null)
                return false;
            if (nodeManagamentMessage.disk_free_alarm || nodeManagamentMessage.mem_alarm)
                return false;
            if (nodeManagamentMessage.mem_limit - nodeManagamentMessage.mem_used <= Convert.ToInt64(minMem))
                return false;
            return true;
        }

        private NodeManagementMessage TryQuery(string url)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}",UserName,Password));
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
            LoggingManager.Debug("Sending message to " + Name +"...");
            StartChannel();
            Channel.BasicPublish("", QueueName, true, null, message);
            LoggingManager.Debug("Message sent to " + Name + ".");
        }

    }
}
