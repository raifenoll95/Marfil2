using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class DatosgeneralesaplicacionStartup:IStartup
    {
        #region Members

        private readonly GestionService<CriteriosagrupacionModel, Criteriosagrupacion> _tablasVariasService;
        private readonly ConfiguracionService _configuracionService;

        #endregion

        #region CTR

        public DatosgeneralesaplicacionStartup(IContextService context,MarfilEntities db)
        {
            _tablasVariasService = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context, db) as CriteriosagrupacionService ;
            _configuracionService = FService.Instance.GetService(typeof(ConfiguracionModel), context, db) as ConfiguracionService;
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            
            var model = new CriteriosagrupacionModel()
            {
                Id = "0001",
                Nombre = "General"
            };

            _tablasVariasService.create(model);

            //establecer los estados por defecto
            var modelconfiguracion = _configuracionService.GetModel();

            //produccion
            modelconfiguracion.Relacionbrutoneto = Math.Round(2.1,2);

            //presupuestos
            modelconfiguracion.Gestionarrevisiones = true;
            modelconfiguracion.Estadoinicial= modelconfiguracion.Estados.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoparcial = modelconfiguracion.Estados.Single(f => f.Tipoestado == TipoEstado.Curso).CampoId;
            modelconfiguracion.Estadototal = modelconfiguracion.Estados.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //pedidos 
            modelconfiguracion.Estadopedidosventasinicial = modelconfiguracion.Estadospedidosventas.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadopedidosventasparcial = modelconfiguracion.Estadospedidosventas.Single(f => f.Tipoestado == TipoEstado.Curso).CampoId;
            modelconfiguracion.Estadopedidosventastotal = modelconfiguracion.Estadospedidosventas.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;
            
            //reserva
            modelconfiguracion.Estadoreservasinicial = modelconfiguracion.Estadosreservas.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoreservasparcial = modelconfiguracion.Estadosreservas.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;
            modelconfiguracion.Estadoreservastotal = modelconfiguracion.Estadosreservas.Single(f => f.Tipoestado == TipoEstado.Caducado).CampoId;

            //albaranes
            modelconfiguracion.Estadoalbaranesventasinicial = modelconfiguracion.Estadosalbaranesventas.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoalbaranesventastotal = modelconfiguracion.Estadosalbaranesventas.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //facturas
            modelconfiguracion.Estadofacturasventasinicial = modelconfiguracion.Estadosfacturasventas.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadofacturasventastotal = modelconfiguracion.Estadosfacturasventas.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //presupuestocompra
            modelconfiguracion.Estadopresupuestoscomprasinicial = modelconfiguracion.Estadospresupuestoscompras.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadopresupuestoscomprasparcial = modelconfiguracion.Estadospresupuestoscompras.Single(f => f.Tipoestado == TipoEstado.Curso).CampoId;
            modelconfiguracion.Estadopresupuestoscomprastotal = modelconfiguracion.Estadospresupuestoscompras.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //pedidocompra
            modelconfiguracion.Estadopedidoscomprasinicial = modelconfiguracion.Estadospedidoscompras.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadopedidoscomprasparcial = modelconfiguracion.Estadospedidoscompras.Single(f => f.Tipoestado == TipoEstado.Curso).CampoId;
            modelconfiguracion.Estadopedidoscomprastotal = modelconfiguracion.Estadospedidoscompras.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //albarancompra
            modelconfiguracion.Estadoalbaranescomprasinicial = modelconfiguracion.Estadosalbaranescompras.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoalbaranescomprastotal = modelconfiguracion.Estadosalbaranescompras.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //facturascompra
            modelconfiguracion.Estadofacturascomprasinicial = modelconfiguracion.Estadosfacturascompras.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadofacturascomprastotal = modelconfiguracion.Estadosfacturascompras.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //descripcionesasientos
            modelconfiguracion.DescripcionAsientoFacturaVenta = modelconfiguracion.DescripcionesAsientos.Single(f => f.Valor == "FRC").Valor;
            modelconfiguracion.DescripcionAsientoFacturaCompra = modelconfiguracion.DescripcionesAsientos.Single(f => f.Valor == "FRP").Valor;            

            //transformación
            modelconfiguracion.Materialentradaigualsalida = true;

            //CRM
            modelconfiguracion.Estadooportunidadesinicial = modelconfiguracion.Estadosoportunidades.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadooportunidadestotal = modelconfiguracion.Estadosoportunidades.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            modelconfiguracion.Estadoproyectosinicial = modelconfiguracion.Estadosproyectos.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoproyectostotal = modelconfiguracion.Estadosproyectos.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            modelconfiguracion.Estadocampañasinicial = modelconfiguracion.Estadoscampañas.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadocampañastotal = modelconfiguracion.Estadoscampañas.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            modelconfiguracion.Estadoincidenciasinicial = modelconfiguracion.Estadosincidencias.Single(f => f.Tipoestado == TipoEstado.Diseño).CampoId;
            modelconfiguracion.Estadoincidenciastotal = modelconfiguracion.Estadosincidencias.Single(f => f.Tipoestado == TipoEstado.Finalizado).CampoId;

            //Contabilidad
            modelconfiguracion.PrefijosPrestacionServicios = "705,629,623";
            modelconfiguracion.ComentarioExistenciasIniciales = "REGUL. EXISTENCIAS INICIALES";
            modelconfiguracion.ComentarioExistenciasFinales = "EXIST. FINALES S/INVENTARIO";
            modelconfiguracion.ComentarioDebePYG = "REGULARIZAR SALDOS DEUDORES";
            modelconfiguracion.ComentarioHaberPYG = "REGULARIZAR SALDOS ACREEDORES";
            modelconfiguracion.ComentarioCuentasDetalle = "REGULARIZACION DE SALDO";
            modelconfiguracion.ComentarioCierreEjercicio = "CIERRE DEL EJERCICIO";
            modelconfiguracion.ComentarioAperturaEjercicio = "REAPERTURA DEL EJERCICIO";
            modelconfiguracion.ComentarioAperturaProvisional = "APERTURA PROVISIONAL";

            _configuracionService.CreateOrUpdate(modelconfiguracion);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
