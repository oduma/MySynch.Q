using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Common.Mappers;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.MVVM;

namespace MySynch.Q.Sender.Configurator.Mappers
{
    public class MapFilters:IMap<XmlElement,FiltersConfiguratorViewModel>
    {
        private readonly IMap<XmlElement, FilterConfigurationViewModel> _mapFilter;

        public MapFilters(IMap<XmlElement, FilterConfigurationViewModel> mapFilter)
        {
            _mapFilter = mapFilter;
        }

        public FiltersConfiguratorViewModel Map(XmlElement input)
        {
            if (_mapFilter == null)
                return null;
            if (input == null)
                return null;
            var result = new FiltersConfiguratorViewModel
            {
                Filters = new ObservableCollection<FilterConfigurationViewModel>()
            };
            foreach (XmlElement extensionNode in input.ChildNodes)
            {
                var filter = _mapFilter.Map(extensionNode);
                if(filter!=null)
                    result.Filters.Add(filter);
            }

            return result;
        }

        public XmlElement UnMap(FiltersConfiguratorViewModel input, XmlElement parrentElement)
        {
            if (_mapFilter == null)
                return null;
            if (input == null)
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            foreach (var filterConfigurationViewModel in input.Filters)
            {
                _mapFilter.UnMap(filterConfigurationViewModel,newElement);
            }
            return newElement;
        }
    }
}
