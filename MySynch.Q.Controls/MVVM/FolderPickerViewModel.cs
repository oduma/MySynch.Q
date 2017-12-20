using System.IO;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Controls.MVVM
{
    public class FolderPickerViewModel:ViewModelBase
    {
        public ICommand SelectFolder { get; private set; }
        public string Folder { get; set; }

        public FolderPickerViewModel()
        {
            SelectFolder=new RelayCommand(ShowSelectFolder);
        }

        private void ShowSelectFolder()
        {
            var dialog = new CommonOpenFileDialog();
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