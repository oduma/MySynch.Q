using System;
using System.Windows.Input;
using MySynch.Q.Common.Contracts;
using MySynch.Q.Controls.MVVM;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class SenderConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        private QueuesConfigurationViewModel _queuesViewModel;
        private FiltersConfigurationViewModel _filtersViewModel;

        private FolderPickerViewModel _localRootFolderViewModel;

        public FolderPickerViewModel LocalRootFolderViewModel { get {return _localRootFolderViewModel;}
            set
            {
                if (_localRootFolderViewModel != value)
                {
                    _localRootFolderViewModel = value;
                    TrackAllChildren(new [] {_localRootFolderViewModel});
                }
            } }


        public BodyType MessageBodyType { get; set; }


        public int MinMemory { get; set; }

        public string QueuesLauncherTitle => $"Defined ({NoOfQueues})";

        public string FiltersLauncherTitle => $"Defined ({NoOfFilters})";

        private int _noOfQueues;

        public int NoOfQueues
        {
            get
            {
                return _noOfQueues;
            }
            set
            {
                if (_noOfQueues != value)
                {
                    _noOfQueues = value;
                    RaiseChangeEvent();
                }
            }
        }

        private int _noOfFilters;

        public int NoOfFilters
        {
            get
            {
                return _noOfFilters;
            }
            set
            {
                if (_noOfFilters != value)
                {
                    _noOfFilters = value;
                    RaiseChangeEvent();
                }
            }
        }

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
            QueuesViewModel.SenderIdentifier = (LocalRootFolderViewModel.Folder)??string.Empty;
            var queuesView=new QueuesView(QueuesViewModel);
            queuesView.ShowDialog();
            NoOfQueues = (QueuesViewModel.Queues!=null)?QueuesViewModel.Queues.Count:0;
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
                    TrackAllChildren(new []{_queuesViewModel});
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
                    TrackAllChildren(new [] {_filtersViewModel});
                    RaisePropertyChanged(() => NoOfFilters);
                    RaisePropertyChanged(() => FiltersLauncherTitle);
                }
            }
        }
    }
}