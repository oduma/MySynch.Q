using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Controls.MVVM;
using MySynch.Q.Sender.Configurator.Configuration;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    internal class SendersConfigurationViewModel:ViewModelBase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IConfigurationViewModelProvider<SenderConfigurationViewModel> _sendersProvider;
        private readonly ISvcController _svcController;

        public SendersConfigurationViewModel(IConfigurationProvider configurationProvider,
            IConfigurationViewModelProvider<SenderConfigurationViewModel> sendersProvider, ISvcController svcController)
        {
            _configurationProvider = configurationProvider;
            _sendersProvider = sendersProvider;
            _svcController = svcController;
            AllAvailableBodyTypes = new[] {BodyType.None, BodyType.Binary, BodyType.Text,};
            WindowTitle = DefaultWindowTitle;
            SaveEnabled = false;
        }

        public ICommand Save { get; private set; }

        public ICommand AddNewSender { get; private set; }

        private ObservableCollection<SenderConfigurationViewModel> _senders;

        public ObservableCollection<SenderConfigurationViewModel> Senders
        {
            get { return _senders; }
            set
            {
                if (_senders != value)
                {
                    _senders = value;
                    foreach(var sender in _senders)
                        sender.ViewModelChanged += Sender_ViewModelChanged;
                    RaisePropertyChanged(() => Senders);
                }
            }
        }

        private void Sender_ViewModelChanged(object sender, System.EventArgs e)
        {
            WindowTitle = DefaultWindowTitle + " * ";
            SaveEnabled = true;
        }

        private bool _saveEnabled;
        public bool SaveEnabled { get {return _saveEnabled;} set
        {
            if (_saveEnabled != value)
            {
                _saveEnabled = value; 
                RaisePropertyChanged(()=>SaveEnabled);
            }
        } }

        public const string DefaultWindowTitle = "Senders Configurator";

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    RaisePropertyChanged(()=>WindowTitle);
                }
            }
        }
        public BodyType[] AllAvailableBodyTypes { get; private set; }
        public void InitiateView()
        {
            Senders =_sendersProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetSenderConfigurationDescription.SectionElementName));
            Save= new RelayCommand(SaveConfig);
            AddNewSender= new RelayCommand(AddSender);
        }

        private void AddSender()
        {
            var newSenderConfig = new SenderConfigurationViewModel
            {
                LocalRootFolderViewModel = new FolderPickerViewModel(),
                QueuesViewModel = new QueuesConfigurationViewModel()
            };
            newSenderConfig.ViewModelChanged += Sender_ViewModelChanged;
            Senders.Add(newSenderConfig);
            RaisePropertyChanged(()=>Senders);
        }

        private void SaveConfig()
        {
            var serviceNames = _configurationProvider.GetConfigInfo().Select(l => l.ServiceName).Distinct();
            _svcController.Stop(serviceNames);
            if (_sendersProvider.SetViewModelsCollection(Senders, _configurationProvider.GetConfigInfo()?.FirstOrDefault()))
            {
                _svcController.Start(serviceNames);
                SaveEnabled = false;
                WindowTitle = DefaultWindowTitle;
                return;
            }
            _svcController.Start(serviceNames);
        }
    }
}