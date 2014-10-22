using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySynch.Q.Receiver
{
    public partial class ReceiverService : ServiceBase
    {
        private Consummer _receiver;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public ReceiverService()
        {
            LoggingManager.Debug("Constructing Receiver...");
            InitializeComponent();
            _receiver = new Consummer();
            LoggingManager.Debug("Receiver constructed.");
        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting Receiver...");
            _receiver.Initialize();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _cancellationToken.Register(_receiver.Stop);
            Task sendTask = new Task(_receiver.TryStart, _cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Receiver started.");
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping Receiver...");

            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Receiver stopped.");

        }

        protected override void OnContinue()
        {
            LoggingManager.Debug("Starting Receiver...");
            base.OnContinue();
            Task sendTask = new Task(_receiver.TryStart, _cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Receiver started.");
        }

        protected override void OnShutdown()
        {
            LoggingManager.Debug("Stoping Receiver...");
            _cancellationTokenSource.Cancel();
            base.OnShutdown();
            LoggingManager.Debug("Receiver stopped.");
        }
    }
}
