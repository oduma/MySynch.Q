using System;
using System.Threading.Tasks;
using Sciendo.Common.Logging;
using System.Threading;

namespace MySynch.Q.Sender
{
    public class SenderService : ISenderService
    {
        private IPublisher _publisher;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public SenderService(IPublisher publisher)
        {
            if(publisher==null)
                throw new ArgumentNullException(nameof(publisher));
            LoggingManager.Debug("Constructing Sender...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _publisher = publisher;
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
            _publisher.Initialize();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _cancellationToken.Register(_publisher.Stop);
            Task sendTask = new Task(_publisher.TryStart,_cancellationToken);
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
            Task sendTask = new Task(_publisher.TryStart, _cancellationToken);
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
