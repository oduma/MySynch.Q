using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Controls.MVVM
{
    public class FolderPickerViewModel:ViewModelWithTrackChangesBase
    {
        public ICommand SelectFolder { get; private set; }

        private string _folder;
        private bool _firstLoad;

        public string Folder { get { return _folder; }
            set
            {
                if (_folder != value)
                {
                    _folder = value;
                    RaisePropertyChanged(()=>Folder);
                    if (!_firstLoad)
                    {
                        RaiseChangeEvent();
                    }
                    _firstLoad = false;
                }    
            }
        }

        public FolderPickerViewModel()
        {
            SelectFolder=new RelayCommand(ShowSelectFolder);
            _firstLoad = true;
        }

        private void ShowSelectFolder()
        {
            var dialog = new CommonOpenFileDialog();
            if (string.IsNullOrEmpty(Folder))
            {
                Folder = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (Directory.Exists(Folder))
                dialog.InitialDirectory = Folder;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Folder = dialog.FileName;
                RaisePropertyChanged(() => Folder);
            }
        }
    }
}