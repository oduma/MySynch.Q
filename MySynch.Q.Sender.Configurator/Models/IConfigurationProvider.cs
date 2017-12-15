using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Sender.Configurator.Models
{
    public interface IConfigurationProvider
    {
        SenderSectionLocator GetConfigInfo();
    }
}
