using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using Marfil.Inf.Genericos;
using RPeticiones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PeticionesAsincronas;

namespace Marfil.Dom.Persistencia.Model
{

    public enum EstadoPeticion
    {        
        [StringValue(typeof(RPeticiones), "EstadoPeticionEnCurso")]        
        EnCurso,        
        [StringValue(typeof(RPeticiones), "EstadoPeticionError")]
        Error,
        [StringValue(typeof(RPeticiones), "EstadoPeticionFinalizadaLogs")]
        FinalizadaLogs,
        [StringValue(typeof(RPeticiones), "EstadoPeticionFinalizada")]
        Finalizada
    }

    public enum TipoPeticion
    {
        [StringValue(typeof(RPeticiones), "TipoPeticionImportacion")]
        Importacion,
        [StringValue(typeof(RPeticiones), "TipoPeticionCalculo")]
        Calculo
    }

    public enum TipoImportacion
    {        
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarMovs")]
        ImportarMovs,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarCuentas,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarStock,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarArticulos,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarClientes,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarProveedores,
        [StringValue(typeof(RPeticiones), "TipoImportacionImportarCuentas")]
        ImportarAcreedores
    }
    
    public class PeticionesAsincronasModel : BaseModel<PeticionesAsincronasModel, PeticionesAsincronas>
    {
        #region Properties

        public string Empresa { get; set; }
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoPeticion Estado { get; set; }
        public TipoPeticion Tipo { get; set; }
        public string Configuracion { get; set; }
        public string Resultado { get; set; }     

        #endregion

        #region CTR

        public PeticionesAsincronasModel()
        {
            
        }

        public PeticionesAsincronasModel(IContextService context) : base(context)
        {
            
        }

        #endregion

        public override object generateId(string id)
        {
            return new Guid(id);
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        public override string DisplayName => RPeticiones.TituloEntidad;

    }
}
