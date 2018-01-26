using System.Collections.ObjectModel;
using System.Configuration;
using System.Xml;
using MySynch.Q.Common.Configurators.Description;
using MySynch.Q.Common.Mappers;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Common.Configurators
{
    public class ConfigurationToViewModelProvider<T>:IConfigurationViewModelProvider<T> where T: ViewModelBase
    {
        private readonly IMap<XmlElement, ObservableCollection<T>> _mapper;
        public ConfigurationToViewModelProvider(IMap<XmlElement, ObservableCollection<T>> mapper)
        {
            _mapper = mapper;
        }

        public ObservableCollection<T> GetViewModelsCollection(ConfigurationSectionLocator configurationSectionLocator)
        {
            if (string.IsNullOrEmpty(configurationSectionLocator?.FilePath))
                return new ObservableCollection<T>();
            ConfigXmlDocument xmlDocument = new ConfigXmlDocument();
            xmlDocument.Load(configurationSectionLocator.FilePath);
            return ReadViewModelsFromConfig(xmlDocument,configurationSectionLocator.SectionIdentifier);
        }

        private ObservableCollection<T> ReadViewModelsFromConfig(ConfigXmlDocument xmlDocument, string sectionIdentifier)
        {
            if(_mapper==null)
                return new ObservableCollection<T>();
            if(string.IsNullOrEmpty(sectionIdentifier))
                throw new ConfigurationErrorsException();

            var sectionRootNode = xmlDocument.SelectSingleNode($"/{TargetConfigurationDescription.ConfigurationElementName}/{sectionIdentifier}");
            if (sectionRootNode == null)
                throw new ConfigurationErrorsException();
            var result = _mapper.Map((XmlElement) sectionRootNode.ChildNodes[0]);
            if(result==null)
                return new ObservableCollection<T>();
            return result;
        }

        public bool SetViewModelsCollection(ObservableCollection<T> input, ConfigurationSectionLocator configurationSectionLocator)
        {
            if (string.IsNullOrEmpty(configurationSectionLocator?.FilePath))
                return false;
            ConfigXmlDocument xmlDocument = new ConfigXmlDocument();
            xmlDocument.Load(configurationSectionLocator.FilePath);
            if (WriteSendersToConfig(xmlDocument, configurationSectionLocator.SectionIdentifier, input))
            {
                xmlDocument.Save(configurationSectionLocator.FilePath);
                return true;
            }
            return false;
        }

        private bool WriteSendersToConfig(ConfigXmlDocument xmlDocument, string sectionIdentifier, ObservableCollection<T> input)
        {
            if(string.IsNullOrEmpty(sectionIdentifier))
                throw new ConfigurationErrorsException();
            
            var sectionRootNode = xmlDocument.SelectSingleNode($"/{TargetConfigurationDescription.ConfigurationElementName}/{sectionIdentifier}");
            if (sectionRootNode == null)
                throw new ConfigurationErrorsException();
            sectionRootNode.RemoveAll();
            if (input == null)
                return true;
            var mapResult = _mapper?.UnMap(input, (XmlElement) sectionRootNode);
            if (mapResult == null)
                return false;
            if (sectionRootNode.ChildNodes.Count > 0)
                return true;
            return false;
        }
    }
}
