using System;
using System.Collections.ObjectModel;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class QueuesConfigurationViewModel:ViewModelWithTrackChangesBase
    {

        public string QueuesViewTitle => $"Queues for sender - {SenderIdentifier}";

        private ObservableCollection<QueueConfigurationViewModel> _queues;

        public ObservableCollection<QueueConfigurationViewModel> Queues
        {
            get { return _queues; }
            set
            {
                if (_queues != value)
                {
                    _queues = value;
                    TrackAllChildren(_queues);
                    RaisePropertyChanged(() => Queues);
                }
            }
        }

        public void InitiateView()
        {
        }

        public string SenderIdentifier { get; set; }
    }
}