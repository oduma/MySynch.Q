using Sciendo.Common.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySynch.Q.Receiver
{
    public partial class ReceiverService
    {
        private Consummer _receiver;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public ReceiverService(Consummer consummer )
        {
            LoggingManager.Debug("Constructing Receiver...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _receiver = consummer;
            LoggingManager.Debug("Receiver constructed.");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingManager.Debug("An unhandled exception has happened. See the exceptions log.");
            LoggingManager.LogSciendoSystemError(e.ExceptionObject as Exception);
        }

        public void Start()
        {
            LoggingManager.Debug("Starting Receiver...");
            _receiver.Initialize();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _cancellationToken.Register(_receiver.Stop);
            Task receiveTask = new Task(_receiver.TryStart, _cancellationToken);
            receiveTask.Start();
            LoggingManager.Debug("Receiver started.");
        }

        public void Stop()
        {
            LoggingManager.Debug("Stoping Receiver...");

            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Receiver stopped.");

        }
        public void Pause()
        {
            LoggingManager.Debug("Stoping Receiver...");

            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Receiver stopped.");

        }

        public void Continue()
        {
            LoggingManager.Debug("Starting Receiver...");
            Task sendTask = new Task(_receiver.TryStart, _cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Receiver started.");
        }

        public void Shutdown()
        {
            LoggingManager.Debug("Stoping Receiver...");
            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Receiver stopped.");
        }
    }
}
