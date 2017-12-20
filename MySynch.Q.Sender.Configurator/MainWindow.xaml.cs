using System.Windows;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var senderConfiguratorViewModel = new SendersConfigurationViewModel(new ConfigurationProvider(),
                new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(
                    new MapCollectionNodeNoAttributes<SenderConfigurationViewModel>(
                        new MapSender(
                            new MapCollectionNodeNoAttributes<FilterConfigurationViewModel>(new MapFilter(),
                                TargetFilterConfigurationDescription.FiltersCollectionElementName),
                            new MapCollectionNodeNoAttributes<QueueConfigurationViewModel>(new MapQueue(),
                                TargetQueueConfigurationDescription.QueuesCollectionElementName)),
                        TargetSenderConfigurationDescription.SendersCollectionElementName)));
            senderConfiguratorViewModel.InitiateView();
            DataContext = senderConfiguratorViewModel;
        }
    }
}
