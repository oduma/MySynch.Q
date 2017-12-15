using System.Collections.Generic;
using System.Collections.ObjectModel;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Models
{
    public interface ISendersProvider
    {
        ObservableCollection<SenderConfigurationViewModel> GetSenders(SenderSectionLocator senderSectionLocator);

        bool SetSenders(ObservableCollection<SenderConfigurationViewModel> input, SenderSectionLocator senderSectionLocator);
    }
}
