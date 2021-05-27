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
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;

using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
//using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosBalancePedidos 
    {

       
        public IContextService Context { get; set; }
        public MarfilEntities _db;
       
        public ApplicationHelper AppService { get; set; }

        private List<string> _series = new List<string>();

        public ListadosBalancePedidos(IContextService context)
        {
            Context = context;
            _db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
            System.DateTime moment = new System.DateTime();
            FechaInforme = DateTime.Today;
            FechaDesde = new DateTime(FechaInforme.Value.Year,1, 1);
            FechaHasta = new DateTime(FechaInforme.Value.Year, FechaInforme.Value.Month +1, 1).AddDays(-1);
        }




        [Display(ResourceType = typeof(RPedidos), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "PedidosDesde")]
        public string PedidoDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "PedidosHasta")]
        public string PedidoHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ClienteDesde")]
        public string ClienteDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ClienteHasta")]
        public string ClienteHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }


        public List<EstadosModel> Estados
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
        public string Estado { get; set; }



        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {

                var service = FService.Instance.GetService(typeof(SeriesModel), Context, _db) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.PedidosVentas);
            }
        }

      
    }

 }
