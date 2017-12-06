﻿using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    internal class QueuesConfiguratorViewModel:ViewModelBase
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