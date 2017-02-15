using Sciendo.Common.Logging;
using System;
using System.Configuration;
using System.Threading;

namespace MySynch.Q.Receiver
{
    public class Consummer
    {
        private ReceiverQueue _receiverQueue;
        private string _rootPath;
        private MessageApplyer _messageApplyer;

        internal Consummer(ReceiverQueue receiverQueue, MessageApplyer messageApplyer, string rootPath)
        {
            LoggingManager.Debug("Constructing Consummer...");
            _rootPath = rootPath;
            _receiverQueue = receiverQueue;
            _messageApplyer = messageApplyer;
            LoggingManager.Debug("Consummer Constructed.");

        }

        internal void Initialize()
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

        internal void Stop()
        {
            LoggingManager.Debug("Stoping Consumer...");
            More = false;
            Thread.Sleep(2000);
            _receiverQueue.StopChannels();
            LoggingManager.Debug("Consumer Stopped.");
        }

        internal void TryStart(object obj)
        {
            More = true;
            LoggingManager.Debug("More: " + More);
            while(More)
            {
                _messageApplyer.ApplyMessage(_receiverQueue.GetMessage());
            }
        }

        public bool More { get; set; }
    }
}
