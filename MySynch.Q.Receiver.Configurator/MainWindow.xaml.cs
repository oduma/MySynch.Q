using System.Windows;
using MySynch.Q.Common.Configurators;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Receiver.Configurator.Configuration;
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
            var receiversConfiguratorViewModel = new ReceiversConfigurationViewModel(new ConfigurationProvider(),
                new ConfigurationToViewModelProvider<ReceiverConfigurationViewModel>(new MapCollectionNodeNoAttributes<ReceiverConfigurationViewModel>(new MapReceiver(),TargetReceiverConfigurationDescription.ReceiversCollectionElementName)),
                new ConfigurationToViewModelProvider<TranslatorConfigurationViewModel>(
                    new MapCollectionNodeNoAttributes<TranslatorConfigurationViewModel>(new MapTranslator(),TargetTranslatorConfigurationDescription.TranslatorsCollectionElementName)));
            receiversConfiguratorViewModel.InitiateView();
            DataContext = receiversConfiguratorViewModel;
        }
    }
}
