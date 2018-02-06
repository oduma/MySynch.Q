using Sciendo.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySynch.Q.Receiver.Configuration;
using Sciendo.Common.Music.Contracts;
using Sciendo.IOC;
using Sciendo.IOC.Configuration;
using Sciendo.Playlist.Translator;

namespace MySynch.Q.Receiver
{
    public partial class ReceiverService
    {
        private readonly List<Consummer> _consummers;
        private readonly ITranslator[] _translators;

        public ReceiverService(ITranslator[] translators)
        {
            LoggingManager.Debug("Constructing Receivers...");
            this._translators = translators;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _consummers = LoadAllConsummers();

            LoggingManager.Debug("Receivers constructed.");
        }

        private List<Consummer> LoadAllConsummers()
        {
            var consummers = new List<Consummer>();
            

            foreach (var receiver in ((ReceiversSection)ConfigurationManager.GetSection("receiversSection")).Receivers.Cast<ReceiverElement>())
            {

                consummers.Add(new Consummer(new MessageApplyer(receiver.LocalRootFolder,_translators,GetPostProcessors(receiver).ToArray()), new ReceiverQueue
                {
                    Name = receiver.Name,
                    QueueName = receiver.QueueName,
                    HostName = receiver.HostName,
                    UserName = receiver.UserName,
                    Password = receiver.Password
                }));
            }

            return consummers;
        }

        private IEnumerable<IPostProcessor> GetPostProcessors(ReceiverElement receiver)
        {
            if (receiver.PostProcessors == null || receiver.PostProcessors.Count == 0)
                yield return null;
            else
            {
                foreach ( var postProcessorElement in receiver.PostProcessors.Cast<PostProcessorElement>())
                {
                    IPostProcessor postProcessor;
                    try
                    {
                        postProcessor = Container.GetInstance().Resolve<IPostProcessor>(postProcessorElement.Value);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Container.GetInstance().UsingConfiguration().AddAllFromFilteredAssemblies<IPostProcessor>(LifeStyle.Transient);
                            postProcessor = Container.GetInstance().Resolve<IPostProcessor>(postProcessorElement.Value);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            throw;
                        }
                    }
                    yield return postProcessor;
                }
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingManager.Debug("An unhandled exception has happened. See the exceptions log.");
            LoggingManager.LogSciendoSystemError(e.ExceptionObject as Exception);
        }

        public void Start()
        {
            LoggingManager.Debug("Starting Receivers...");
            foreach (var consummer in _consummers)
            {
                consummer.Initialize(new CancellationTokenSource());
                Task currentReceiveTask = new Task(consummer.TryStart, consummer.CancellationToken);
                currentReceiveTask.Start();
            }
            LoggingManager.Debug("Receivers started.");
        }

        public void Stop()
        {
            LoggingManager.Debug("Stoping Receivers...");
            foreach (var consummer in _consummers)
            {
                consummer.CancellationTokenSource.Cancel();
            }
            LoggingManager.Debug("Receivers stopped.");
        }
        public void Pause()
        {
            LoggingManager.Debug("Stoping Receivers...");

            Stop();
            LoggingManager.Debug("Receivers stopped.");

        }

        public void Continue()
        {
            LoggingManager.Debug("Starting Receivers...");
            Start();
            LoggingManager.Debug("Receivers started.");
        }

        public void Shutdown()
        {
            LoggingManager.Debug("Stoping Receivers...");
            Stop();
            LoggingManager.Debug("Receivers stopped.");
        }
    }
}
