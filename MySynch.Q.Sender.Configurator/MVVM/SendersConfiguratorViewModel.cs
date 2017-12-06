using System.Collections.ObjectModel;
using MySynch.Q.Common.Contracts;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    internal class SendersConfiguratorViewModel:ViewModelBase
    {
        public SendersConfiguratorViewModel()
        {
            AllAvailableBodyTypes = new[] {BodyType.None, BodyType.Binary, BodyType.Text,};
        }

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
            Senders = new ObservableCollection<SenderConfigurationViewModel>();
            Senders.Add(new SenderConfigurationViewModel
            {
                LocalRootFolderViewModel = new RootFolderViewModel {LocalRootFolder = "cccccccc"},
                MessageBodyType = BodyType.Binary,
                MinMemory = 2000,
                QueuesViewModel =
                    new QueuesConfiguratorViewModel
                    {
                        Queues =
                            new ObservableCollection<QueueConfigurationViewModel>
                            {
                                new QueueConfigurationViewModel {Name = "queue1"}
                            }
                    },
                FiltersViewModel =
                    new FiltersConfiguratorViewModel
                    {
                        Filters =
                            new ObservableCollection<FilterConfigurationViewModel>
                            {
                                new FilterConfigurationViewModel {Key = "abc", Value = ".abc"}
                            }
                    }
            });
        }
    }
}