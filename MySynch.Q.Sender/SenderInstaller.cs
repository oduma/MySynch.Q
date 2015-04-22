using System.ComponentModel;
using System.ServiceProcess;

namespace MySynch.Q.Sender
{
    [RunInstaller(true)]
    public partial class SenderInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;


        public SenderInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();

#if DEBUG
            service.ServiceName = "Sciendo Synch Sender (Debug)";
            service.Description = "Sends messages when files change (Debug)";
#else
            service.ServiceName = "Sciendo Synch Sender";
            service.Description = "Sends messages when files located at change";
#endif
            Installers.Add(process);
            Installers.Add(service);



            InitializeComponent();
        }
    }
}
