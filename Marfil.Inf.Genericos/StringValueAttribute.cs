using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos
{
    public class StringValueAttribute : Attribute
    {
        private readonly string _value;
        private readonly Type _resource;
        private readonly string _name;

        public StringValueAttribute(Type resource, string name)
        {
            _resource = resource;
            _name = name;
        }

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get
            {
                if (_resource != null)
                {
                    var rm=new ResourceManager(_resource);
                    return rm.GetString(_name);
                }
                return _value;
            }
        }

    }
}
