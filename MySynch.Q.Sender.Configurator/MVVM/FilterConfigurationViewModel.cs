using System.Windows.Input;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class FilterConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string Key { get; set; }

        public string Value { get; set; }

    }
}