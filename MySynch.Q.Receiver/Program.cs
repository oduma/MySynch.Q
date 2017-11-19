using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;
using Sciendo.Common.Logging;
using Sciendo.IOC;
using Sciendo.IOC.Configuration;
using Sciendo.Playlist.Translator;
using Sciendo.Playlist.Translator.Configuration;
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
            var findAndReplaceParameters= GetSortedParams(((FindAndReplaceConfigSection)ConfigurationManager.GetSection("playlistTranslatorSection"))
                        .FromToParams
                        .Cast<FromToParamsElement>().Select(e => e).OrderBy(e => e.Priority));
            LoggingManager.Debug("Playlists configuration loaded.");
            Container.GetInstance().UsingConfiguration().AddFirstFromFilteredAssemblies<ITranslator>(LifeStyle.Transient, "textTranslators",findAndReplaceParameters);

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

        private static Dictionary<string, string> GetSortedParams(IOrderedEnumerable<FromToParamsElement> fromToParams)
        {
            var result = new Dictionary<string, string>();
            foreach (var fromToParam in fromToParams)
            {
                result.Add(fromToParam.From, fromToParam.To);
            }
            return result;
        }
    }
}

