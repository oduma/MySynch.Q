﻿using System.ComponentModel;
using System.ServiceProcess;
namespace MySynch.Q.Sender
{

    partial class SenderInstaller
    {

        private ServiceProcessInstaller process;
        private ServiceInstaller service;


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
#if (DEBUG)
            service.ServiceName = "MySynch.Q.Sender.Debug";
#endif
#if (!DEBUG)
            service.ServiceName = "MySynch.Q.Sender";
#endif
            Installers.Add(process);
            Installers.Add(service);


        }

        #endregion
    }
}