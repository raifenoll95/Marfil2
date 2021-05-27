using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI;
using Resources;
using REstados = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Estados;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TipoEstado
    {
        [StringValue(typeof(REstados), "EnumDiseno")]
        Diseño,
        [StringValue(typeof(REstados), "EnumCurso")]
        Curso,
        [StringValue(typeof(REstados), "EnumFinalizado")]
        Finalizado,
        [StringValue(typeof(REstados), "EnumAnulado")]
        Anulado,
        [StringValue(typeof(REstados), "EnumCaducado")]
        Caducado
    }

    public enum DocumentoEstado
    {
        [StringValue(typeof(REstados), "EnumTodos")]
        Todos = 99,
        [StringValue(typeof(REstados), "EnumPresupuestosVentas")]
        PresupuestosVentas = 0 ,
        [StringValue(typeof(REstados), "EnumPedidosVentas")]
        PedidosVentas = 1 ,
        [StringValue(typeof(REstados), "EnumAlbaranesVentas")]
        AlbaranesVentas = 2,
        [StringValue(typeof(REstados), "EnumFacturasVentas")]
        FacturasVentas = 3,
        [StringValue(typeof(REstados), "EnumPresupuestosCompras")]
        PresupuestosCompras = 4,
        [StringValue(typeof(REstados), "EnumPedidosCompras")]
        PedidosCompras = 5,
        [StringValue(typeof(REstados), "EnumAlbaranesCompras")]
        AlbaranesCompras = 6,
        [StringValue(typeof(REstados), "EnumFacturasCompras")]
        FacturasCompras = 7,
        [StringValue(typeof(REstados), "EnumReservas")]
        Reservasstock = 8,
        [StringValue(typeof(REstados), "EnumTraspasosalmacen")]
        Traspasosalmacen = 9,
        [StringValue(typeof(REstados), "EnumTransformaciones")]
        Transformaciones = 10,
        [StringValue(typeof(REstados), "EnumTransformacioneslotes")]
        Transformacioneslotes = 11,  
        [StringValue(typeof(REstados), "EnumKits")]
        Kits = 12,
        [StringValue(typeof(REstados), "EnumInventarios")]
        Inventarios = 13,
        [StringValue(typeof(REstados), "EnumCortabloques")]
        Cortabloques = 14,
        [StringValue(typeof(REstados), "EnumOportunidades")]
        Oportunidades = 20,
        [StringValue(typeof(REstados), "EnumProyectos")]
        Proyectos = 21,
        [StringValue(typeof(REstados), "EnumCampañas")]
        Campañas = 22,
        [StringValue(typeof(REstados), "EnumIncidencias")]
        Incidencias = 23,
        [StringValue(typeof(REstados), "EnumDivisionesLotes")]
        DivisionesLotes = 24,
        [StringValue(typeof(REstados), "EnumImputacionCostes")]
        ImputacionCostes = 25
    }

    public enum TipoMovimiento
    {
        [StringValue(typeof(REstados), "EnumManual")]
        Manual,
        [StringValue(typeof(REstados), "EnumAutomatico")]
        Automatico
    }

    public class EstadosModel : BaseModel<EstadosModel, Tiposcuentas>
    {
        

        #region Properties

        public string CampoId
        {
            get { return string.Format("{0}-{1}", (int)Documento, Id); }
        }

        [Required]
        [Display(ResourceType = typeof(REstados), Name = "Documento")]
        public DocumentoEstado Documento { get; set; }

        [Display(ResourceType = typeof(REstados), Name = "Id")]
        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(REstados), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(REstados), Name = "Imputariesgo")]
        public bool Imputariesgo { get; set; }

        [Required]
        [Display(ResourceType = typeof(REstados), Name = "Tipoestado")]
        public TipoEstado Tipoestado { get; set; }

        [Required]
        [Display(ResourceType = typeof(REstados), Name = "Tipomovimiento")]
        public TipoMovimiento Tipomovimiento { get; set; }

        [Display(ResourceType = typeof(REstados), Name = "Notas")]
        public string Notas { get; set; }

        #endregion

        #region CTR

        public EstadosModel()
        {

        }

        public EstadosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => REstados.TituloEntidad;

        public override string GetPrimaryKey()
        {
            return ((int)Documento) + "-" + Id;
        }
    }
}
