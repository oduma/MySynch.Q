using System;
using System.Configuration;
using System.IO;
using MySynch.Q.Sender.Configurator.Configuration;

namespace MySynch.Q.Sender.Configurator.Models
{
    public class ConfigurationProvider:IConfigurationProvider
    {
        public SenderSectionLocator GetConfigInfo()
        {
            var configSection = ConfigurationManager.GetSection("senderLocator") as SenderLocatorSection;
            if (string.IsNullOrEmpty(configSection?.Location))
                return null;
            if (string.IsNullOrEmpty(configSection.SectionId))
                return null;
            var result = new SenderSectionLocator();
            result.SectionIdentifier = configSection.SectionId;
            if (File.Exists(configSection.Location))
            {
                result.FilePath = configSection.Location;
                return result;
            }
            result.FilePath= File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSection.Location)) 
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSection.Location) : string.Empty;
            return result;
        }
    }
}
