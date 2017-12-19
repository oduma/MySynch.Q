using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MySynch.Q.Common.Configurators.Configuration;

namespace MySynch.Q.Common.Configurators
{
    public class ConfigurationProvider:IConfigurationProvider
    {
        private const string ConfigurationproviderSection = "locatorsSection";
        public IEnumerable<ConfigurationSectionLocator> GetConfigInfo()
        {
            var configSection = ConfigurationManager.GetSection(ConfigurationproviderSection) as ConfigurationLocatorSection;
            if(configSection!=null && configSection.Locators!=null && configSection.Locators.Count>=1)
            foreach (var locator in configSection.Locators)
            {
                var configSectionLocator = GetConfigInfo((LocatorElement)locator);
                if (configSectionLocator != null)
                    yield return configSectionLocator;
            }

        }

        private ConfigurationSectionLocator  GetConfigInfo(LocatorElement locator)
        {
            if (string.IsNullOrEmpty(locator?.Location))
                return null;
            if (string.IsNullOrEmpty(locator.SectionId))
                return null;
            var result = new ConfigurationSectionLocator();
            result.SectionIdentifier =locator.SectionId;
            if (File.Exists(locator.Location))
            {
                result.FilePath = locator.Location;
                return result;
            }
            result.FilePath = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, locator.Location))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, locator.Location) : string.Empty;
            return result;
        }
    }
}
