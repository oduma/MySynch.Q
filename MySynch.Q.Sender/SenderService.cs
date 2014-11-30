﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Sciendo.Common.Logging;
using System.Threading;

namespace MySynch.Q.Sender
{
    public partial class SenderService : ServiceBase
    {
        private Publisher _sender;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public SenderService()
        {
            LoggingManager.Debug("Constructing Sender...");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeComponent();
            _sender = new Publisher();
            LoggingManager.Debug("Sender constructed.");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingManager.Debug("An unhandled exception has happened. See the exceptions log.");
            LoggingManager.LogSciendoSystemError(e.ExceptionObject as Exception);
        }

        protected override void OnStart(string[] args)
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

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping Sender...");

            _cancellationTokenSource.Cancel();
            LoggingManager.Debug("Sender stopped.");

        }

        protected override void OnContinue()
        {
            LoggingManager.Debug("Starting Sender...");
            base.OnContinue();
            Task sendTask = new Task(_sender.TryStart, _cancellationToken);
            sendTask.Start();
            LoggingManager.Debug("Sender started.");
        }

        protected override void OnShutdown()
        {
            LoggingManager.Debug("Stoping Sender...");
            _cancellationTokenSource.Cancel();
            base.OnShutdown();
            LoggingManager.Debug("Sender stopped.");
        }

    }
}
