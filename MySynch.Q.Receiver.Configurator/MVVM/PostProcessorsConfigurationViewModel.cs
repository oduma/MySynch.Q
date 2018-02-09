using System.Collections.ObjectModel;
using System.Windows.Input;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class PostProcessorsConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string PostProcessorsViewTitle => $"Postprocessors for receiver - {ReceiverIdentifier}";
        public string ReceiverIdentifier { get; set; }

        private ObservableCollection<PostProcessorConfigurationViewModel> _postProcessors;

        public ObservableCollection<PostProcessorConfigurationViewModel> PostProcessors
        {
            get { return _postProcessors; }
            set
            {
                if (_postProcessors != value)
                {
                    
                    _postProcessors = value;
                    TrackAllChildren(_postProcessors);
                    RaisePropertyChanged(() => PostProcessors);
                }
            }
        }

        public PostProcessorConfigurationViewModel SelectedPostProcessor { get; set; }

        public ICommand AddNewPostProcessor { get; private set; }

        public ICommand RemovePostProcessor { get; private set; }

        public PostProcessorsConfigurationViewModel()
        {
            AddNewPostProcessor = new RelayCommand(AddPostProcessor);
            RemovePostProcessor = new RelayCommand(Remove);
        }

        private void Remove()
        {
            PostProcessors.Remove(SelectedPostProcessor);
            RaisePropertyChanged(() => PostProcessors);
            RaiseChangeEvent();

        }

        private void AddPostProcessor()
        {
            var newPostProcessor = new PostProcessorConfigurationViewModel();
            TrackAllChildren(new[] { newPostProcessor });
            if (PostProcessors == null)
                PostProcessors = new ObservableCollection<PostProcessorConfigurationViewModel>();
            PostProcessors.Add(newPostProcessor);
            RaisePropertyChanged(() => PostProcessors);
        }

        public void InitiateView()
        {
            
        }
    }
}