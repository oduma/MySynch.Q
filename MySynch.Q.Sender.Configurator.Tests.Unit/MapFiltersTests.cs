using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MySynch.Q.Sender.Configurator.Configuration;
using MySynch.Q.Sender.Configurator.Mappers;
using MySynch.Q.Sender.Configurator.MVVM;
using NUnit.Framework;
using Rhino.Mocks;

namespace MySynch.Q.Sender.Configurator.Tests.Unit
{
    [TestFixture]
    public class MapFiltersTests
    {
        [Test]
        public void MapFiltersNoMapFilter()
        {

            MapFilters mapFilters = new MapFilters(null);
            Assert.IsNull(mapFilters.Map(null));
        }

        [Test]
        public void MapFiltersNoElement()
        {
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            Assert.IsNull(mapFilters.Map(null));
        }

        [Test]
        public void MapFiltersElementNoChildNodes()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var filterElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            var filters = mapFilters.Map(filterElement);
            Assert.IsNotNull(filters);
            Assert.IsNotNull(filters.Filters);
            Assert.IsEmpty(filters.Filters);
        }

        [Test]
        public void MapFiltersMapFilterMapsToNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var filterElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            filterElement.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            mockMapFilter.Expect(m => m.Map(filterElement)).IgnoreArguments().Return(null);
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            var filters = mapFilters.Map(filterElement);
            Assert.IsNotNull(filters);
            Assert.IsNotNull(filters.Filters);
            Assert.IsEmpty(filters.Filters);
        }

        [Test]
        public void MapFiltersElemenOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var filterElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FiltersCollectionElementName);
            filterElement.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            mockMapFilter.Expect(m => m.Map(filterElement))
                .IgnoreArguments()
                .Return(new FilterConfigurationViewModel { Key = "keyValue", Value = "valueValue" });
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            var filters = mapFilters.Map(filterElement);
            Assert.IsNotNull(filters);
            Assert.IsNotNull(filters.Filters);
            Assert.IsNotEmpty(filters.Filters);
            Assert.AreEqual(1,filters.Filters.Count);
            Assert.AreEqual("keyValue", filters.Filters[0].Key);
            Assert.AreEqual("valueValue", filters.Filters[0].Value);
        }

        [Test]
        public void UnMapFiltersNoMapFilter()
        {

            MapFilters mapFilters = new MapFilters(null);
            Assert.IsNull(mapFilters.UnMap(null,null));
        }

        [Test]
        public void UnMapEmptyViewModel()
        {
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            Assert.IsNull(mapFilters.UnMap(null,null));
        }

        [Test]
        public void UnMapEmptyParrentElement()
        {
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            Assert.IsNull(mapFilters.UnMap(new FiltersConfiguratorViewModel(), null));
        }

        [Test]
        public void UnMapUnMapFilterReturnsNull()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            mockMapFilter.Expect(m => m.UnMap(new FilterConfigurationViewModel(),senderElement)).IgnoreArguments().Return(null);
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            var filtersElement =
                mapFilters.UnMap(
                    new FiltersConfiguratorViewModel
                    {
                        Filters =
                            new ObservableCollection<FilterConfigurationViewModel>
                            {
                                new FilterConfigurationViewModel {Key = "keyValue", Value = "valueValue"}
                            }
                    },
                    senderElement);
            Assert.AreEqual(TargetFilterConfigurationDescription.FilterElementName,senderElement.Name);
            Assert.AreEqual(1,senderElement.ChildNodes.Count);
            Assert.AreEqual(TargetFilterConfigurationDescription.FiltersCollectionElementName,senderElement.ChildNodes[0].Name);
            Assert.False(senderElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetFilterConfigurationDescription.FiltersCollectionElementName,filtersElement.Name);
            Assert.AreEqual(senderElement,filtersElement.ParentNode);
        }

        [Test]
        public void UnMapNoItemsInTheCollection()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetFilterConfigurationDescription.FilterElementName);
            var mockMapFilter = MockRepository.Mock<IMap<XmlElement, FilterConfigurationViewModel>>();
            mockMapFilter.Expect(m => m.UnMap(new FilterConfigurationViewModel(), senderElement)).IgnoreArguments().Return(null);
            MapFilters mapFilters = new MapFilters(mockMapFilter);
            var filtersElement =
                mapFilters.UnMap(
                    new FiltersConfiguratorViewModel
                    {
                        Filters = new ObservableCollection<FilterConfigurationViewModel>()
                    },
                    senderElement);
            Assert.AreEqual(TargetSenderConfigurationDescription.SenderElementName, senderElement.Name);
            Assert.AreEqual(1, senderElement.ChildNodes.Count);
            Assert.AreEqual(TargetFilterConfigurationDescription.FiltersCollectionElementName, senderElement.ChildNodes[0].Name);
            Assert.False(senderElement.ChildNodes[0].HasChildNodes);
            Assert.AreEqual(TargetFilterConfigurationDescription.FiltersCollectionElementName, filtersElement.Name);
            Assert.AreEqual(senderElement, filtersElement.ParentNode);
        }
        [Test]
        public void UnMapOk()
        {
            XmlDocument xmlDocument = new XmlDocument();
            var senderElement = xmlDocument.CreateElement(TargetSenderConfigurationDescription.SenderElementName);
            MapFilters mapFilters = new MapFilters(new MapFilter());
            var filtersElement =
                mapFilters.UnMap(
                    new FiltersConfiguratorViewModel
                    {
                        Filters =
                            new ObservableCollection<FilterConfigurationViewModel>
                            {
                                new FilterConfigurationViewModel {Key = "keyValue", Value = "valueValue"}
                            }
                    },
                    senderElement);
            Assert.AreEqual(senderElement, filtersElement.ParentNode);
            Assert.False(filtersElement.HasAttributes);
            Assert.AreEqual(TargetFilterConfigurationDescription.FiltersCollectionElementName,filtersElement.Name);
            Assert.AreEqual(1,filtersElement.ChildNodes.Count);
            Assert.AreEqual("keyValue",filtersElement.ChildNodes[0].Attributes[TargetFilterConfigurationDescription.Key].Value);
            Assert.AreEqual("valueValue", filtersElement.ChildNodes[0].Attributes[TargetFilterConfigurationDescription.Value].Value);
        }

    }
}
