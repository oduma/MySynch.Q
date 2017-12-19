using System;
using System.Windows.Input;
using MySynch.Q.Common.Contracts;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class SenderConfigurationViewModel:ViewModelBase
    {
        private QueuesConfiguratorViewModel _queuesViewModel;
        private FiltersConfiguratorViewModel _filtersViewModel;
        public RootFolderViewModel LocalRootFolderViewModel { get; set; }

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
            FiltersViewModel.SenderIdentifier = this.LocalRootFolderViewModel.LocalRootFolder;
            var filtersView = new FiltersView(FiltersViewModel);
            var dialogResult = filtersView.ShowDialog();
            NoOfFilters = FiltersViewModel.Filters.Count;
            RaisePropertyChanged(() => FiltersLauncherTitle);
        }

        private void ShowQueues()
        {
            QueuesViewModel.SenderIdentifier = this.LocalRootFolderViewModel.LocalRootFolder;
            var queuesView=new QueuesView(QueuesViewModel);
            var dialogResult = queuesView.ShowDialog();
            NoOfQueues = QueuesViewModel.Queues.Count;
            RaisePropertyChanged(()=>QueuesLauncherTitle);
        }

        public ICommand ViewQueues { get; private set; }

        public ICommand ViewFilters { get; private set; }

        public QueuesConfiguratorViewModel QueuesViewModel
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

        public FiltersConfiguratorViewModel FiltersViewModel
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