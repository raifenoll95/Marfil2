using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Marfil.Inf.Genericos
{
    public class Serializer<T> : ISerializer<T> where T : class
    {
        private readonly XmlSerializer _serializer;

        public Serializer()
        {
            _serializer = new XmlSerializer(typeof(T));
        }

        public string GetXml(T obj)
        {
            using (var sm = new StringWriter())
            {
                _serializer.Serialize(sm, obj);
                return sm.ToString();
            }
        }

        public T SetXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return Activator.CreateInstance<T>();

            using (TextReader reader = new StringReader(xml))
            {
                return _serializer.Deserialize(reader) as T;
            }
        }
    }
}