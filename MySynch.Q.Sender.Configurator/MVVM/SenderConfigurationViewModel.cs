using System.Windows.Input;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Controls.MVVM;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class SenderConfigurationViewModel:ViewModelBase
    {
        private QueuesConfigurationViewModel _queuesViewModel;
        private FiltersConfigurationViewModel _filtersViewModel;
        public FolderPickerViewModel LocalRootFolderViewModel { get; set; }

        public BodyType MessageBodyType { get; set; }

        public int MinMemory { get; set; }

        public string QueuesLauncherTitle => $"Defined ({NoOfQueues})";

        public string FiltersLauncherTitle => $"Defined ({NoOfFilters})";

        public int NoOfQueues { get; set; }

        public int NoOfFilters { get; set; }

        public SenderConfigurationViewModel()
        {
            ViewQueues=new RelayCommand(ShowQueues);
            ViewFilters= new RelayCommand(ShowFilters);
        }

        private void ShowFilters()
        {
            FiltersViewModel.SenderIdentifier = LocalRootFolderViewModel.Folder;
            var filtersView = new FiltersView(FiltersViewModel);
            filtersView.ShowDialog();
            NoOfFilters = FiltersViewModel.Filters.Count;
            RaisePropertyChanged(() => FiltersLauncherTitle);
        }

        private void ShowQueues()
        {
            QueuesViewModel.SenderIdentifier = LocalRootFolderViewModel.Folder;
            var queuesView=new QueuesView(QueuesViewModel);
            queuesView.ShowDialog();
            NoOfQueues = QueuesViewModel.Queues.Count;
            RaisePropertyChanged(()=>QueuesLauncherTitle);
        }

        public ICommand ViewQueues { get; private set; }

        public ICommand ViewFilters { get; private set; }

        public QueuesConfigurationViewModel QueuesViewModel
        {
            get { return _queuesViewModel; }
            set
            {
                if (value != _queuesViewModel)
                {
                    _queuesViewModel = value;
                    if(_queuesViewModel.Queues!=null)
                        NoOfQueues = _queuesViewModel.Queues.Count;
                    RaisePropertyChanged(()=>NoOfQueues);
                    RaisePropertyChanged(()=>QueuesLauncherTitle);
                }
            }
        }

        public FiltersConfigurationViewModel FiltersViewModel
        {
            get { return _filtersViewModel; }
            set
            {
                if (value != _filtersViewModel)
                {
                    _filtersViewModel = value;
                    if(_filtersViewModel.Filters!=null)
                        NoOfFilters = _filtersViewModel.Filters.Count;
                    RaisePropertyChanged(() => NoOfFilters);
                    RaisePropertyChanged(() => FiltersLauncherTitle);
                }
            }
        }

    }
}