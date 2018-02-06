using System;
using Sciendo.Common.Logging;
using Sciendo.Common.Music.Contracts;
using Sciendo.IOC;
using Sciendo.IOC.Configuration;
using Sciendo.Playlist.Translator;
using Topshelf;

namespace MySynch.Q.Receiver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LoggingManager.Debug("Loading playlists configuration ...");
            Container.GetInstance().UsingConfiguration().AddFirstFromFilteredAssemblies<ITranslator>(LifeStyle.Transient, "textTranslators");

            HostFactory.Run(x =>
            {
                x.Service<ReceiverService>(
                    s =>
                    {
                        s.ConstructUsing(name => new ReceiverService(Container.GetInstance().ResolveAll<ITranslator>()));
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenShutdown(tc => tc.Shutdown());
                        s.WhenContinued(tc => tc.Continue());
                        s.WhenPaused(tc => tc.Pause());
                    });
                x.RunAsLocalSystem();
            });
        }

    }
}

