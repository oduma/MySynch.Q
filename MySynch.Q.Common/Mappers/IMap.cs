using System.Xml;

namespace MySynch.Q.Common.Mappers
{
    public interface IMap<T1,T2>
    {
        T2 Map(T1 input);

        T1 UnMap(T2 input, XmlElement parrentElement);
    }
}
