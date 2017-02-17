using Sciendo.Common.Logging;
using System;
using System.CodeDom;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using MySynch.Q.Common.Contracts;
using Sciendo.Common.Serialization;

namespace MySynch.Q.Receiver
{
    public class Consummer : IConsummer
    {
        private ReceiverQueue _receiverQueue;
        private string _rootPath;
        private MessageApplyer _messageApplyer;

        public Consummer(ReceiverQueue receiverQueue, MessageApplyer messageApplyer, string rootPath)
        {
            LoggingManager.Debug("Constructing Consummer...");
            if(string.IsNullOrEmpty(rootPath))
                throw new ArgumentNullException(nameof(rootPath));
            if(receiverQueue==null)
                throw new ArgumentNullException(nameof(receiverQueue));
            if(messageApplyer==null)
                throw new ArgumentNullException(nameof(messageApplyer));
            if (!Directory.Exists(rootPath))
                throw new ArgumentException("Root Path not found.", nameof(rootPath));
            _rootPath = rootPath;
            _receiverQueue = receiverQueue;
            _messageApplyer = messageApplyer;
            LoggingManager.Debug("Consummer Constructed.");

        }

        public void Initialize()
        {
            try
            {
                LoggingManager.Debug("Initializing Consummer ...");
                _receiverQueue.StartChannels();
                LoggingManager.Debug("Consummer Initialized.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }
        }

        public void Stop()
        {
            LoggingManager.Debug("Stoping Consumer...");
            More = false;
            Thread.Sleep(2000);
            _receiverQueue.StopChannels();
            LoggingManager.Debug("Consumer Stopped.");
        }

        public void TryStart(object obj)
        {
            More = true;
            LoggingManager.Debug("More: " + More);
            while(More)
            {
                var messageBytes = _receiverQueue.GetMessage();
                if(messageBytes==null || messageBytes.Length<=0)
                    LoggingManager.Debug("Empty message taken from the queue.");
                else
                {
                    var message = Serializer.Deserialize<BodyTransferMessage>(Encoding.UTF8.GetString(messageBytes));
                    if (message == null)
                        LoggingManager.Debug("Empty message after the deserialization.");
                    try
                    {
                        _messageApplyer.ApplyMessage(message);
                    }
                    catch (Exception e)
                    {
                        LoggingManager.LogSciendoSystemError("Message not applied.",e);
                    }
                }
            }
        }

        public bool More { get; set; }
    }
}
