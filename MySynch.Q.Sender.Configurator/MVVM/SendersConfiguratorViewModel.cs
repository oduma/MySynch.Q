using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.Models;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    internal class SendersConfiguratorViewModel:ViewModelBase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISendersProvider _sendersProvider;

        public SendersConfiguratorViewModel(IConfigurationProvider configurationProvider,
            ISendersProvider sendersProvider)
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
            Senders =_sendersProvider.GetSenders(_configurationProvider.GetConfigInfo());
            Save= new RelayCommand(SaveConfig);
        }

        private void SaveConfig()
        {
            if (_sendersProvider.SetSenders(Senders, _configurationProvider.GetConfigInfo()))
            {
                //mark as saved
                return;
            }
            //mark as unsaved
            return;
        }
    }
}