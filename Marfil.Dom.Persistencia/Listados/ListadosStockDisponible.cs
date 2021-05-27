using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RProveedor = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosStockDisponible : ListadosModel
    {
        #region Members

        private bool _pedidoscompraactivado=true;
        private bool _reservasactivado=true;
        private bool _pedidostalleractivado=true;

        #endregion

        #region Properties

        public override string TituloListado => Menuaplicacion.listadostockdisponible;

        public override string IdListado => FListadosModel.StockAgrupadoDisponibleArticulo;

        public string Order { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkzonas")]
        public string Fkzonasalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaMateriales")]
        public string Fkfamiliasmateriales { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaDesde")]
        public string FkfamiliasDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaHasta")]
        public string FkfamiliasHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesDesde")]
        public string FkmaterialesDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesHasta")]
        public string FkmaterialesHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "CaracteristicasDesde")]
        public string FkcaracteristicasDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "CaracteristicasHasta")]
        public string FkcaracteristicasHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "GrosoresDesde")]
        public string FkgrosoresDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "GrosoresHasta")]
        public string FkgrosoresHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AcabadosDesde")]
        public string FkacabadosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AcabadosHasta")]
        public string FkacabadosHasta { get; set; }


        public bool Pedidostalleractivado
        {
            get { return _pedidostalleractivado; }
            set { _pedidostalleractivado = value; }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public List<EstadosModel> Estadosdescontarpedidostaller
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PedidosVentas).Where(f => f.Tipoestado < TipoEstado.Finalizado).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Descontarpedidostaller")]
        public string Estadodescontarpedidostaller { get; set; }

        public bool Reservasactivado
        {
            get { return _reservasactivado; }
            set { _reservasactivado = value; }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public List<EstadosModel> Estadosdescontarreservasstock
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.Reservasstock).Where(f => f.Tipoestado < TipoEstado.Finalizado).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Descontarreservasstock")]
        public string Estadodescontarreservasstock { get; set; }

        public bool Pedidoscompraactivado
        {
            get { return _pedidoscompraactivado; }
            set { _pedidoscompraactivado = value; }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public List<EstadosModel> Estadosañadirpedidoscompra
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PedidosCompras).Where(f=>f.Tipoestado<TipoEstado.Finalizado).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Añadirpedidoscompra")]
        public string Estadoañadirpedidoscompra { get; set; }


        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }


        #endregion

        public ListadosStockDisponible()
        {
            ConfiguracionColumnas.Add("Metros Almacén", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Ped.Venta", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Reservas", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Ped. Compras", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Disponibles", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
        }

        public ListadosStockDisponible(IContextService context) : base(context)
        {
            ConfiguracionColumnas.Add("Metros Almacén", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Ped.Venta", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Reservas", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Ped. Compras", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros Disponibles", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });


        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.AppendFormat(" s.empresa='{0}' ", Empresa);
            if (!string.IsNullOrEmpty(Fkalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkalmacen", Fkalmacen);
                sb.Append(" s.fkalmacenes = @fkalmacen  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkalmacen, Fkalmacen));
                flag = true;
            }


            if (!string.IsNullOrEmpty(Fkzonasalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonasalmacen", Fkzonasalmacen);
                sb.Append(" s.fkalmaceneszona = @fkzonasalmacen  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkzonas, Fkzonasalmacen));
                flag = true;
            }


            if (Tipodealmacenlote.HasValue)
            {
                int index = Array.IndexOf(Enum.GetValues(Tipodealmacenlote?.GetType()), Tipodealmacenlote);
                string Tipoalmacenlote = index.ToString();
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("tipoalmacenlote", Tipoalmacenlote);
                sb.Append(" s.tipoalmacenlote = @tipoalmacenlote  ");

                switch (Tipodealmacenlote)
                {
                    case TipoAlmacenlote.Mercaderia:
                        {
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLoteMercaderia;
                            break;
                        }
                    case TipoAlmacenlote.Gestionado:
                        {
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLoteGestionado;
                            break;
                        }
                    case TipoAlmacenlote.Propio:
                        {
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLotePropio;
                            break;
                        }
                }
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranesCompras.TipoAlmacenLote, Tipoalmacenlote));
                flag = true;
            }




            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" s.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" s.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                var appService=new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(s.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, appService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(s.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(s.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(s.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(s.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(s.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(s.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(s.fkarticulos,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(s.fkarticulos,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(s.fkarticulos,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(s.fkarticulos,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }

          

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select  s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],u.codigounidad as [UM], sum( s.cantidadtotal) as [Cant. Almacén],sum(s.metros) as [Metros Almacén],r1.cantidad as [Cantidad Ped. Venta],r1.metros as [Metros Ped.Venta],sum(rl.cantidad) as [Cantidad Reservas],sum(rl.metros) as [Metros Reservas],r2.cantidad as [Cantidad Ped. Compras],r2.metros as [Metros Ped. Compras],((isnull(sum(s.cantidadtotal),0) + {4} - ({3} + {5}))) as [Piezas disponibles], ((isnull(sum(s.metros),0) + {1} - ({0} + {2}))) as [Metros Disponibles],u.decimalestotales as [_decimales] from stockactual as s ", Pedidostalleractivado ? "isnull(r1.metros,0)" : "0",Pedidoscompraactivado ? " isnull(r2.metros,0)" : "0",Reservasactivado ? "isnull(sum(rl.metros),0)" : "0", Pedidostalleractivado ? "isnull(r1.cantidad,0)" : "0", Pedidoscompraactivado ? " isnull(r2.cantidad,0)" : "0", Reservasactivado ? "isnull(sum(rl.cantidad),0)" : "0");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
            sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");
            sb.AppendFormat(" left join (select pvl.fkarticulos, sum(pvl.cantidad)  as cantidad,sum(pvl.metros) as metros from pedidoslin as pvl  inner join pedidos as pv on pv.empresa= '0001' and pv.id=pvl.fkpedidos inner join estados as ep on concat(ep.documento,'-',ep.id) = pv.fkestados and ep.tipoestado <2 group by pvl.fkarticulos)as r1 on r1.fkarticulos=s.fkarticulos ");
            sb.AppendFormat(" left join (select pvl.fkarticulos, sum(pvl.cantidad)  as cantidad,sum(pvl.metros) as metros from pedidoscompraslin as pvl  inner join pedidoscompras as pv on pv.empresa= '0001' and pv.id=pvl.fkpedidoscompras inner join estados as ep on concat(ep.documento,'-',ep.id) = pv.fkestados and ep.tipoestado <2 group by pvl.fkarticulos)as r2 on r2.fkarticulos=s.fkarticulos ");
            sb.AppendFormat(" left join (reservasstocklin as rl   inner join reservasstock as r on r.empresa= '{0}'  and r.id=rl.fkreservasstock  inner join estados as er on concat(er.documento,'-',er.id) = r.fkestados and er.tipoestado <{2}  {3}) on s.empresa =rl.empresa  and s.fkarticulos=rl.fkarticulos and s.lote= rl.lote and s.loteid=rl.tabla ", Context.Empresa, (int)TipoDocumento.Reservas, (int)TipoEstado.Finalizado, GenerarFiltrosReservas());
            
            return sb.ToString();
        }

        private string GenerarFiltrosPedidosVentas()
        {
            var sb=new StringBuilder();
            if (Estadodescontarpedidostaller != "0-")
            {
                
                sb.Append(" AND ");

                ValoresParametros.Add("estadopedidostaller", Estadodescontarpedidostaller);
                sb.Append(" concat(ep.documento,'-',ep.id) = @estadopedidostaller  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Descontarpedidostaller, Estadosdescontarpedidostaller.Single(f=>f.CampoId== Estadodescontarpedidostaller).Descripcion));
                
            }


            return sb.ToString();
        }

        private string GenerarFiltrosReservas()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Estadodescontarreservasstock) && Estadodescontarreservasstock != "0-")
            {

                sb.Append(" AND ");

                ValoresParametros.Add("estadoreservas", Estadodescontarreservasstock);
                sb.Append(" concat(er.documento,'-',er.id) = @estadoreservas  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Descontarreservasstock, Estadosdescontarreservasstock.Single(f => f.CampoId == Estadodescontarreservasstock).Descripcion));

            }


            return sb.ToString();
        }

        private string GenerarFiltrosPedidosCompras()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Estadoañadirpedidoscompra) && Estadoañadirpedidoscompra!="0-")
            {

                sb.Append(" AND ");

                ValoresParametros.Add("estadopedidoscompra", Estadoañadirpedidoscompra);
                sb.Append(" concat(ec.documento,'-',ec.id) = @estadopedidoscompra  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Añadirpedidoscompra, Estadosañadirpedidoscompra.Single(f => f.CampoId == Estadoañadirpedidoscompra).Descripcion));

            }


            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = " group by s.fkarticulos,a.descripcion,u.codigounidad,u.decimalestotales ,r1.cantidad,r1.metros,r2.cantidad,r2.metros";
            return result;
        }

        #region Helper





        #endregion
    }
}
