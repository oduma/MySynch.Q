using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Controls.MVVM;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class ReceiverConfigurationViewModel:ViewModelBase
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string User { get; set; }
        public FolderPickerViewModel LocalRootFolderViewModel { get; set; }
    }
}
