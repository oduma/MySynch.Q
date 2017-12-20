using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Receiver.Configurator.MVVM
{
    public class TranslatorConfigurationViewModel:ViewModelBase
    {
        public int Priority { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}