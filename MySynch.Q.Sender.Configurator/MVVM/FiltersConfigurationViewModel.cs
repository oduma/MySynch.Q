using System.Collections.ObjectModel;
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

        public string SenderIdentifier { get; set; }

        public void InitiateView()
        {
            
        }
    }
}