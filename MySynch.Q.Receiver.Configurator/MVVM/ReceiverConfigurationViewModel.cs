using MySynch.Q.Controls.MVVM;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class ReceiverConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string User { get; set; }
        private FolderPickerViewModel _localRootFolderViewModel;

        public FolderPickerViewModel LocalRootFolderViewModel
        {
            get { return _localRootFolderViewModel; }
            set
            {
                if (_localRootFolderViewModel != value)
                {
                    _localRootFolderViewModel = value;
                    TrackAllChildren(new[] { _localRootFolderViewModel });
                }
            }
        }

    }
}
