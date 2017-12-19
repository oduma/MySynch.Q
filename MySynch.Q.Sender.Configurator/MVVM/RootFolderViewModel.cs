using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Sender.Configurator.MVVM
{
    public class RootFolderViewModel:ViewModelBase
    {
        public ICommand SelectFolder { get; private set; }
        public string LocalRootFolder { get; set; }

        public RootFolderViewModel()
        {
            SelectFolder=new RelayCommand(ShowSelectFolder);
        }

        private void ShowSelectFolder()
        {
            var dialog = new CommonOpenFileDialog();
            if (Directory.Exists(LocalRootFolder))
                dialog.InitialDirectory = LocalRootFolder;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LocalRootFolder = dialog.FileName;
                RaisePropertyChanged(() => LocalRootFolder);
            }
        }
    }
}