using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Receiver.Configurator.Mappers;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var receiversConfiguratorViewModel = new ReceiversConfiguratorViewModel(new ConfigurationProvider(),
                new ConfigurationToViewModelProvider<ReceiverConfigurationViewModel>(new MapReceivers(new MapReceiver())),
                new ConfigurationToViewModelProvider<TranslatorConfigurationViewModel>(
                    new MapTextTranslators(new MapTrextTranslator())));
            receiversConfiguratorViewModel.InitiateView();
            this.DataContext = receiversConfiguratorViewModel;
        }
    }
}
