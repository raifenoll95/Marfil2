using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RConfiguraciongraficos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuraciongraficas;
namespace Marfil.Dom.Persistencia.Model.Graficaslistados
{
    public enum Intervalotemporal
    {

    }

    public enum Tipografico
    {
        Area,
        Barras,
        Columnas,
        Lineas,
        Puntos,
        Tabla,
        Tarta

    }

    public class StOrdenPanelControl
    {
        public string Grafica { get; set; }
        public int Indice { get; set; }
    }

    public class GraficasModel : ConfiguraciongraficasModel
    {
        public DataTable Datos { get; set; }



        #region CTR

        public GraficasModel()
        {

        }

        public GraficasModel(ConfiguraciongraficasModel m)
        {
            var properties = typeof(ConfiguraciongraficasModel).GetProperties().Where(f => f.CanWrite);
            foreach (var item in properties)
            {
                item.SetValue(this, item.GetValue(m));
            }
        }

        #endregion
    }

    public class ConfiguraciongraficasWrapperModel : IToolbar
    {
        public string Idlistado { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<ConfiguraciongraficasModel> Lineas { get; set; }
        public ToolbarModel Toolbar { get; set; }
    }

    public class ConfiguraciongraficasModel : BaseModel<ConfiguraciongraficasModel, Configuraciongraficas>
    {
        private List<string> _columnasAgruparPor = new List<string>();
        private List<string> _columnasValores = new List<string>();

        #region Propiedades

        public string Empresa { get; set; }

        public Guid Usuario { get; set; }

        public string Idlistado { get; set; }

        public int Codigo { get; set; }

        [Required]
        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Titulo")]
        public string Titulo { get; set; }

        [Required]
        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Agruparpor")]
        public string Agruparpor { get; set; }

        [Required]
        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Valores")]
        public string Valores { get; set; }

        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Intervalotemporal")]
        public Intervalotemporal? Intervalotemporal { get; set; }

        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Apareceinicio")]
        public bool Apareceinicioview
        {
            get { return Apareceinicio ?? false; }
            set { Apareceinicio = value; }
        }
        public bool? Apareceinicio { get; set; }

        [Required]
        [Display(ResourceType = typeof(RConfiguraciongraficos), Name = "Tipografica")]
        public Tipografico Tipografica { get; set; }


        public IListados ListadoModel { get; set; }

        public List<string> ColumnasAgruparPor
        {
            get { return _columnasAgruparPor; }
            set { _columnasAgruparPor = value; }
        }

        public List<string> ColumnasValores
        {
            get { return _columnasValores; }
            set { _columnasValores = value; }
        }

        #endregion

        #region CTR

        public ConfiguraciongraficasModel()
        {
            
        }

        public ConfiguraciongraficasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override void createNewPrimaryKey()
        {
           
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Codigo.ToString();
        }

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RConfiguraciongraficos.TituloEntidad;
    }
}
