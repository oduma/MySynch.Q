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
        private readonly ISvcController _receiverServiceController;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IConfigurationViewModelProvider<ReceiverConfigurationViewModel> _receiversConfigurationToViewModelProvider;
        private readonly IConfigurationViewModelProvider<TranslatorConfigurationViewModel> _translatorsConfigurationToViewModelProvider;

        public ReceiversConfigurationViewModel(ISvcController receiverServiceController, IConfigurationProvider configurationProvider,
            IConfigurationViewModelProvider<ReceiverConfigurationViewModel> receiversConfigurationToViewModelProvider,
            IConfigurationViewModelProvider<TranslatorConfigurationViewModel> translatorsConfigurationToViewModelProvider)
        {
            _receiverServiceController = receiverServiceController;
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
            var serviceNames = _configurationProvider.GetConfigInfo().Select(l => l.ServiceName).Distinct();
            _receiverServiceController.Stop(serviceNames);
            if (_receiversConfigurationToViewModelProvider.SetViewModelsCollection(Receivers, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetReceiverConfigurationDescription.SectionName))
                && _translatorsConfigurationToViewModelProvider.SetViewModelsCollection(Translators, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetTranslatorConfigurationDescription.SectionName)))
            {
                //mark as saved
                _receiverServiceController.Start(serviceNames);
                return;
            }
            //mark as unsaved
            _receiverServiceController.Start(serviceNames);
            return;

        }
    }
}
