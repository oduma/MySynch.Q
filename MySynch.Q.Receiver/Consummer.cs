using Sciendo.Common.Logging;
using System;
using System.Threading;

namespace MySynch.Q.Receiver
{
    internal class Consummer
    {
        private readonly ReceiverQueue _receiverQueue;
        private readonly MessageApplyer _messageApplyer;
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationToken CancellationToken { get; set; }

        internal Consummer(MessageApplyer messageApplyer, ReceiverQueue receiverQueue)
        {
            LoggingManager.Debug("Constructing Consummer...");
            _messageApplyer = messageApplyer;
            _receiverQueue = receiverQueue;
            LoggingManager.Debug("Consummer Constructed.");

        }

        internal void Initialize(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                LoggingManager.Debug("Initializing Consummer ...");
                CancellationTokenSource = cancellationTokenSource;
                CancellationToken = CancellationTokenSource.Token;
                CancellationToken.Register(Stop);
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
            LoggingManager.Debug("Stoping _consumer...");
            More = false;
            Thread.Sleep(2000);
            _receiverQueue.StopChannels();
            LoggingManager.Debug("Publisher Stopped.");
        }

        internal void TryStart(object obj)
        {
            More = true;
            LoggingManager.Debug("More: " + More);
            while(More)
            {
                var message = _receiverQueue.GetMessage();
                _messageApplyer.ApplyMessage(message);
            }
        }

        public bool More { get; set; }
    }
}
