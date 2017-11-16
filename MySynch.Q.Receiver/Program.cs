using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common;
using Sciendo.IOC;
using Sciendo.IOC.Configuration;
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
            var findAndReplaceParameters= GetSortedParams(((FindAndReplaceConfigSection)ConfigurationManager.GetSection("playlistTranslatorSection"))
                        .FromToParams
                        .Cast<FromToParamsElement>().Select(e => e).OrderBy(e => e.Priority));
            var configuredContainer = Container.GetInstance().UsingConfiguration().AddFirstFromFilteredAssemblies<IMessageTranslator>(LifeStyle.Transient, "textTranslators",findAndReplaceParameters);

            HostFactory.Run(x =>
            {
                x.Service<ReceiverService>(
                    s =>
                    {
                        s.ConstructUsing(name => new ReceiverService(Container.GetInstance().ResolveAll<IMessageTranslator>()));
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                        s.WhenShutdown(tc => tc.Shutdown());
                        s.WhenContinued(tc => tc.Continue());
                        s.WhenPaused(tc => tc.Pause());
                    });
                x.RunAsLocalSystem();

#if DEBUG
                x.SetServiceName("Sciendo Synch Receiver (Debug)");
                x.SetDisplayName("Sciendo Synch Receiver (Debug)");
                x.SetDescription("Receives messages from  a queue and persists files to folder. (Debug)");
#else
                x.SetServiceName("Sciendo Synch Receiver");
                x.SetDisplayName("Sciendo Synch Receiver");
                x.SetDescription("Receives messages from  a queue and persists files to folder.");
#endif
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

