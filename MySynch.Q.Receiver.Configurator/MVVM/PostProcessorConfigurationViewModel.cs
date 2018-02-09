using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class PostProcessorConfigurationViewModel:ViewModelWithTrackChangesBase
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}