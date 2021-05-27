using System.Xml;

namespace Marfil.Inf.Genericos
{
    public interface ISerializer<T> where T : class
    {
        string GetXml(T obj);
        T SetXml(string xml);
    }
}