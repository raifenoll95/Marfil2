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
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos.Helper;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosBundles : ListadosModel
    {
      

        #region Properties

        public override string TituloListado => "Listado de Bundles";

        public override string IdListado => FListadosModel.ListadoBundles;

        [Display(ResourceType = typeof(RKit), Name = "Estado")]
        public EstadoKit? Estado { get; set; }

        public string Agrupacion { get; set; }

        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

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


        [Display(ResourceType = typeof(RAlbaranes), Name = "FkzonasDesde")]
        public string FkzonaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkzonasHasta")]
        public string FkzonaHasta { get; set; }

        #endregion

        public ListadosBundles()
        {
            Agrupacion = "1";
        }

        public ListadosBundles(IContextService context):base(context)
        {
            Agrupacion = "1";
            Fkalmacen = context.Fkalmacen;
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" p.empresa = '" + Context.Empresa + "' ");

            if (Estado.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("estado", (int)Estado.Value);
                sb.Append(" p.estado=@estado ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Estado, Funciones.GetEnumByStringValueAttribute(Estado.Value)));
                flag = true;
            }
          
            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" p.fecha>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" p.fecha<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" pl.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" pl.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(pl.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(pl.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(pl.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(pl.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(pl.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(pl.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(pl.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(pl.fkarticulos,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(pl.fkarticulos,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(pl.fkarticulos,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(pl.fkarticulos,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkzonaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonasdesde", FkzonaDesde);
                sb.Append(" p.fkzonaalmacen >= @fkzonasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkzonasDesde, FkzonaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkzonaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonashasta", FkzonaHasta);
                sb.Append(" p.fkzonaalmacen <= @fkzonashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkzonasHasta, FkzonaHasta));
                flag = true;
            }

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            if (Agrupacion == "1")
            {
                sb.AppendFormat(
                "select  ud.decimalestotales as [_decimalesunidades],p.lote [Lote],p.id as [Bundle], a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],az.descripcion as [Zona Almacen],sum(pl.cantidad) as [Cantidad],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],sum(pl.metros) as [Metros],ud.textocorto as [UM], sum(sh.pesonetolote) as [Peso Neto] from bundle as p " +
                " inner join bundlelin as pl on pl.empresa= p.empresa and pl.fkbundle= p.id and pl.fkbundlelote=p.lote " +
                " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0 " +
                " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
                " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
                " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
                " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
                " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
                " left join almaceneszona as az on az.empresa=p.empresa and az.fkalmacenes= p.fkalmacen and az.id=p.fkzonaalmacen " +
                " left join stockhistorico as sh  on sh.empresa= p.empresa and sh.lote=pl.lote and sh.loteid=pl.loteid ");
            }

            else
            {
                sb.AppendFormat(
                 "select  ud.decimalestotales as [_decimalesunidades],p.lote [Lote],p.id as [Bundle], a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],az.descripcion as [Zona Almacen],sum(pl.cantidad) as [Cantidad],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],pl.largo as [Largo],pl.ancho as [Ancho],pl.grueso as [Grueso],pl.metros as [Metros],ud.textocorto as [UM] from bundle as p " +
                 " inner join bundlelin as pl on pl.empresa= p.empresa and pl.fkbundle= p.id and pl.fkbundlelote=p.lote " +
                 " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0 " +
                 " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
                 " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
                 " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
                 " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
                 " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
                 " left join almaceneszona as az on az.empresa=p.empresa and az.fkalmacenes= p.fkalmacen and az.id=p.fkzonaalmacen ");

            }


            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;
            switch (Agrupacion)
            {
                case "1":
                    result = " group by p.lote,p.id, a.id,a.descripcionabreviada,az.descripcion,gm.descripcion,fm.descripcion,ud.textocorto,  ud.decimalestotales";
                    break;
                case "2":
                    result = "  group by p.lote,p.id, a.id,a.descripcionabreviada,az.descripcion,gm.descripcion,fm.descripcion,ud.textocorto, ud.decimalestotales,pl.largo,pl.ancho,pl.grueso,pl.metros";
                    break;
            }


            result += " order by p.lote,p.id,a.id";

            return result;
        }

    
    }
}
