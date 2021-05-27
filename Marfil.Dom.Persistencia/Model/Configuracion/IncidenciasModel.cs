using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI;
using Resources;
using RIncidencias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Incidencias;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TipoMaterial
    {
        [StringValue(typeof(RIncidencias), "EnumMaterial")]
        Material,
        [StringValue(typeof(RIncidencias), "EnumProduccion")]
        Produccion
    }

    public enum TipoDocumentoMaterial
    {
        [StringValue(typeof(RIncidencias), "EnumTodos")]
        Todos
    }

    public class IncidenciasModel : BaseModel<IncidenciasModel, Incidencias>
    {

        #region Properties

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RIncidencias), Name = "Id")]
        public string Id { get; set; }
 
        [Required]
        [Display(ResourceType = typeof(RIncidencias), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RIncidencias), Name = "Tipomaterial")]
        public TipoMaterial Tipomaterial { get; set; }

        [Display(ResourceType = typeof(RIncidencias), Name = "Documento")]
        public TipoDocumentoMaterial Documento { get; set; }

        [Display(ResourceType = typeof(RIncidencias), Name = "Fkgrupo")]
        public string Fkgrupo { get; set; }

        #endregion

        #region CTR

        public IncidenciasModel()
        {

        }

        public IncidenciasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RIncidencias.TituloEntidad;
        
        public override string GetPrimaryKey()
        {
            return Id;
        }
    }


}
