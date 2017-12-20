using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    internal class SendersConfiguratorViewModel:ViewModelBase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IConfigurationViewModelProvider<SenderConfigurationViewModel> _sendersProvider;

        public SendersConfiguratorViewModel(IConfigurationProvider configurationProvider,
            IConfigurationViewModelProvider<SenderConfigurationViewModel> sendersProvider)
        {
            _configurationProvider = configurationProvider;
            _sendersProvider = sendersProvider;
            AllAvailableBodyTypes = new[] {BodyType.None, BodyType.Binary, BodyType.Text,};
        }

        public ICommand Save { get; private set; }

        private ObservableCollection<SenderConfigurationViewModel> _senders;

        public ObservableCollection<SenderConfigurationViewModel> Senders
        {
            get { return _senders; }
            set
            {
                if (_senders != value)
                {
                    _senders = value;
                    RaisePropertyChanged(() => Senders);
                }
            }
        }

        public BodyType[] AllAvailableBodyTypes { get; private set; }
        public void InitiateView()
        {
            Senders =_sendersProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetSenderConfigurationDescription.SectionElementName));
            Save= new RelayCommand(SaveConfig);
        }

        private void SaveConfig()
        {
            if (_sendersProvider.SetViewModelsCollection(Senders, _configurationProvider.GetConfigInfo()?.FirstOrDefault()))
            {
                //mark as saved
                return;
            }
            //mark as unsaved
            return;
        }
    }
}