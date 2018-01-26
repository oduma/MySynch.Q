using System.Collections.ObjectModel;
using System.Windows.Input;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class FiltersConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string QueuesViewTitle => $"Filters for sender - {SenderIdentifier}";

        private ObservableCollection<FilterConfigurationViewModel> _filters;

        public ObservableCollection<FilterConfigurationViewModel> Filters
        {
            get { return _filters; }
            set
            {
                if (_filters != value)
                {
                    _filters = value;
                    TrackAllChildren(_filters);
                    RaisePropertyChanged(() => Filters);
                }
            }
        }

        public FilterConfigurationViewModel SelectedFilter { get; set; }
        public ICommand AddNewFilter { get; private set; }

        public ICommand RemoveFilter { get; private set; }
        public string SenderIdentifier { get; set; }

        public void InitiateView()
        {
            
        }

        public FiltersConfigurationViewModel()
        {
            AddNewFilter= new RelayCommand(AddFilter);
            RemoveFilter = new RelayCommand(Remove);
        }

        private void Remove()
        {
            Filters.Remove(SelectedFilter);
            RaisePropertyChanged(()=>Filters);
            RaiseChangeEvent();
        }

        private void AddFilter()
        {
            var newFilter = new FilterConfigurationViewModel();
            TrackAllChildren(new[] { newFilter });
            if (Filters == null)
                Filters = new ObservableCollection<FilterConfigurationViewModel>();
            Filters.Add(newFilter);
            RaisePropertyChanged(() => Filters);

        }
    }
}