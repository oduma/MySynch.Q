using System.Windows;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator
{
    /// <summary>
    /// Interaction logic for QueuesView.xaml
    /// </summary>
    public partial class QueuesView : Window
    {
        internal QueuesView(QueuesConfigurationViewModel queuesConfiguratorViewModel)
        {
            InitializeComponent();
            queuesConfiguratorViewModel.InitiateView();
            DataContext = queuesConfiguratorViewModel;
        }
    }
}
