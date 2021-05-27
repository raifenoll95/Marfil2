using System;
using System.Collections.Generic;
using Marfil.Dom.ControlsUI.CampoVerificacion;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Campoverificacion
{
    public class FCampoverificacion
    {
        #region Singleton

        
        public static FCampoverificacion Instance
        {
            get {
                return new FCampoverificacion();
                
            }
        }

        #endregion

        #region Members

        private readonly Dictionary<Type, CampoverificacionModel> _dictionary; 
        #endregion

        #region CTR

        private FCampoverificacion()
        {
            _dictionary=new Dictionary<Type, CampoverificacionModel>();

          
        }

        #endregion

        public CampoverificacionModel GetModel<T>(IContextService context)
        {
            var appService=new ApplicationHelper(context);
            if (typeof(T) == typeof(TrabajosModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Trabajos.TituloEntidadSingular,
                Longitud = "5",
                Tipo = "0"

            };
            if (typeof(T) == typeof(StockActualModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Lote",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Stock.TituloEntidadSingular,
                Longitud = "0",
                Tipo = ""

            };
            if (typeof(T) == typeof(ObrasModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Nombreobra",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Obras.TituloEntidad,
                Longitud = "0",
                Tipo = ""

            };
            if (typeof(T) == typeof(AlmacenesModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Almacenes.TituloEntidad,
                Longitud = "4",
                Tipo = "0"

            };
            if (typeof(T) == typeof(IncidenciasModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Incidencias.TituloEntidad,
                Longitud = "3",
                Tipo = "0"

            };
            if (typeof(T) == typeof(AlmacenesZonasModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Almacenes.Zonas,
                Longitud = "0",
                Tipo = ""

            };

            if (typeof(T) == typeof(BancosModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Nombre",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Bancos.TituloEntidad,
                Longitud = "4",
                Tipo = "2"

            };

            if (typeof(T)==typeof(BancosMandatosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.BancosMandatos.TituloEntidad,
                Longitud = "3",
                Tipo = "2"

            };

            if(typeof(T)==typeof(ArticulosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcionabreviada",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Articulos.TituloEntidad,
                Longitud = "15",
                Tipo = "2"

           };

            if(typeof(T)==typeof(EjerciciosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcioncorta",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Ejercicios.TituloEntidad,
                Longitud = "0",
                Tipo = ""

           };


            if(typeof(T)==typeof(PresupuestosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Referencia",
                CampoDescripcion = "",
                Configuracion = new ControlsUI.CampoVerificacion.Configuracion() { OcultarTexto = true },
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.TituloEntidad,
                Longitud = "30",
                Tipo = "2"

           };

            if(typeof(T)==typeof(PedidosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Referencia",
                CampoDescripcion = "",
                Configuracion = new ControlsUI.CampoVerificacion.Configuracion() { OcultarTexto = true },
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Pedidos.TituloEntidad,
                Longitud = "30",
                Tipo = "2"

           };
            if (typeof(T) == typeof(PedidosComprasModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Referencia",
                CampoDescripcion = "",
                Configuracion = new ControlsUI.CampoVerificacion.Configuracion() { OcultarTexto = true },
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras.TituloEntidad,
                Longitud = "30",
                Tipo = "2"

            };
            if (typeof(T)==typeof(SeriesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Series.TituloEntidadSingular,
                Longitud = "3",
                Tipo = "2"

           };

            if (typeof(T) == typeof(SeriesContablesModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.SeriesContables.TituloEntidadSingular,
                Longitud = "3",
                Tipo = "2"

            };
            if (typeof(T)==typeof(ContadoresModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Contadores.TituloEntidad,
                Longitud = "12",
                Tipo = "2"

           };

            if(typeof(T)==typeof(FamiliasproductosModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos.TituloEntidad,
                Longitud = "2",
                Tipo = "0"

           };

            if(typeof(T)==typeof(MaterialesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Materiales.TituloEntidad,
                Longitud = "3",
                Tipo = "0"

           };

            if(typeof(T)==typeof(CaracteristicasModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Caracteristicas.TituloEntidad,
                Longitud = "2",
                Tipo = "0"

           };

            if(typeof(T)==typeof(GrosoresModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Grosores.TituloEntidad,
                Longitud = "2",
                Tipo = "0"

           };

            if(typeof(T)==typeof(AcabadosModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Acabados.TituloEntidad,
                Longitud = "2",
                Tipo = "0"
           };

            if (typeof(T) == typeof(SituacionesTesoreriaModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Cod",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.SituacionesTesoreria.TituloEntidad,
                Longitud = "1",
                Tipo = "0"
            };

            if (typeof(T) == typeof(CircuitoTesoreriaCobrosModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Circuitos de tesorería",
                Longitud = "0",
                Tipo = ""
            };

            if (typeof(T)==typeof(CuentasModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Cuentas",
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"

            };

            if (typeof(T) == typeof(ContactosModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Nombre",
                Titulo = "Contactos",
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"
                     
            };

            if (typeof(T)==typeof(AgentesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Agentes.TituloEntidad,
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"

            };

            if(typeof(T)==typeof(ComercialesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Comerciales.TituloEntidad,
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"

            };

            if(typeof(T)==typeof(AcreedoresModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Acreedores.TituloEntidad,
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"

            };

            if(typeof(T)==typeof(AseguradorasModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = Inf.ResourcesGlobalization.Textos.Entidades.Aseguradoras.TituloEntidad,
                Longitud = appService.DigitosCuentas().ToString(),
                Tipo = "0",
                Columnabloqueados = "Bloqueado"

            };

            if(typeof(T)==typeof(RegimenIvaModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Régimen de IVA",
                Longitud = "5",
                Tipo = "0"

           };

            if(typeof(T)==typeof(GruposIvaModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Grupos de IVA",
                Longitud = "4",
                Tipo = "0"

           };

            if (typeof(T) == typeof(GrupoMaterialesModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Cod",
                CampoDescripcion = "Descripcion",
                Titulo = "Grupos de materiales",
                Longitud = "3",
                Tipo = "0"

            };

            if (typeof(T)==typeof(TiposRetencionesModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Tipos de retención",
                Longitud = "4",
                Tipo = "0"
           };

            if(typeof(T)==typeof(FormasPagoModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Nombre",
                Titulo = "Formas de pago",
                Longitud = "0",
                Tipo = ""
           };

            if(typeof(T)==typeof(MonedasModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Monedas",
                Longitud = "0",
                Tipo = ""
           };

            if(typeof(T)==typeof(GuiascontablesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Guías contables",
                Longitud = "12",
                Tipo = "2"

           };

            if(typeof(T)==typeof(UnidadesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Unidades medidas",
                Longitud = "2",
                Tipo = "0"

           };

            if(typeof(T)==typeof(DireccionesModel )) return new CampoverificacionModel()
            {
                CampoIdentificador = "Id",
                CampoDescripcion = "Descripcion",
                Titulo = "Direcciones",
                Longitud = "0",
                Tipo = ""
           };

            if (typeof(T) == typeof(FacturasModel)) return new CampoverificacionModel()
            {
                CampoIdentificador = "Referencia",
                CampoDescripcion = "Referencia",
                Titulo = "Facturas",
                Longitud = "0",
                Tipo = ""
            };

            return new CampoverificacionModel();
        }
    }
}
