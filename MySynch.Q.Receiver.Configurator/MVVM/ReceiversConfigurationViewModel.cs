using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Controls.MVVM;
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
            WindowTitle = DefaultWindowTitle;
            SaveEnabled = false;
        }

        private bool _saveEnabled;
        public bool SaveEnabled
        {
            get { return _saveEnabled; }
            set
            {
                if (_saveEnabled != value)
                {
                    _saveEnabled = value;
                    RaisePropertyChanged(() => SaveEnabled);
                }
            }
        }


        public const string DefaultWindowTitle = "Receivers Configurator";

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    RaisePropertyChanged(() => WindowTitle);
                }
            }
        }

        public ICommand Save { get; private set; }

        public ICommand AddNewReceiver { get; private set; }

        public ICommand AddNewTranslator { get; private set; }

        public ICommand RemoveReceiver { get; private set; }

        public ICommand RemoveTranslator { get; private set; }

        private ObservableCollection<ReceiverConfigurationViewModel> _receivers;

        public ObservableCollection<ReceiverConfigurationViewModel> Receivers
        {
            get { return _receivers; }
            set
            {
                if (_receivers != value)
                {
                    _receivers = value;
                    foreach (var receiver in _receivers)
                        receiver.ViewModelChanged += Child_ViewModelChanged;

                    RaisePropertyChanged(() => Receivers);
                }
            }
        }

        private void Child_ViewModelChanged(object sender, System.EventArgs e)
        {
            MarkForSave();
        }

        private void MarkForSave()
        {
            WindowTitle = DefaultWindowTitle + " * ";
            SaveEnabled = true;
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
                    foreach (var translator in _translators)
                        translator.ViewModelChanged += Child_ViewModelChanged;
                    RaisePropertyChanged(() => Translators);
                }
            }
        }
        public void InitiateView()
        {
            Receivers = _receiversConfigurationToViewModelProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetReceiverConfigurationDescription.SectionName));
            Translators = _translatorsConfigurationToViewModelProvider.GetViewModelsCollection(_configurationProvider.GetConfigInfo()?.FirstOrDefault(c=>c.SectionIdentifier==TargetTranslatorConfigurationDescription.SectionName));
            Save = new RelayCommand(SaveConfig);
            AddNewReceiver = new RelayCommand(AddReceiver);
            AddNewTranslator = new RelayCommand(AddTranslator);
            RemoveReceiver= new RelayCommand(DeleteReceiver);
            RemoveTranslator= new RelayCommand(DeleteTranslator);
        }

        private void DeleteTranslator()
        {
            Translators.Remove(SelectedTranslator);
            RaisePropertyChanged(()=>Translators);
            MarkForSave();
        }

        public TranslatorConfigurationViewModel SelectedTranslator { get; set; }

        private void DeleteReceiver()
        {
            Receivers.Remove(SelectedReceiver);
            RaisePropertyChanged(()=>Receivers);
            MarkForSave();
        }

        public ReceiverConfigurationViewModel SelectedReceiver { get; set; }

        private void AddTranslator()
        {
            var newTranslatorConfig = new TranslatorConfigurationViewModel();
            newTranslatorConfig.ViewModelChanged += Child_ViewModelChanged;
            Translators.Add(newTranslatorConfig);
            RaisePropertyChanged(() => Translators);
        }

        private void AddReceiver()
        {
            var newReceiverConfig = new ReceiverConfigurationViewModel
            {
                LocalRootFolderViewModel = new FolderPickerViewModel()
            };
            newReceiverConfig.ViewModelChanged += Child_ViewModelChanged;
            Receivers.Add(newReceiverConfig);
            RaisePropertyChanged(() => Receivers);
        }

        private void SaveConfig()
        {
            var serviceNames = _configurationProvider.GetConfigInfo().Select(l => l.ServiceName).Distinct();
            _receiverServiceController.Stop(serviceNames);
            if (_receiversConfigurationToViewModelProvider.SetViewModelsCollection(Receivers, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetReceiverConfigurationDescription.SectionName))
                && _translatorsConfigurationToViewModelProvider.SetViewModelsCollection(Translators, _configurationProvider.GetConfigInfo()?.FirstOrDefault(c => c.SectionIdentifier == TargetTranslatorConfigurationDescription.SectionName)))
            {
                SaveEnabled = false;
                WindowTitle = DefaultWindowTitle;
                _receiverServiceController.Start(serviceNames);
                return;
            }
            _receiverServiceController.Start(serviceNames);
        }
    }
}
