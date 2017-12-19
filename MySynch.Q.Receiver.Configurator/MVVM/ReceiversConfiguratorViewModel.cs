using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Q.Common.Configurators;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class ReceiversConfiguratorViewModel : ViewModelBase
    {
        private ConfigurationProvider configurationProvider;
        private ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> configurationToViewModelProvider1;
        private ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> configurationToViewModelProvider2;

        public ReceiversConfiguratorViewModel(ConfigurationProvider configurationProvider, ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> configurationToViewModelProvider1, ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> configurationToViewModelProvider2)
        {
            this.configurationProvider = configurationProvider;
            this.configurationToViewModelProvider1 = configurationToViewModelProvider1;
            this.configurationToViewModelProvider2 = configurationToViewModelProvider2;
        }

        public void InitiateView()
        {
            throw new NotImplementedException();
        }
    }
}
