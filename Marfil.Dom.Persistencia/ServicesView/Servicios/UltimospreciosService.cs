using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class UltimospreciosService : IDisposable
    {
        #region Members

        private readonly MarfilEntities _db;

        #endregion

        #region Properties

        private string _empresa;
        public string Empresa
        {
            get { return _empresa; }
            set
            {
                _empresa = value;
            }
        }

        #endregion

        #region CTR

        public UltimospreciosService(IContextService context)
        {

            if (context.IsAuthenticated())
            {

                _empresa = context.Empresa;
                _db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);

            }


        }

        #endregion

        #region API

        public IEnumerable<UltimoprecioEspecificoModel> GetUltimosPrecios(string articulo, string cuenta, TipoDocumentos tipo)
        {
            //ultimos precios presupuestos
            var result = new List<UltimoprecioEspecificoModel>();
            using (var connection = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(CreateQuery(articulo, cuenta, tipo), connection))
                {
                    cmd.Parameters.Add(new SqlParameter("empresa", Empresa));
                    cmd.Parameters.Add(new SqlParameter("articulo", articulo));
                    cmd.Parameters.Add(new SqlParameter("cuenta", cuenta));
                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        using (var tabla = new DataTable())
                        {
                            ad.Fill(tabla);
                            foreach (DataRow row in tabla.Rows)
                            {
                                result.Add(new UltimoprecioEspecificoModel()
                                {
                                    Referenciadocumento = Funciones.Qnull(row["Referencia"]),
                                    Cantidad = Funciones.Qdouble(row["Cantidad"]),
                                    Precio = Funciones.Qdouble(row["Precio"]),
                                    DtoCial = Funciones.Qdouble(row["DtoCial"]),
                                    DtoLin = Funciones.Qdouble(row["DtoLin"]),
                                    DtoPP = Funciones.Qdouble(row["DtoPP"]),
                                    Fecha = Funciones.Qdate(row["Fecha"])?.ToShortDateString().ToString(CultureInfo.CurrentUICulture),
                                    Metros = Funciones.Qdouble(row["Metros"]),
                                    Moneda = Funciones.Qnull(row["Moneda"])
                                });
                            }
                        }


                    }
                }
            }

            return result;
        }

        private string CreateQuery(string articulo, string cuenta, TipoDocumentos tipo)
        {
            var sb = new StringBuilder();
            var tablacabecera = "";
            var tablalineas = "";
            var columnaclaveajena = "";
            var tercero = "clientes";
            switch (tipo)
            {
                case TipoDocumentos.PresupuestosVentas:
                    tablacabecera = "presupuestos";
                    tablalineas = "presupuestoslin";
                    columnaclaveajena = tablacabecera;
                    break;
                case TipoDocumentos.PedidosVentas:
                    tablacabecera = "pedidos";
                    tablalineas = "pedidoslin";
                    columnaclaveajena = tablacabecera;
                    break;
                case TipoDocumentos.AlbaranesVentas:
                    tablacabecera = "albaranes";
                    tablalineas = "albaraneslin";
                    columnaclaveajena = tablacabecera;
                    break;
                case TipoDocumentos.FacturasVentas:
                    tablacabecera = "facturas";
                    tablalineas = "facturaslin";
                    columnaclaveajena = tablacabecera;
                    break;
                case TipoDocumentos.AlbaranesCompras:
                    tablacabecera = "albaranescompras";
                    tablalineas = "albaranescompraslin";
                    columnaclaveajena = "albaranes";
                    tercero = "proveedores";
                    break;
                case TipoDocumentos.PresupuestosCompras:
                    tablacabecera = "presupuestoscompras";
                    tablalineas = "presupuestoscompraslin";
                    columnaclaveajena = "presupuestoscompras";
                    tercero = "proveedores";
                    break;
                case TipoDocumentos.PedidosCompras:
                    tablacabecera = "pedidoscompras";
                    tablalineas = "pedidoscompraslin";
                    columnaclaveajena = "pedidoscompras";
                    tercero = "proveedores";
                    break;
            }

            sb.AppendLine(string.Format("Select p.referencia as [Referencia],Sum(pl.cantidad) as [Cantidad],pl.precio as [Precio],p.porcentajedescuentocomercial as [DtoCial],pl.porcentajedescuento as [DtoLin],p.porcentajedescuentoprontopago as [DtoPP],p.fechadocumento as [Fecha],Sum(pl.Metros) as [Metros], m.descripcion as [Moneda]  from {0} as pl", tablalineas));
            sb.AppendLine(string.Format(" inner join {0} as p on p.empresa=pl.empresa and p.id=pl.fk{1} and pl.empresa=@empresa and p.fk{2}=@cuenta and pl.fkarticulos=@articulo ", tablacabecera, columnaclaveajena, tercero));
            sb.AppendLine(" left join monedas as m on m.id= p.fkmonedas ");
            sb.AppendLine(" group  by p.referencia,p.porcentajedescuentocomercial,p.porcentajedescuentoprontopago,p.fechadocumento,m.descripcion,pl.precio,pl.porcentajedescuento ");


            return sb.ToString();
        }

        #endregion

        public void Dispose()
        {
            _db?.Dispose();
        }

        public IEnumerable<UltimopreciosistemaModel> GetPreciosSistema(string articulo, TipoFlujo tipo, string cuenta)
        {
            // var list = _appService.GetTarifasSistema(tipo, true, Empresa);
            var existing = _db.TarifasLin.Include("Tarifas").Where(f => f.empresa == Empresa && f.fkarticulos == articulo && f.Tarifas.tipotarifa == (int)TipoTarifa.Sistema && f.Tarifas.tipoflujo == (int)tipo).ToList();

            if (tipo == TipoFlujo.Venta)
            {
                var listaEspeciales = GetPreciosEspeciales(articulo, cuenta);

                var listaVenta =
                    existing.Select(
                        f =>
                            new UltimopreciosistemaModel()
                            {
                                Tarifa = f.Tarifas.descripcion,
                                Precio = f.precio ?? 0
                            }).ToList();

                listaVenta.AddRange(listaEspeciales);

                return listaVenta;
            }
            else
            {
                return
                    existing.Select(
                        f =>
                            new UltimopreciosistemaModel()
                            {
                                Tarifa = f.Tarifas.descripcion,
                                Precio = f.precio ?? 0
                            }).ToList();
            }


        }

        public List<UltimopreciosistemaModel> GetPreciosEspeciales(string articulo, string cuenta)
        {
            // var list = _appService.GetTarifasSistema(tipo, true, Empresa);
            var existing = _db.PreciosEspeciales.Where(f => f.empresa == Empresa && f.fkarticulo == articulo && f.fkclientes == cuenta && f.fechavalidez >= DateTime.Today).ToList();
            return
                existing.Select(
                    f =>
                        new UltimopreciosistemaModel()
                        {
                            Tarifa = "Precio especial",
                            Precio = f.precio
                        }).ToList();
        }
    }
}
