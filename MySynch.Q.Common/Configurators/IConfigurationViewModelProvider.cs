using System.Collections.ObjectModel;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Common.Configurators
{
    public interface IConfigurationViewModelProvider<T> where T:ViewModelBase
    {
        ObservableCollection<T> GetViewModelsCollection(ConfigurationSectionLocator configurationSectionLocator);

        bool SetViewModelsCollection(ObservableCollection<T> input, ConfigurationSectionLocator configurationSectionLocator);
    }
}
