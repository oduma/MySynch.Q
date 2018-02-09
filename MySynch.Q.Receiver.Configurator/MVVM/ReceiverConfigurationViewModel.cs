using System.Windows.Input;
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

        public string PostProcessorsLauncherTitle => $"Defined ({NoOfPostProcessors})";

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

        public ICommand ViewPostProcessors { get; private set; }

        private int _noOfPostProcessors;

        public int NoOfPostProcessors
        {
            get
            {
                return _noOfPostProcessors;
            }
            set
            {
                if (_noOfPostProcessors != value)
                {
                    _noOfPostProcessors = value;
                    RaiseChangeEvent();
                }
            }
        }

        public ReceiverConfigurationViewModel()
        {
            ViewPostProcessors = new RelayCommand(ShowPostProcessors);

        }

        private PostProcessorsConfigurationViewModel _postProcessorsViewModel;

        public PostProcessorsConfigurationViewModel PostProcessorsViewModel
        {
            get { return _postProcessorsViewModel; }
            set
            {
                if (value != _postProcessorsViewModel)
                {
                    _postProcessorsViewModel = value;
                    if (_postProcessorsViewModel.PostProcessors != null)
                        NoOfPostProcessors = _postProcessorsViewModel.PostProcessors.Count;
                    TrackAllChildren(new[] { _postProcessorsViewModel });
                    RaisePropertyChanged(() => NoOfPostProcessors);
                    RaisePropertyChanged(() => PostProcessorsLauncherTitle);
                }
            }
        }

        private void ShowPostProcessors()
        {
            if (PostProcessorsViewModel == null)
                PostProcessorsViewModel = new PostProcessorsConfigurationViewModel();
            PostProcessorsViewModel.ReceiverIdentifier = (LocalRootFolderViewModel.Folder) ?? string.Empty;
            var postProcesorsView = new PostProcessorsView(PostProcessorsViewModel);
            postProcesorsView.ShowDialog();
            NoOfPostProcessors = PostProcessorsViewModel.PostProcessors.Count;
            RaisePropertyChanged(() => PostProcessorsLauncherTitle);
        }
    }
}
