using System.Windows;
using MySynch.Q.Common.Configurators;
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
            var senderConfiguratorViewModel = new SendersConfiguratorViewModel(new ConfigurationProvider(),
                new ConfigurationToViewModelProvider<SenderConfigurationViewModel>(new MapSenders((new MapSender(new MapFilters(new MapFilter()),new MapQueues(new MapQueue()))))));
            senderConfiguratorViewModel.InitiateView();
            this.DataContext = senderConfiguratorViewModel;
        }
    }
}
