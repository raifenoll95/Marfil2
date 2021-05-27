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
    public class PreferenciaEjercicioDefecto : ITipoPreferencia
    {
        public const string Id = "EjercicioDefecto";
        public const string Nombre = "Defecto";

        public TiposPreferencias Tipo => TiposPreferencias.EjercicioDefecto;

        [XmlIgnore]
        public Dictionary<string, string> ListEjercicios { get; private set; }

        public string[][] Preferencias
        {
            get { return GetRecordFields(); }
            set { SetRecordFields(value); }
        }
        private string[][] GetRecordFields()
        {

            return (
              from record in ListEjercicios
              select new [] {
      record.Key,
      record.Value
              }
            ).ToArray();
        }


        private void SetRecordFields(string[][] vector)
        {
            ListEjercicios = vector.ToDictionary(key => key[0], value => value[1]);
        }

        public void SetEjercicio(string empresa, string ejercicio)
        {
            if (!string.IsNullOrEmpty(ejercicio))
            {
                if (ListEjercicios == null)
                    ListEjercicios = new Dictionary<string, string>();

                if (ListEjercicios.ContainsKey(empresa))
                    ListEjercicios[empresa] = ejercicio;
                else
                {
                    ListEjercicios.Add(empresa, ejercicio);
                }

            }

        }

    }
}
