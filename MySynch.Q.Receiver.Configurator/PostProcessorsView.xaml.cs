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
using System.Windows.Shapes;
using MySynch.Q.Receiver.Configurator.MVVM;

namespace MySynch.Q.Receiver.Configurator
{
    /// <summary>
    /// Interaction logic for PostProcessorsView.xaml
    /// </summary>
    public partial class PostProcessorsView : Window
    {
        internal PostProcessorsView(PostProcessorsConfigurationViewModel postProcessorsConfiguratorViewModel)
        {
            InitializeComponent();
            postProcessorsConfiguratorViewModel.InitiateView();
            this.DataContext = postProcessorsConfiguratorViewModel;
        }
    }
}
