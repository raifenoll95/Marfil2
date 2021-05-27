using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IContextService: ICustomPrincipalData
    {
        string ServerMapPath(string ruta);
        bool IsAuthenticated();
        UrlHelper GetUrlHelper();

        bool IsObjectDictionaryEnabled();
        void SetObject(string key, object elem);
        object GetObject(string key);

        void SetItem(string key, object elem);
        object GetItem(string key);
    }
}
