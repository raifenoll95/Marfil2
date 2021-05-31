using System;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.Dom.Persistencia.Listados
{

    public class ToolbarListadosModel : ToolbarModel
    {
        public ToolbarListadosModel()
        {
            Operacion=TipoOperacion.Custom;
        }

        public override string GetCustomTexto()
        {
            return General.LblListar;
        }
    }

    #region static y Dictionary
      
    public class FListadosModel
    {
        private Dictionary<string, Type> _dictionary =new Dictionary<string, Type>();
        private Dictionary<string, string> _dictionaryController = new Dictionary<string, string>();

        private static FListadosModel _instance;

        #region const
        public const string Terceros = "TercerosGeneral";
        public const string PresupuestosCabecera = "PresupuestosCabecera";
        public const string PresupuestosCabeceraAgrupado = "PresupuestosCabeceraAgrupado";
        public const string PresupuestosDetallado = "PresupuestosDetallado";
        public const string PedidosCabecera = "PedidosCabecera";
        public const string PedidosCabeceraAgrupado = "PedidosCabeceraAgrupado";
        public const string PedidosDetallado = "PedidosDetallado";
        public const string AlbaranesCabecera = "AlbaranesCabecera";
        public const string AlbaranesCabeceraAgrupado = "AlbaranesCabeceraAgrupado";
        public const string AlbaranesDetallado = "AlbaranesDetallado";
        public const string InformeMargen = "InformeMargen";

        public const string FacturasCabecera = "FacturasCabecera";
        public const string FacturasCabeceraAgrupado = "FacturasCabeceraAgrupado";
        public const string FacturasDetallado = "FacturasDetallado";


        //compras
        public const string PresupuestosComprasCabecera = "PresupuestosComprasCabecera";
        public const string PresupuestosComprasCabeceraAgrupado = "PresupuestosComprasCabeceraAgrupado";
        public const string PresupuestosComprasDetallado = "PresupuestosComprasDetallado";

        public const string PedidosComprasCabecera = "PedidosComprasCabecera";
        public const string PedidosComprasCabeceraAgrupado = "PedidosComprasCabeceraAgrupado";
        public const string PedidosComprasDetallado = "PedidosComprasDetallado";

        public const string FacturasComprasCabecera = "FacturasComprasCabecera";
        public const string FacturasComprasCabeceraAgrupado = "FacturasComprasCabeceraAgrupado";
        public const string FacturasComprasDetallado = "FacturasComprasDetallado";

        public const string AlbaranesComprasCabecera = "AlbaranesComprasCabecera";
        public const string AlbaranesComprasCabeceraAgrupado = "AlbaranesComprasCabeceraAgrupado";
        public const string AlbaranesComprasDetallado = "AlbaranesComprasDetallado";
        public const string StockAlbaranesCompras = "StockAlbaranesCompras";

        public const string ProductosEnPresupuestos = "ProductosEnPresupuestos";
        public const string ProductosEnPedidos = "ProductosEnPedidos";
        public const string ProductosEnAlbaranes = "ProductosEnAlbaranes";
        public const string ProductosEnFacturas = "ProductosEnFacturas";
        public const string ProductosEnAlbaranesCompras = "ProductosEnAlbaranesCompras";
        public const string ProductosEnPresupuestosCompras = "ProductosEnPresupuestosCompras";
        public const string ProductosEnPedidosCompras = "ProductosEnPedidosCompras";
        public const string ProductosEnFacturasCompras = "ProductosEnFacturasCompras";

        public const string ListadoBundles = "ListadosBundles";
        public const string ListadoKits = "ListadosKits";

        public const string ListadoReservas = "ListadosReservas";
        //stock
        public const string StockGeneral = "General";
        public const string StockAgrupadoArticulo = "StockAgrupadoArticulo";
        public const string StockAgrupadoArticuloLote = "StockAgrupadoArticuloLote";
        public const string StockAgrupadoArticuloLoteMedidas = "StockAgrupadoArticuloLoteMedidas";
        public const string StockAgrupadoArticuloMedidas = "StockAgrupadoArticuloMedidas";
        public const string StockAgrupadoDisponibleArticulo = "StockAgrupadoDisponibleArticulo";
        public const string StockSalidas = "Salidas";

        //stock valorado
        public const string StockGeneralValorado = "GeneralValorado";
        public const string StockAgrupadoArticuloValorado = "StockAgrupadoArticuloValorado";
        public const string StockAgrupadoArticuloLoteValorado = "StockAgrupadoArticuloLoteValorado";
        public const string StockAgrupadoArticuloLoteMedidasValorado = "StockAgrupadoArticuloLoteMedidasValorado";
        public const string StockAgrupadoArticuloMedidasValorado = "StockAgrupadoArticuloMedidasValorado";
        public const string StockAgrupadoDisponibleArticuloValorado = "StockAgrupadoDisponibleArticuloValorado";

        public const string Articulos = "Articulos";

        public const string Comisiones = "Comisiones";

        public const string InformeLotes = "Informelotes";
        
        // Contabilidad
        public const string DiarioContable = "DiarioContable";
        public const string Mayor = "Mayor";
        public const string SumasYSaldos = "SumasYSaldos";
        //public const string Inmovilizado = "Inmovilizado";
        //public const string IvaSoprtado = "IVA Soportado";
        //public const string IvaRepercutido = "IVA Repercutido";
        //public const string PrevisionCobros = "Previsión cobros";
        //public const string PrevisionPagos = "Previsión pagos";
        public const string ListadoGuiasBalances = "GuiasBalances";
        public const string ListadoGuiasBalancesLineas = "GuiasBalancesLineas";
        public const string ListadoAcumuladorPeriodos = "AcumuladorPeriodos";
        public const string ConsultaTesoreria = "ConsultaTesoreria";

        // CRM
        public const string ListadoCrm = "ListadoCrm";

        #endregion const

        private FListadosModel()
        {
            #region _dictionary
            _dictionary.Add(Terceros, typeof(ListadosTerceros));
            _dictionary.Add(PresupuestosCabecera, typeof(ListadosPresupuestos));
            _dictionary.Add(PresupuestosCabeceraAgrupado, typeof(ListadosPresupuestos));
            _dictionary.Add(PresupuestosDetallado, typeof(ListadosDetalladoPresupuestos));
            _dictionary.Add(PedidosCabecera, typeof(ListadosPedidos));
            _dictionary.Add(PedidosDetallado, typeof(ListadosDetalladoPedidos));
            _dictionary.Add(AlbaranesCabecera, typeof(ListadosAlbaranes));
            _dictionary.Add(AlbaranesDetallado, typeof(ListadosDetalladoAlbaranes));
            _dictionary.Add(InformeMargen, typeof(ListadoInformeMargen));
            _dictionary.Add(FacturasCabecera, typeof(ListadosFacturas));
            _dictionary.Add(FacturasDetallado, typeof(ListadosDetalladoFacturas));
            _dictionary.Add(ProductosEnPresupuestos, typeof(ListadosProductosPresupuestos));
            _dictionary.Add(ProductosEnPedidos, typeof(ListadosProductosPedidos));
            _dictionary.Add(ProductosEnAlbaranes, typeof(ListadosProductosAlbaranes));
            _dictionary.Add(ProductosEnFacturas, typeof(ListadosProductosFacturas));
            _dictionary.Add(Articulos, typeof(ListadosArticulos));
            _dictionary.Add(Comisiones, typeof(ListadosComisiones));

            _dictionary.Add(PresupuestosComprasCabecera, typeof(ListadosPresupuestosCompras));
            _dictionary.Add(PresupuestosComprasCabeceraAgrupado, typeof(ListadosPresupuestosCompras));
            _dictionary.Add(PresupuestosComprasDetallado, typeof(ListadosDetalladoPresupuestosCompras));
            _dictionary.Add(ProductosEnPresupuestosCompras, typeof(ListadosProductosPresupuestosCompras));

            _dictionary.Add(PedidosComprasCabecera, typeof(ListadosPedidosCompras));
            _dictionary.Add(PedidosComprasCabeceraAgrupado, typeof(ListadosPedidosCompras));
            _dictionary.Add(PedidosComprasDetallado, typeof(ListadosDetalladoPedidosCompras));
            _dictionary.Add(ProductosEnPedidosCompras, typeof(ListadosProductosPedidosCompras));

            _dictionary.Add(AlbaranesComprasCabecera, typeof(ListadosAlbaranesCompras));
            _dictionary.Add(AlbaranesComprasDetallado, typeof(ListadosDetalladoAlbaranesCompras));
            _dictionary.Add(StockAlbaranesCompras, typeof(ListadosStockAlbaranesCompras));
            _dictionary.Add(ProductosEnAlbaranesCompras, typeof(ListadosProductosAlbaranesCompras));


            _dictionary.Add(FacturasComprasCabecera, typeof(ListadosFacturasCompras));
            _dictionary.Add(FacturasComprasDetallado, typeof(ListadosDetalladoFacturasCompras));
            _dictionary.Add(ProductosEnFacturasCompras, typeof(ListadosProductosFacturasCompras));

            _dictionary.Add(ListadoBundles, typeof(ListadosBundles));
            _dictionary.Add(ListadoKits, typeof(ListadosKits));
            _dictionary.Add(ListadoReservas, typeof(ListadosReservas));
            _dictionary.Add(StockGeneral, typeof(ListadosStock));            
            _dictionary.Add(StockAgrupadoDisponibleArticulo, typeof(ListadosStockDisponible));
            _dictionary.Add(StockGeneralValorado, typeof(ListadosStockValorado));
            _dictionary.Add(StockSalidas, typeof(ListadosStockSalidas));

            _dictionary.Add(InformeLotes, typeof(ListadosLotes));

            // Contabilidad
            _dictionary.Add(DiarioContable, typeof(ListadosDiarioContable));
            _dictionary.Add(Mayor, typeof(ListadosMayor));
            _dictionary.Add(SumasYSaldos, typeof(ListadosBalanceSumasYSaldos));
            //_dictionary.Add(ListadoGuiasBalances, typeof(ListadoGuiasBalances));
            //_dictionary.Add(ListadoGuiasBalancesLineas, typeof(ListadoGuiasBalancesLineas));
            _dictionary.Add(ListadoAcumuladorPeriodos, typeof(ListadoAcumuladorPeriodos));
            _dictionary.Add(ConsultaTesoreria, typeof(ListadosConsultaTesoreria));

            // CRM
            _dictionary.Add(ListadoCrm, typeof(ListadoCrm));

            #endregion _dictionary

            #region _dictionaryController
            _dictionaryController.Add(Terceros, "ListadosTerceros");
            _dictionaryController.Add(PresupuestosCabecera, "ListadosPresupuestos");
            _dictionaryController.Add(PresupuestosCabeceraAgrupado, "ListadosPresupuestos");
            _dictionaryController.Add(PresupuestosDetallado, "ListadoDetalladoPresupuestos");
            _dictionaryController.Add(PedidosCabecera, "ListadosPedidos");
            _dictionaryController.Add(PedidosDetallado, "ListadoDetalladoPedidos");
            _dictionaryController.Add(AlbaranesCabecera, "ListadosAlbaranes");
            _dictionaryController.Add(AlbaranesDetallado, "ListadoDetalladoAlbaranes");
            _dictionaryController.Add(FacturasCabecera, "ListadosFacturas");
            _dictionaryController.Add(FacturasDetallado, "ListadoDetalladoFacturas");
            _dictionaryController.Add(ProductosEnPresupuestos, "ListadoProductosPresupuestos");
            _dictionaryController.Add(ProductosEnPedidos, "ListadoProductosPedidos");
            _dictionaryController.Add(ProductosEnAlbaranes, "ListadoProductosAlbaranes");
            _dictionaryController.Add(ProductosEnFacturas, "ListadoProductosFacturas");
            _dictionaryController.Add(Articulos, "ListadoArticulos");
            _dictionaryController.Add(Comisiones, "ListadosComisiones");

            _dictionaryController.Add(PresupuestosComprasCabecera, "ListadosPresupuestosCompras");
            _dictionaryController.Add(PresupuestosComprasCabeceraAgrupado, "ListadosPresupuestosCompras");
            _dictionaryController.Add(PresupuestosComprasDetallado, "ListadoDetalladoPedidosCompras");
            _dictionaryController.Add(ProductosEnPresupuestosCompras, "ListadoProductosPresupuestosCompras");

            _dictionaryController.Add(PedidosComprasCabecera, "ListadosPedidosCompras");
            _dictionaryController.Add(PedidosComprasCabeceraAgrupado, "ListadosPedidosCompras");
            _dictionaryController.Add(PedidosComprasDetallado, "ListadoDetalladoPedidosCompras");
            _dictionaryController.Add(ProductosEnPedidosCompras, "ListadoProductosPedidosCompras");

            _dictionaryController.Add(AlbaranesComprasCabecera, "ListadosAlbaranesCompras");
            _dictionaryController.Add(AlbaranesComprasDetallado, "ListadoDetalladoAlbaranesCompras");
            _dictionaryController.Add(StockAlbaranesCompras, "ListadoStockAlbaranesCompras");
            _dictionaryController.Add(ProductosEnAlbaranesCompras, "ListadoProductosAlbaranesCompras");


            _dictionaryController.Add(FacturasComprasCabecera, "ListadosFacturasCompras");
            _dictionaryController.Add(FacturasComprasDetallado, "ListadoDetalladoFacturasCompras");
            _dictionaryController.Add(ProductosEnFacturasCompras, "ListadoProductosFacturasCompras");
            _dictionaryController.Add(ListadoBundles, "ListadoBundles");
            _dictionaryController.Add(ListadoKits, "ListadoKits");
            _dictionaryController.Add(ListadoReservas, "ListadoReservas");
            _dictionaryController.Add(StockGeneral, "ListadosStock");            
            _dictionaryController.Add(StockAgrupadoDisponibleArticulo, "ListadosStockDisponible");
            _dictionaryController.Add(StockGeneralValorado, "ListadosStockValorado");
            _dictionaryController.Add(StockSalidas, "ListadosStockSalidas");
            
            _dictionaryController.Add(InformeLotes, "ListadosLotes");

            // Contabilidad
            _dictionaryController.Add(DiarioContable, "ListadosDiarioContable");
            _dictionaryController.Add(Mayor, "ListadosMayor");
            _dictionaryController.Add(SumasYSaldos, "ListadosBalanceSumasYSaldos");
            //_dictionaryController.Add(ListadoGuiasBalances, "GuiasBalances");
            _dictionaryController.Add(ListadoAcumuladorPeriodos, "ListadoAcumuladorPeriodos");
            _dictionaryController.Add(ConsultaTesoreria, "ListadosConsultaTesoreria");

            // CRM
            _dictionaryController.Add(ListadoCrm, "ListadoCrm");

            #endregion _dictionaryController
        }
        #endregion static y Dictionary
        

        public static FListadosModel Instance
        {
            get
            {
                if(_instance==null)
                    _instance=new FListadosModel();
                return _instance;
            }
        }

     
        public IListados GetModel(IContextService context,string name,string empresa,string ejercicio)
        {
            var type = _dictionary[name];
            var ctor = type.GetConstructor(new[] { typeof(IContextService)});
            var result = ctor.Invoke(new object[] { context}) as IListados;
            
            result.Empresa = empresa;
            
            result.AppService = new ApplicationHelper(context);
            CalcularFechaDesdeFechaHasta(context,result, empresa,ejercicio);

            return result;
        }

        public string GetController(string id)
        {
            return _dictionaryController[id];
        }

        private void CalcularFechaDesdeFechaHasta(IContextService context,IListados objeto,string empresa,string ejercicio)
        {
            if (!string.IsNullOrEmpty(ejercicio))
            {
                var properties = objeto.GetType().GetProperties();
                EjerciciosModel obj=null;
                if (properties.Any(f=>f.Name== "FechaDesde") || properties.Any(f => f.Name == "FechaHasta"))
                {
                    using (var ejercicioservice = FService.Instance.GetService(typeof(EjerciciosModel), context))
                    {
                        obj = ejercicioservice.get(ejercicio) as EjerciciosModel;
                    }
                }
                if (obj != null)
                {
                    if (properties.Any(f => f.Name == "FechaDesde"))
                    {
                        objeto.GetType().GetProperty("FechaDesde").SetValue(objeto, obj.Desde);
                    }

                    if (properties.Any(f => f.Name == "FechaHasta"))
                    {
                        objeto.GetType().GetProperty("FechaHasta").SetValue(objeto, obj.Hasta);
                    }

                }


            }
        }
    }
}
