using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;

namespace Marfil.Dom.Persistencia.Model
{
    [Serializable]
    public class PermisosModel
    {
        private List<PermisosItemModel> _permisos = new List<PermisosItemModel>();
        public List<PermisosItemModel> Items
        {
            get
            {
                return _permisos;
            }
            set { _permisos = value; }
        }

        
        
    }
    [Serializable]
    public class PermisosItemModel
    {
        public string Nombre { get; set; }
        public bool Visible { get; set; }
        public bool Crear { get; set; }
        public bool Modificar { get; set; }
        public bool Eliminar { get; set; }
        public bool Bloquear { get; set; }

        private List<PermisosItemModel> _permisos = new List<PermisosItemModel>();
        public List<PermisosItemModel> Permisos
        {
            get
            {
                return _permisos;
            }
            set { _permisos = value; }
        }
    }
}
