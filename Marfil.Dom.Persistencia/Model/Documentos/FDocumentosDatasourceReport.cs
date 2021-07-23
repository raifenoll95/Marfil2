using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Bundle;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Documentos.Kit;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Helpers;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Documentos.Margen;
using Marfil.Dom.Persistencia.Model.Documentos.CarteraVencimientos;

namespace Marfil.Dom.Persistencia.Model.Documentos
{
    public class FDocumentosDatasourceReport
    {
        public static IReport CreateReport(TipoDocumentoImpresion tipo, IContextService user, string primarykey = "")
        {

            if (tipo == TipoDocumentoImpresion.PresupuestosVentas)
            {
                return new PresupuestosReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.PedidosVentas)
            {
                return new PedidosReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.AlbaranesVentas)
            {
                return new AlbaranesReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.FacturasVentas)
            {
                return new FacturasReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.PresupuestosCompras)
            {
                return new PresupuestosComprasReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.PedidosCompras)
            {
                return new PedidosComprasReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.AlbaranesCompras)
            {
                return new AlbaranesComprasReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.FacturasCompras)
            {
                return new FacturasComprasReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Kit)
            {
                return new KitReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Bundle)
            {
                return new BundleReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Reservasstock)
            {
                return new ReservasstockReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Traspasosalmacen)
            {
                return new TraspasosalmacenReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Inventarios)
            {
                return new InventariosReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Transformaciones)
            {
                return new TransformacionesReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.TransformacionesAcabados)
            {
                return new TransformacionesAcabadosReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Transformacioneslotes)
            {
                return new TransformacioneslotesReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.CarteraVencimientos)
            {
                return new CarteraVencimientosReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Remesa)
            {
                return new RemesaReport(user, primarykey);
            }
            else if (tipo == TipoDocumentoImpresion.Asientos)
            {
                Dictionary<string, object> dictionary = null;
                if (primarykey.IsValidJson())
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
                }

                return new DiarioReport(user, dictionary);
            }
            else if (tipo == TipoDocumentoImpresion.Mayor)
            {
                Dictionary<string, object> dictionary = null;
                if (primarykey.IsValidJson())
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
                }

                return new MayorReport(user, dictionary);
            }
            else if (tipo == TipoDocumentoImpresion.SumasYSaldos)
            {
                Dictionary<string, object> dictionary = null;
                if (primarykey.IsValidJson())
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
                }

                return new SumasYSaldosReport(user, dictionary);
            }
            else if (tipo == TipoDocumentoImpresion.BalancePedidos)
            {
                Dictionary<string, object> dictionary = null;
                if (primarykey.IsValidJson())
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
                }

                return new BalancePedidosPe(user, dictionary);
            }

            else if (tipo == TipoDocumentoImpresion.ListadoMargen)
            {
                Dictionary<string, object> dictionary = null;
                if (primarykey.IsValidJson())
                {
                    dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(primarykey);
                }

                return new ListadoMargenQuery(user, dictionary);
            }

            throw new NotImplementedException(string.Format("Tipo de documento {0} no implementado", tipo));
        }
    }
}
