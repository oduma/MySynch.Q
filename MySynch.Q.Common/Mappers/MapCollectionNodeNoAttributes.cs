using System.Collections.ObjectModel;
using System.Xml;
using Sciendo.Common.WPF.MVVM;

namespace MySynch.Q.Common.Mappers
{
    public class MapCollectionNodeNoAttributes<T>:IMap<XmlElement,ObservableCollection<T>> where T:ViewModelBase
    {
        private readonly string _collectionNodeName;

        private readonly IMap<XmlElement,T> _mapCollectionElement;

        public MapCollectionNodeNoAttributes(
            IMap<XmlElement, T> mapCollectionElement, string collectionNodeName)
        {
            _mapCollectionElement = mapCollectionElement;
            _collectionNodeName = collectionNodeName;
        }

        public ObservableCollection<T> Map(XmlElement input)
        {
            if (_mapCollectionElement == null)
                return null;
            if (input == null)
                return null;
            var result = new ObservableCollection<T>();

            foreach (var senderNode in input.ChildNodes)
            {
                var mappedSender = _mapCollectionElement.Map((XmlElement) senderNode);
                if(mappedSender!=null)
                    result.Add(mappedSender);
            }

            return result;
        }

        public XmlElement UnMap(ObservableCollection<T> input, XmlElement parrentElement)
        {
            if (_mapCollectionElement == null)
                return null;
            if (input == null)
                return null;
            if (parrentElement == null)
                return null;
            var newElement = parrentElement.CreateElement(_collectionNodeName);
            foreach (var sender in input)
            {
                _mapCollectionElement.UnMap(sender,newElement);
            }
            return newElement;
        }
    }
}
