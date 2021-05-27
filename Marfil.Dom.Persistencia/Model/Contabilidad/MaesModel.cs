using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using RCuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;


namespace Marfil.Dom.Persistencia.Model.Contabilidad.Maes
{
    public class MaesModel : BaseModel<MaesModel, Persistencia.Maes>
    {
        private int? _decimalesmonedas = 2;
        
        #region CTR

        public MaesModel()
        {

        }

        public MaesModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region Properties

        public int? Decimalesmonedas
        {
            get { return _decimalesmonedas; }
            set { _decimalesmonedas = value; }
        }



        [Display(ResourceType = typeof(RCuentas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        public string Empresa { get; set; }

        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovs), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        public string Fkcuentas { get; set; }
        //Importe


        [Display(ResourceType = typeof(RMovs), Name = "Saldo")]
        public decimal? Saldo { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Saldo")]
        public string SSaldo
        {
            ///get { return (Importe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            get { return (Saldo ?? 0).ToString(string.Format("N{0}", (2))); }
            set { Saldo = (Funciones.Qdecimal(value) ?? 0); }
        }


        //public short Esdebe { get; set; }


        #region Debe_Haber

        //Debe
        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        public decimal? Debe { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Debe")]
        public string SDebe
        {
            get { return (Debe ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Debe = (Funciones.Qdecimal(value) ?? 0); }
        }


        //Haber
        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        public decimal? Haber { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Haber")]
        public string SHaber
        {
            get { return (Haber ?? 0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Haber = (Funciones.Qdecimal(value) ?? 0); }
        }

        #endregion Debe_Haber

        #endregion



        public override object generateId(string id)
        {
            return id;
        }

        //public override string GetPrimaryKey()
        //{
        //    return Id;
        //}

        public override string DisplayName => RMovs.TituloEntidad;

    }
}
