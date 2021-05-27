using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public enum CategoriaMovimientos
    {
        [StringValue(typeof(RAlbaranes), "CategoriaMovimientosPrincipal")]
        Principal,
        [StringValue(typeof(RAlbaranes), "CategoriaMovimientosSecundario")]
        Secudario
    }

    public class DiarioStockModel : IToolbar
    {
        public IContextService Context { get; set; }

        //[Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Categoria")]
        public CategoriaMovimientos Categoria { get; set; }


        [Display(ResourceType = typeof(RAlbaranes), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }



        public ToolbarModel Toolbar { get; set; }

        public IEnumerable<string> GetFiltros()
        {
            var list = new List<string>();

            if (!string.IsNullOrEmpty(Fkarticulos))
                list.Add(string.Format("{0}: {1}", RAlbaranes.Fkarticulos, Fkarticulos));

            if (!string.IsNullOrEmpty(Lote))
                list.Add(string.Format("{0}: {1}", RAlbaranes.Lote, Lote));

            if (FechaDesde.HasValue)
                list.Add(string.Format("{0}: {1}", RAlbaranes.FechaDesde, FechaDesde.Value.ToShortDateString()));

            if (FechaHasta.HasValue)
                list.Add(string.Format("{0}: {1}", RAlbaranes.FechaHasta, FechaHasta.Value.ToShortDateString()));

            if (Tipodealmacenlote.HasValue)
                list.Add(string.Format("{0}: {1}", RAlbaranes.TipoAlmacenLote, Tipodealmacenlote.ToString()));

            return list;
        }
    }
}
