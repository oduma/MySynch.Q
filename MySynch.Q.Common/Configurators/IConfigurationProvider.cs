using System.Collections.Generic;

namespace MySynch.Q.Common.Configurators
{
    public interface IConfigurationProvider
    {
        IEnumerable<ConfigurationSectionLocator> GetConfigInfo();
    }
}
