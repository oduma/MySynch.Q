using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Receiver.Configurator.Configuration;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class ReceiversConfigurationViewModel : ViewModelBase
    {
        private readonly ConfigurationProvider _configurationProvider;
        private readonly ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> _receiversConfigurationToViewModelProvider;
        private readonly ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> _translatorsConfigurationToViewModelProvider;

        public ReceiversConfigurationViewModel(ConfigurationProvider configurationProvider, 
            ConfigurationToViewModelProvider<ReceiverConfigurationViewModel> receiversConfigurationToViewModelProvider, 
            ConfigurationToViewModelProvider<TranslatorConfigurationViewModel> translatorsConfigurationToViewModelProvider)
        {
            _configurationProvider = configurationProvider;
            _receiversConfigurationToViewModelProvider = receiversConfigurationToViewModelProvider;
            _translatorsConfigurationToViewModelProvider = translatorsConfigurationToViewModelProvider;
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
            if (_receiversConfigurationToViewModelProvider.SetViewModelsCollection(Receivers, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetReceiverConfigurationDescription.SectionName))
                && _translatorsConfigurationToViewModelProvider.SetViewModelsCollection(Translators, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetTranslatorConfigurationDescription.SectionName)))
            {
                //mark as saved
                return;
            }
            //mark as unsaved
            return;

        }
    }
}
