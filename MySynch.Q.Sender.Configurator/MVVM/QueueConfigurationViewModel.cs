using System;
using System.Windows.Input;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class QueueConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string Name { get; set; }

        public string QueueName { get; set; }

        public string Host { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

    }
}