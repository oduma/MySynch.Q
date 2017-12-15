using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Models
{
    public class SendersProvider:ISendersProvider
    {
        private readonly IMap<XmlElement, ObservableCollection<SenderConfigurationViewModel>> _mapSenders;
        public SendersProvider(IMap<XmlElement, ObservableCollection<SenderConfigurationViewModel>> mapSenders)
        {
            _mapSenders = mapSenders;
        }

        public ObservableCollection<SenderConfigurationViewModel> GetSenders(SenderSectionLocator senderSectionLocator)
        {
            if (string.IsNullOrEmpty(senderSectionLocator?.FilePath))
                return new ObservableCollection<SenderConfigurationViewModel>();
            ConfigXmlDocument xmlDocument = new ConfigXmlDocument();
            xmlDocument.Load(senderSectionLocator.FilePath);
            return ReadSendersFromConfig(xmlDocument,senderSectionLocator.SectionIdentifier);
        }

        private ObservableCollection<SenderConfigurationViewModel> ReadSendersFromConfig(ConfigXmlDocument xmlDocument, string sectionIdentifier)
        {
            if(_mapSenders==null)
                return new ObservableCollection<SenderConfigurationViewModel>();
            if(string.IsNullOrEmpty(sectionIdentifier))
                throw new ConfigurationErrorsException();

            var sectionRootNode = xmlDocument.SelectSingleNode($"/{TargetConfigurationDescription.ConfigurationElementName}/{sectionIdentifier}");
            if (sectionRootNode == null)
                throw new ConfigurationErrorsException();
            var result = _mapSenders.Map((XmlElement) sectionRootNode.ChildNodes[0]);
            if(result==null)
                return new ObservableCollection<SenderConfigurationViewModel>();
            return result;
        }

        public bool SetSenders(ObservableCollection<SenderConfigurationViewModel> input, SenderSectionLocator senderSectionLocator)
        {
            if (string.IsNullOrEmpty(senderSectionLocator?.FilePath))
                return false;
            ConfigXmlDocument xmlDocument = new ConfigXmlDocument();
            xmlDocument.Load(senderSectionLocator.FilePath);
            if (WriteSendersToConfig(xmlDocument, senderSectionLocator.SectionIdentifier, input))
            {
                xmlDocument.Save(senderSectionLocator.FilePath);
                return true;
            }
            return false;
        }

        private bool WriteSendersToConfig(ConfigXmlDocument xmlDocument, string sectionIdentifier, ObservableCollection<SenderConfigurationViewModel> input)
        {
            if(string.IsNullOrEmpty(sectionIdentifier))
                throw new ConfigurationErrorsException();
            
            var sectionRootNode = xmlDocument.SelectSingleNode($"/{TargetConfigurationDescription.ConfigurationElementName}/{sectionIdentifier}");
            if (sectionRootNode == null)
                throw new ConfigurationErrorsException();
            sectionRootNode.RemoveAll();
            if (input == null)
                return true;
            var mapResult = _mapSenders?.UnMap(input, (XmlElement) sectionRootNode);
            if (mapResult == null)
                return false;
            if (sectionRootNode.ChildNodes.Count > 0)
                return true;
            return false;
        }
    }
}
