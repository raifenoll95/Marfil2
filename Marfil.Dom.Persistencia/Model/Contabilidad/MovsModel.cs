using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Diseñador;

using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;



using Marfil.Inf.Genericos.Helper;
using Resources;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;

namespace Marfil.Dom.Persistencia.Model.Contabilidad.Movs
{
    public class StMovs
    {
        public DateTime? Fecha { get; set; }

        public string Fechadocumento
        {
            get { return Fecha.Value.ToShortDateString(); }
        }
    }

    public enum GenerarMovimientoAPartirDe
    {
       AsignarCartera
    }


    public class MovsModel : BaseModel<MovsModel, Persistencia.Movs>
    {
        private int? _decimalesmonedas = 2;
        private List<MovsLinModel> _lineas = new List<MovsLinModel>();

        #region CTR

        public MovsModel()
        {
            
        }

        public MovsModel(IContextService context) : base(context)
        {
            //using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            //{
            //    Fkseriescontables = db.SeriesContables.Where(f => f.empresa == context.Empresa && f.fkejercicios == context.Ejercicio).Select(f => f.id)/*.SingleOrDefault()*/;
            //}
        }

        #endregion

        #region Properties


        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }

        public string Empresa { get; set; }




        [Required]
        public Guid? Integridadreferencial { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "Fkseriescontables")]
        public string Fkseriescontables { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }


        [Display(ResourceType = typeof(RMovs), Name = "ReferenciaLibre")]
        public string Referencialibre { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "Fecha")]

        public DateTime? Fecha { get; set; }


        [Display(ResourceType = typeof(RMovs), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }


        public string Fechacadena
        {
            get { return Fecha?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "TipoAsiento")]
        //public string Tipoasiento { get; set; }
        public string Tipoasiento { get; set; }

        //[Display(ResourceType = typeof(RMovs), Name = "TipoAsiento")]
        //public TipoAsientoContable Tipoasientoenum
        //    { 
        //    get { return (TipoAsientoContable)Tipoasiento; }
        //    set
        //    {
        //        Tipoasiento = (int)value;
        //    }
        //}

        [Display(ResourceType = typeof(RMovs), Name = "Codigodescripcionasiento")]
        public string Codigodescripcionasiento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "Descripcionasiento")]
        public string Descripcionasiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Vinculo")]
        public string Vinculo { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Canalcontable")]
        public string Canalcontable { get; set; }


        [Display(ResourceType = typeof(RMovs), Name = "Traza")]
        public string Traza { get; set; }
        

        [Display(ResourceType = typeof(RMovs), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        public int Decimalesmonedas { get; set; }

        public GenerarMovimientoAPartirDe Generar { get; set; }


        [Display(ResourceType = typeof(RMovs), Name = "Bloqueado")]
        public Nullable<bool> Bloqueado { get; set; }


        //Debe
        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        public decimal? Debe { get; set; }
        

        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        public string SDebe
        {
            get { return (Debe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ))); }
            set { Debe = (Funciones.Qdecimal(value) ?? 0); }
        }


        //Haber
        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        public decimal? Haber { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        public string SHaber
        {
            get { return (Haber ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas))); }
            set { Haber = (Funciones.Qdecimal(value) ?? 0); }
        }

        //Saldo
        [Display(ResourceType = typeof(RMovs), Name = "Saldo")]
        public decimal? Saldo { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Saldo")]
        public string SSaldo
        {
            get { return (Saldo ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas))); }
            set { Haber = (Funciones.Qdecimal(value) ?? 0); }
        }

        #endregion

        
        //lineas
        public List<MovsLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        public override object generateId(string id)
        {
            return id;
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RMovs.TituloEntidad;

        //public DocumentosBotonImprimirModel GetListFormatos()
        //{
        //    var user = Context;
        //    using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
        //    {
        //        var servicePreferencias = new PreferenciasUsuarioService(db);
        //        var doc = servicePreferencias.GetDocumentosImpresionMantenimiento
        //            (user.Id,
        //                TipoAsientoImpresion.Normal.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;

        //        var service = new DocumentosUsuarioService(db);
        //        {
        //            var lst =
        //                service.GetDocumentos(TipoDocumentoImpresion.FacturasVentas, user.Id)
        //                    .Where(f => f.Tiporeport == TipoReport.Report);
        //            return new DocumentosBotonImprimirModel()
        //            {
        //                Tipo = TipoDocumentoImpresion.FacturasVentas,
        //                Lineas = lst.Select(f => f.Nombre),
        //                Primarykey = Referencia,
        //                Defecto = doc?.Name
        //            };
        //        }
        //    }

        //}



        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.Asientos.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.Asientos, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.Asientos,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }
        }
    }


    #region Líneas
    public class MovsLinModel : IMovsLinModel
    {
        private int? _decimalesmonedas = 2;
        private int? _orden;
        

        //private string _loteautomaticoid;
        public int Id { get; set; }
        public bool Nueva { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Orden")]
        public int? Orden
        {
            get { return _orden ?? (Id * ApplicationHelper.EspacioOrdenLineas); }
            set { _orden = value; }
        }



        public int? Decimalesmonedas
        {
            get { return _decimalesmonedas; }
            set { _decimalesmonedas = value; }
        }

        public Guid Flagidentifier { get; set; }
        
        public short Esdebe { get; set; }


        [Required]
        
        
        [Display(ResourceType = typeof(RMovs), Name = "Fkcuentas")]
        //[MinLength(8, ErrorMessage = "Faltan dígitos en la cuenta")]
        [MinLength(8, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        //[MinLength(9, ErrorMessage = (ResourceType = typeof(RMovs), Name = "ErrorLongitudCuenta").ToString())]
        public string Fkcuentas { get; set; }


        [Display(ResourceType = typeof(RMovs), Name = "Fkseccionesanaliticas")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Fkseccionesanaliticas { get; set; }
        
        #region Debe_Haber
        //Importe
        [Display(ResourceType = typeof(RMovs), Name = "Importe")]
        public decimal Importe { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Importe")]
        public string SImporte
        {
            get { return (Importe ).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Importe = (Funciones.Qdecimal(value) ?? 0); }
        }

        //Debe
        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        //public decimal? Debe { get; set; }
        public decimal? Debe
        {
            get
            {
                if (Esdebe == 1) return Importe;
                return null;
            }
            set
            {
                if(value != null && value != 0)
                {
                    Importe = value.Value;
                    Esdebe = 1;
                }
                else
                {
                    Esdebe = -1;
                }
            }
        }

        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        public string SDebe
        {
            get { return (Debe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Debe = (Funciones.Qdecimal(value) ?? 0); }
        }


        //Haber
        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        //public decimal? Haber { get; set; }
        public decimal? Haber
        {
            get
            {
                if (Esdebe == -1) return Importe;
                return null;
            }
            set
            {
                if (value != null && value != 0)
                {
                    Importe = value.Value;
                    Esdebe = -1;
                }
                else
                {
                    Esdebe = 1;
                }
            }
        }

        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        public string SHaber
        {
            get { return (Haber ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Haber = (Funciones.Qdecimal(value) ?? 0); }
        }

        #endregion Debe_Haber

        //Importemoneda        
        [Display(ResourceType = typeof(RMovs), Name = "Importemonedadocumento")]
        public decimal? Importemonedadocumento { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Importe")]
        public string SImportemonedadocumento
        {
            get { return (Importemonedadocumento  ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Importemonedadocumento = Funciones.Qdecimal(value); }
        }
        
        [Display(ResourceType = typeof(RMovs), Name = "Codigocomentario")]
        public string Codigocomentario { get; set; }
        
        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RMovs), Name = "Comentario")]
        public string Comentario { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Conciliado")]
        public Nullable<bool> Conciliado { get; set; }

    }
    #endregion lineas

}
