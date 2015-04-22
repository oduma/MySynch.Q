using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MySynch.Q.Receiver
{
    [RunInstaller(true)]
    public partial class ReceiverInstaller : Installer
    {

        public ReceiverInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
#if DEBUG
                service.ServiceName = "Sciendo Synch Receiver (Debug)";
                service.Description = "Receives messages from  a queue and persists files to folder. (Debug)";
#else
            service.ServiceName = "Sciendo Synch Receiver";
                service.Description = "Receives messages from  a queue and persists files to folder.";
#endif
                Installers.Add(process);
            Installers.Add(service);
            InitializeComponent();
        }
    }
}
