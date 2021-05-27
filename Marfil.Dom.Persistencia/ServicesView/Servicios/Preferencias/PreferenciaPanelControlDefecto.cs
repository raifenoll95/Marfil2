using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Graficaslistados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{


    [Serializable]
    public class PreferenciaPanelControlDefecto : ITipoPreferencia
    {
        public const string Id = "PanelControlDefecto";
        public const string Nombre = "Defecto";

        public TiposPreferencias Tipo => TiposPreferencias.PanelControlDefecto;

        [XmlIgnore]
        public Dictionary<string, string> ListPanelControles { get; private set; }

        public string[][] Preferencias
        {
            get { return GetRecordFields(); }
            set { SetRecordFields(value); }
        }
        private string[][] GetRecordFields()
        {
            if (ListPanelControles == null)
                return null;
            return (
              from record in ListPanelControles
              select new [] {
      record.Key,
      record.Value
              }
            ).ToArray();
        }

        private void SetRecordFields(string[][] vector)
        {
            ListPanelControles = vector.ToDictionary(key => key[0], value => value[1]);
        }

        public List<string> GetPanelesControl(string empresa)
        {
            if (ListPanelControles.ContainsKey(empresa))
            return (ListPanelControles[empresa] ?? string.Empty).Split(',').Where(f=>!string.IsNullOrEmpty(f)).ToList();

            return new List<string>();
        }

        public void SetPanelControl(string empresa, IEnumerable<StOrdenPanelControl> vector)
        {
            if (ListPanelControles.ContainsKey(empresa))
                ListPanelControles.Remove(empresa);

            foreach(var item in vector.OrderBy(f=>f.Indice))
                SetPanelControl(empresa, item);
        }

        public void SetPanelControl(string empresa, StOrdenPanelControl panelControl)
        {
            if (!string.IsNullOrEmpty(panelControl.Grafica))
            {
                if (ListPanelControles == null)
                    ListPanelControles = new Dictionary<string, string>();

                if (ListPanelControles.ContainsKey(empresa))
                {
                    var listado=ListPanelControles[empresa];
                    var vector= listado.Split(',');
                    if (!vector.Contains(panelControl.Grafica))
                    {
                        if (panelControl.Indice >= 0)
                        {
                            listado += "," + panelControl.Grafica;
                            ListPanelControles[empresa] = listado;
                        }
                        else
                        {
                            var aux = vector.ToList();
                            if (panelControl.Indice < 0)
                                panelControl.Indice = 0;
                            aux.Insert(panelControl.Indice,panelControl.Grafica);
                            ListPanelControles[empresa] = string.Join(",",aux);
                        }
                        
                    }
                    else
                    {
                        var aux = vector.ToList();
                        
                        if (panelControl.Indice < 0)
                            panelControl.Indice = aux.IndexOf(panelControl.Grafica); 
                        else if (aux.Count <= panelControl.Indice )
                            panelControl.Indice = aux.Count - 1;
                        aux.Remove(panelControl.Grafica);
                        aux.Insert(panelControl.Indice, panelControl.Grafica);
                        ListPanelControles[empresa] = string.Join(",", aux);
                    }
                }
                else
                {
                    ListPanelControles.Add(empresa, panelControl.Grafica);
                }
            }
        }

        public void DeletePanelControl(string empresa, string panelControl)
        {
            if (!string.IsNullOrEmpty(panelControl))
            {
                if (ListPanelControles == null)
                    ListPanelControles = new Dictionary<string, string>();

                if (ListPanelControles.ContainsKey(empresa))
                {
                    var listado =ListPanelControles[empresa];
                    
                    var vector = listado.Split(',').ToList();
                    if (vector.Contains(panelControl))
                    {
                        vector.Remove(panelControl);
                        ListPanelControles[empresa] =string.Join(",",vector);
                    }
                }
            }
        }
    }
}
