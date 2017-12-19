using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class QueueConfigurationViewModel:ViewModelBase
    {
        public string Name { get; set; }

        public string QueueName { get; set; }

        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }
}