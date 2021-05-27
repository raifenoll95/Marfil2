﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
using RProveedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RPresupuestosCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PresupuestosCompras;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosDetalladoPresupuestosCompras : ListadosModel
    {
        private List<string> _series = new List<string>();

        #region Properties

        public override string TituloListado => "Listado detallado de Presupuestos Compras";

        public override string IdListado => FListadosModel.PresupuestosComprasDetallado;

        public string Order { get; set; }

        [Display(ResourceType = typeof (RPresupuestosCompras), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "ProveedorDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "ProveedorHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkfamiliaproveedor")]
        public string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(RProveedores), Name = "Fkzonaproveedor")]
        public string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FamiliaMateriales")]
        public string Fkfamiliasmateriales{ get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FamiliaDesde")]
        public string FkfamiliasDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "FamiliaHasta")]
        public string FkfamiliasHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "MaterialesDesde")]
        public string FkmaterialesDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "MaterialesHasta")]
        public string FkmaterialesHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "CaracteristicasDesde")]
        public string FkcaracteristicasDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "CaracteristicasHasta")]
        public string FkcaracteristicasHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "GrosoresDesde")]
        public string FkgrosoresDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "GrosoresHasta")]
        public string FkgrosoresHasta { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "AcabadosDesde")]
        public string FkacabadosDesde { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "AcabadosHasta")]
        public string FkacabadosHasta { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PresupuestosCompras).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Estado")]
        public string Estado { get; set; }

        #endregion

        public ListadosDetalladoPresupuestosCompras()
        {

        }

        public ListadosDetalladoPresupuestosCompras(IContextService context) : base(context)
        {
            ConfiguracionColumnas.Add("Precio", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("% Dto.", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Subtotal", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Largo", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Ancho", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Grueso", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
               
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" p.empresa = '" + Empresa + "' ");

            if (Series?.Any() ?? false)
            {
                if (flag)
                    sb.Append(" AND ");

                foreach (var item in Series)
                    ValoresParametros.Add(item, item);

                sb.Append(" p.fkseries in ( " + string.Join(", ", Series.ToArray().Select(f => "@" + f)) + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.SeriesListado, string.Join(", ", Series.ToArray())));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Estado) && !Estado.Equals("0-"))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("estado", Estado);
                sb.Append(" p.fkestados=@estado ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.Estado, Estados?.First(f => f.CampoId == Estado).Descripcion??string.Empty));
                flag = true;
            }

            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" p.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" p.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" p.fkproveedores>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" p.fkproveedores<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" pl.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" pl.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(pl.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(pl.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(pl.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(pl.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(pl.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(pl.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(pl.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(pl.fkarticulos,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(pl.fkarticulos,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(pl.fkarticulos,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(pl.fkarticulos,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPresupuestosCompras.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }

            

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
           
                sb.Append(
                "select p.referencia as [Referencia],p.Fechadocumento as [Fecha], es.descripcion as [Estado],cu.id as Id,cu.descripcion as [Descripción],substring(pl.fkarticulos,0,3) as [Cod. Familia],fp.descripcion as [Familia]," +
                " case when fp.validarmaterial = 1 then substring(pl.fkarticulos,3,3) else '' end as [Cod. Material], case when fp.validarmaterial = 1 then m.descripcion else '' end as [Material], " +
                " case when fp.validarcaracteristica = 1 then substring(pl.fkarticulos,6,2) else '' end  as [Cod. Caracteristica], case when fp.validarcaracteristica= 1 then  cl.descripcion else '' end as [Característica], " +
                " case when fp.validargrosor = 1 then substring(pl.fkarticulos,8,2) else '' end as [Cod. Grosor], case when fp.validargrosor= 1 then  g.descripcion else ''  end as [Grosor]," +
                " case when fp.validaracabado = 1 then substring(pl.fkarticulos,10,2) else '' end as [Cod. Acabado], case when fp.validaracabado = 1 then ac.descripcion else '' end as [Acabado], " +
                " pl.fkarticulos as [Cod. Artículo], pl.descripcion as [Artículo],ud.textocorto as [UM],pl.revision as [Revision],cn.descripcion as  [Canal], pl.cantidad as [Cantidad],pl.Largo as [Largo],pl.Ancho as [Ancho],pl.Grueso as [Grueso],pl.metros as [Metros], pl.precio as [Precio],pl.porcentajedescuento as [% Dto.],pl.importe as Subtotal,mo.abreviatura as [Moneda],mo.decimales as [_decimales], ud.decimalestotales as [_decimalesunidades] from PresupuestosCompras as p " +
                " inner join PresupuestosCompraslin as pl on pl.empresa= p.empresa and pl.fkPresupuestosCompras= p.id " +
                " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0 " +
                " left join proveedores as c on c.empresa= p.empresa and p.fkproveedores= c.fkcuentas " +
                " left join direcciones as di on di.empresa=c.empresa and di.fkentidad=c.fkcuentas and di.defecto='true' " +
                " left join acreedores as ar on ar.empresa= p.empresa and p.fkproveedores= ar.fkcuentas " +
                " inner join cuentas as cu on c.empresa= cu.empresa and isnull(c.fkcuentas,ar.fkcuentas)=cu.id " +
                " left join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
                " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
                " left join caracteristicaslin as cl on cl.empresa= p.empresa and cl.fkcaracteristicas=substring(pl.fkarticulos,0,3) and cl.id=substring(pl.fkarticulos,6,2) " +
                " left join grosores as g on g.id=substring(pl.fkarticulos,8,2) " +
                " left join acabados as ac on ac.id=substring(pl.fkarticulos,10,2)" +
                " left join monedas as mo on mo.id = p.fkmonedas" +
                " left join canales as cn on cn.valor= pl.canal" +
                " left join estados as es on p.fkestados=Concat(es.documento,'-',es.id) " +
                " left join unidades as ud on ud.id= fp.fkunidadesmedida");



            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            switch (Order)
            {
                case "1":
                    return " order by p.referencia";
                case "2":
                    return " order by cu.id";
                case "3":
                    return " order by cu.descripcion";
                case "4":
                    return " order by cu.descripcion,di.fkprovincia,di.poblacion,cu.id";
                case "5":
                    return " order by p.referencia,pl.fkarticulos";
            }

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
                var service = FService.Instance.GetService(typeof(SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.PresupuestosCompras);
            }
        }
    }
}
