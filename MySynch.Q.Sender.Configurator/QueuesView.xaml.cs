﻿using System;
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
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator
{
    /// <summary>
    /// Interaction logic for QueuesView.xaml
    /// </summary>
    public partial class QueuesView : Window
    {
        internal QueuesView(QueuesConfiguratorViewModel queuesConfiguratorViewModel)
        {
            InitializeComponent();
            queuesConfiguratorViewModel.InitiateView();
            this.DataContext = queuesConfiguratorViewModel;
        }
    }
}
