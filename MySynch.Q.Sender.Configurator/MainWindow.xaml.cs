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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.Models;
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
                new SendersProvider(new MapSenders((new MapSender(new MapFilters(new MapFilter()),new MapQueues(new MapQueue()))))));
            senderConfiguratorViewModel.InitiateView();
            this.DataContext = senderConfiguratorViewModel;
        }
    }
}
