using System;
using System.Threading.Tasks;
using Sciendo.Common.Logging;
using System.Threading;

namespace MySynch.Q.Sender
{
    public partial class SenderService
    {
        private Publisher _sender;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public SenderService()
        {
            LoggingManager.Debug("Constructing Sender...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _sender = new Publisher();
            LoggingManager.Debug("Sender constructed.");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingManager.Debug("An unhandled exception has happened. See the exceptions log.");
            LoggingManager.LogSciendoSystemError(e.ExceptionObject as Exception);
        }

        public void Start()
        {
            LoggingManager.Debug("Starting Sender...");
            _sender.Initialize();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _cancellationToken.Register(_sender.Stop);
            Task sendTask = new Task(_sender.TryStart,_cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Sender started.");
        }

        public void Stop()
        {
            LoggingManager.Debug("Stoping Sender...");

            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Sender stopped.");

        }

        public void Continue()
        {
            LoggingManager.Debug("Starting Sender...");
            Task sendTask = new Task(_sender.TryStart, _cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Sender started.");
        }

        public void Pause()
        {
            LoggingManager.Debug("Stoping Sender...");
            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Sender stoped.");
        }

        public void Shutdown()
        {
            LoggingManager.Debug("Stoping Sender...");
            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Sender stopped.");
        }

    }
}
