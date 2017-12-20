using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Receiver.Configurator.Configuration;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class ReceiversConfiguratorViewModel : ViewModelBase
    {
        private ConfigurationProvider _configurationProvider;
        private ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> _receiversConfigurationToViewModelProvider;
        private ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> _translatorsConfigurationToViewModelProvider;

        public ReceiversConfiguratorViewModel(ConfigurationProvider configurationProvider, 
            ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> receiversConfigurationToViewModelProvider, 
            ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> translatorsConfigurationToViewModelProvider)
        {
            this._configurationProvider = configurationProvider;
            this._receiversConfigurationToViewModelProvider = receiversConfigurationToViewModelProvider;
            this._translatorsConfigurationToViewModelProvider = translatorsConfigurationToViewModelProvider;
        }

        public ICommand Save { get; private set; }

        private ObservableCollection<ReceiverConfigurationViewModel> _receivers;

        public ObservableCollection<ReceiverConfigurationViewModel> Receivers
        {
            get { return _receivers; }
            set
            {
                if (_receivers != value)
                {
                    _receivers = value;
                    RaisePropertyChanged(() => Receivers);
                }
            }
        }


        private ObservableCollection<TranslatorConfigurationViewModel> _translators;

        public ObservableCollection<TranslatorConfigurationViewModel> Translators
        {
            get { return _translators; }
            set
            {
                if (_translators != value)
                {
                    _translators = value;
                    RaisePropertyChanged(() => Translators);
                }
            }
        }
        public void InitiateView()
        {
            Receivers = _receiversConfigurationToViewModelProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetReceiverConfigurationDescription.SectionName));
            Translators = _translatorsConfigurationToViewModelProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetTranslatorConfigurationDescription.SectionName));
            Save = new RelayCommand(SaveConfig);
        }

        private void SaveConfig()
        {
            throw new NotImplementedException();
        }
    }
}
