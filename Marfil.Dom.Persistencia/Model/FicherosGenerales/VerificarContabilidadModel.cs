using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class VerificarContabilidadModel : BaseModel<VerificarContabilidadModel, VerificarContabilidad>
    {
        private int? _decimalesmonedas = 2;

        public override string DisplayName => "Verificar contabilidad";

        public override object generateId(string id)
        {
            return id;
        }

        #region CTR

        public VerificarContabilidadModel()
        {

        }

        public VerificarContabilidadModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region propiedades
        public int? Decimalesmonedas
        {
            get { return _decimalesmonedas; }
            set { _decimalesmonedas = value; }
        }

        public int Id { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Nivel")]
        public string Nivel { get; set; }

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
    }
}
