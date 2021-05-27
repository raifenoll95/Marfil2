using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using RTablas=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias;
namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias
{
    public enum TipoTablaVaria
    {
        [StringValue(typeof(RTablas), "TipoTablaOtros")]
        Otros,
        [StringValue(typeof(RTablas),"TipoTablaGestion")]
        Gestion,
        [StringValue(typeof(RTablas), "TipoTablaConfiguracion")]
        Configuracion,
        [StringValue(typeof(RTablas), "TipoTablaProduccion")]
        Produccion,
        [StringValue(typeof(RTablas), "TipoTablaContabilidad")]
        Contabilidad,
        [StringValue(typeof(RTablas), "TipoTablaCRM")]
        CRM
    }

    public interface IListTablasVariasModel : IModelView
    {
        [Display(ResourceType = typeof(RTablas), Name = "Id")]
        int Id { get; set; }
        [Display(ResourceType = typeof(RTablas), Name = "Nombre")]
        string Nombre { get; set; }
        [Display(ResourceType = typeof(RTablas), Name = "Tipo")]
        TipoTablaVaria Tipo { get; set; }
        [Display(ResourceType = typeof(RTablas), Name = "Noeditable")]
        bool Noeditable { get; set; }
    }

    public class BaseTablasVariasModel : BaseModel<BaseTablasVariasModel, Tablasvarias>, ITablasVariasModel, IGetColumnasGrid, IListTablasVariasModel
    {
        #region Properties

        private int _id;
        [Display(ResourceType = typeof(RTablas), Name = "Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _clase;
        [Display(ResourceType = typeof(RTablas), Name = "Clase")]
        public string Clase
        {
            get { return _clase; }
            set { _clase = value; }
        }

        private string _nombre;
        [Display(ResourceType = typeof(RTablas), Name = "Nombre")]
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        [Display(ResourceType = typeof(RTablas), Name = "Tipo")]
        public TipoTablaVaria Tipo { get; set; }

        [Display(ResourceType = typeof(RTablas), Name = "Noeditable")]
        public bool Noeditable { get; set; }

        public List<dynamic> Lineas
        {
            get;
            set;
        }

        public object EmptyLineas()
        {
            var tipo = Helper.GetTypeFromFullName(Clase);
            var serializerType = typeof(List<>);
            var genericType = serializerType.MakeGenericType(tipo);
            return Activator.CreateInstance(genericType);
        }

        #endregion

        #region CTR

        public BaseTablasVariasModel()
        {
            _clase = GetType().FullName;
        }

        public BaseTablasVariasModel(IContextService context) : base(context)
        {
            _clase = GetType().FullName;
        }

        #endregion

        public override object generateId(string id)
        {
            return int.Parse(id);
        }

        public IEnumerable<ColumnDefinition> GetColumnDefinitions()
        {
            return new[]
            {
                new ColumnDefinition() {field = "Id", displayName = "Id", visible = false},
                new ColumnDefinition() {field = "Nombre", displayName = "Nombre", visible = true}
            };
        }

        public override string DisplayName => RTablas.TituloEntidad;
    }
}
