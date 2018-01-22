using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
                    if(_queues!=null)
                        TrackAllChildren(_queues);
                    RaisePropertyChanged(() => Queues);
                }
            }
        }

        public ICommand AddNewQueue { get; private set; }

        public QueuesConfigurationViewModel()
        {
            AddNewQueue=new RelayCommand(AddQueue);
        }

        private void AddQueue()
        {
            var newQueue= new QueueConfigurationViewModel();
            TrackAllChildren(new [] {newQueue});
            if(Queues==null)
                Queues= new ObservableCollection<QueueConfigurationViewModel>();
            Queues.Add(newQueue);
            RaisePropertyChanged(()=>Queues);
        }

        public void InitiateView()
        {
        }

        public string SenderIdentifier { get; set; }
    }
}