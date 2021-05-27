using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.DataAccess.Sql;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{


    [Serializable]
    public class PreferenciaAlmacenDefecto : ITipoPreferencia
    {
        public const string Id = "AlmacenDefecto";
        public const string Nombre = "Defecto";

        public TiposPreferencias Tipo => TiposPreferencias.AlmacenDefecto;

        [XmlIgnore]
        public Dictionary<string, string> ListAlmacenes { get; private set; }

        public string[][] Preferencias
        {
            get { return GetRecordFields(); }
            set { SetRecordFields(value); }
        }
        private string[][] GetRecordFields()
        {
            if (ListAlmacenes == null)
                return null;
            return (
              from record in ListAlmacenes
              select new [] {
      record.Key,
      record.Value
              }
            ).ToArray();
        }


        private void SetRecordFields(string[][] vector)
        {
            ListAlmacenes = vector.ToDictionary(key => key[0], value => value[1]);
        }

        public void SetAlmacen(string empresa, string almacen)
        {
            if (!string.IsNullOrEmpty(almacen))
            {
                if (ListAlmacenes == null)
                    ListAlmacenes = new Dictionary<string, string>();

                if (ListAlmacenes.ContainsKey(empresa))
                    ListAlmacenes[empresa] = almacen;
                else
                {
                    ListAlmacenes.Add(empresa, almacen);
                }

            }

        }

    }
}
