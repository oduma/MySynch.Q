using RabbitMQ.Client;

namespace MySynch.Q.Sender
{
    public interface ISenderQueue
    {
        IConnection Connection { get; set; }
        IModel Channel { get; set; }
        void StartChannel(ConnectionFactory connectionFactory);
        void StopChannel();
        bool ShouldSendMessage(string minMem);
        void SendMessage(byte[] message);

        QueueElement QueueElement { get; set; }
    }
}