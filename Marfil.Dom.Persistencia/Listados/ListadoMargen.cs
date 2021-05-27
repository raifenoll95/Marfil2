using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;


namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoMargen
    {


        public IContextService Context { get; set; }
        public MarfilEntities _db;

        public ApplicationHelper AppService { get; set; }

        private List<string> _series = new List<string>();

        public ListadoMargen(IContextService context)
        {
            Context = context;
            _db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
            System.DateTime moment = new System.DateTime();
            FechaInforme = DateTime.Today;
            //FechaDesde = new DateTime(FechaInforme.Value.Year, 1, 1);
            //FechaHasta = new DateTime(FechaInforme.Value.Year, FechaInforme.Value.Month + 1, 1).AddDays(-1);
        }




        [Display(ResourceType = typeof(RPedidos), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteDesde")]
        public string ClienteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteHasta")]
        public string ClienteHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AgenteDesde")]
        public string AgenteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AgenteHasta")]
        public string AgenteHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ComercialDesde")]
        public string ComercialDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ComercialHasta")]
        public string ComercialHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ArticuloDesde")]
        public string ArticuloDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ArticuloHasta")]
        public string ArticuloHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesDesde")]
        public string MaterialDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesHasta")]
        public string MaterialHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }


        /*public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PedidosVentas).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Estado")]
        public string Estado { get; set; }*/



        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {

                var service = FService.Instance.GetService(typeof(SeriesModel), Context, _db) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.AlbaranesVentas);
            }
        }


    }

}
