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
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RProveedor = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosStock : ListadosModel
    {
        private List<string> _series = new List<string>();
        private string _agrupacion ="0";
        private string _detallepor ="0";

        #region Properties

        public override string TituloListado => Menuaplicacion.listadostock;

        public override string IdListado
        {
            get
            {
                var resultado = string.Empty;
                switch (Agrupacion)
                {
                    case "0":
                        resultado = FListadosModel.StockGeneral + Detallepor;
                        break;
                    case "1":
                        resultado = FListadosModel.StockAgrupadoArticulo + Detallepor;
                        break;
                    case "2":
                        resultado = FListadosModel.StockAgrupadoArticuloLote + Detallepor;
                        break;
                    case "3":
                        resultado = FListadosModel.StockAgrupadoArticuloLoteMedidas + Detallepor;
                        break;
                    case "4":
                        resultado = FListadosModel.StockAgrupadoArticuloMedidas + Detallepor;
                        break;
                }

                return resultado;
            }
        }

        

        public override string WebIdListado => FListadosModel.StockGeneral;

        public string Order { get; set; }


        public string Detallepor
        {
            get { return _detallepor; }
            set { _detallepor = value; }
        }

        public string Agrupacion
        {
            get { return _agrupacion; }
            set { _agrupacion = value; }
        }

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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }


        // Listado de lotes
        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteDesde")]
        public string LoteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteHasta")]
        public string LoteHasta { get; set; }


        #endregion

        public ListadosStock()
        {
            
        }

        public ListadosStock(IContextService context):base(context)
        {
            ConfiguracionColumnas.Add("L", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() {Columna = "_decimales"} });
            ConfiguracionColumnas.Add("A", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("G", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            Agrupacion = "0";

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
                            Tipoalmacenlote=RAlbaranesCompras.TipoAlmacenLotePropio;
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
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(s.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
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

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(s.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
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
            if (Agrupacion == "0")
            {
                return CadenaSinagrupacion();
            }
            else if (Agrupacion == "1")
            {
                return CadenaAgrupadoArticulos();
            }
            else if (Agrupacion == "2")
            {
                return CadenaAgrupadoArticulosLote();
            }
            else if (Agrupacion == "3")
            {
                return CadenaAgrupadoArticulosLoteMedidas();
            }
            else if (Agrupacion == "4")
            {
                return CadenaAgrupadoArticulosMedidas();
            }

            return CadenaSinagrupacion();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            if (Agrupacion == "0")
            {
                result = CadenaAgrupacionSinagrupacion();
            }
            else if (Agrupacion == "1")
            {
                result = CadenaAgrupacionAgrupadoArticulos();
            }
            else if (Agrupacion == "2")
            {
                result = CadenaAgrupacionAgrupadoArticulosLote();
            }
            else if (Agrupacion == "3")
            {
                result = CadenaAgrupacionAgrupadoArticulosLoteMedidas();
            }
            else if (Agrupacion == "4")
            {
                result = CadenaAgrupacionAgrupadoArticulosMedidas();
            }

            switch (Order)
            {
                case "1":
                    result += " order by s.fkarticulos ";
                    break;
            }

            

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
                
                var service = FService.Instance.GetService(typeof (SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.AlbaranesVentas);
            }
        }

        #region Helper

        #region Cadena Select

        private string GetColumnasAlmacen()
        {
            switch (_detallepor)
            {
                case "0":
                    return string.Empty;
                case "1":
                    return " s.fkalmacenes as [Cod. Almacén],alm.descripcion as [Almacén], ";
                case "2":
                    return " s.fkalmacenes as [Cod. Almacén],alm.descripcion as [Almacén],almz.descripcion as [Zona], ";
            }

            return string.Empty;
        }

        private string GetColumnasAgrupacionAlmacen()
        {
            switch (_detallepor)
            {
                case "0":
                    return string.Empty;
                case "1":
                    return " ,s.fkalmacenes,alm.descripcion ";
                case "2":
                    return ", s.fkalmacenes ,alm.descripcion,almz.descripcion  ";
            }

            return string.Empty;
        }
        private string CadenaSinagrupacion()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],");
            sb.AppendFormat("fm.Descripcion as [Familia material],s.lote as [Lote],");
            sb.AppendFormat("dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote) as [Tipo de Lote],"); //
            sb.AppendFormat("s.loteid as [Tabla],s.cantidaddisponible as [Cant. Disponible],s.largo as [L],");
            sb.AppendFormat("s.ancho as [A],s.grueso as [G],s.metros as [Metros],u.codigounidad as [UM],u.decimalestotales as [_decimales] ");
            sb.AppendFormat(" from stockactual as s ");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            }
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");


            return sb.ToString();
        }

        private string CadenaAgrupadoArticulos()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material],");
            sb.AppendFormat("dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote) as [Tipo de Lote],"); //

            //columnas sum
            sb.AppendFormat("sum( s.cantidaddisponible) as [Cant. Disponible],sum(s.metros) as [Metros],");

            sb.AppendFormat("u.codigounidad as [UM],u.decimalestotales as [_decimales] ");
            sb.AppendFormat(" from stockactual as s ");

            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            }
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");

            return sb.ToString();
        }

        private string CadenaAgrupadoArticulosLote()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],s.lote as [Lote],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material],");
            //columnas sum
            sb.AppendFormat(" sum(s.cantidaddisponible) as [Cant. Disponible],sum(s.metros) as [Metros],");

            sb.AppendFormat("u.codigounidad as [UM],u.decimalestotales as [_decimales] from stockactual as s ");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            }
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");

            return sb.ToString();
        }

        private string CadenaAgrupadoArticulosLoteMedidas()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],s.lote as [Lote],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material],");
            
            //campos sum
            sb.AppendFormat(" sum(s.cantidaddisponible) as [Cant. Disponible], ");

            sb.AppendFormat(" s.largo as [L],s.ancho as [A],s.grueso as [G],");
            
            //campos sum
            sb.AppendFormat(" sum(s.metros) as [Metros],");

            sb.AppendFormat(" u.codigounidad as [UM],u.decimalestotales as [_decimales] ");
            sb.AppendFormat(" from stockactual as s ");

            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            }
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");

            return sb.ToString();
        }

        public string CadenaAgrupadoArticulosMedidas()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material],");
            //campos sum
            sb.AppendFormat(" sum(s.cantidaddisponible) as [Cant. Disponible],");
            sb.AppendFormat("s.largo as [L],s.ancho as [A],s.grueso as [G],");
            //campos sum
            sb.AppendFormat("sum(s.metros) as [Metros],");
            sb.AppendFormat("u.codigounidad as [UM],u.decimalestotales as [_decimales] ");
            sb.AppendFormat("from stockactual as s ");
            //---
            sb.AppendFormat("select {0} s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material], ", GetColumnasAlmacen());
            //campos sum
            sb.AppendFormat("sum(s.cantidaddisponible) as [Cant. Disponible],");
            sb.AppendFormat("s.largo as [L],s.ancho as [A],s.grueso as [G],");
            //campos sum
            sb.AppendFormat("sum(s.metros) as [Metros],");
            sb.AppendFormat("u.codigounidad as [UM],u.decimalestotales as [_decimales] ");
            sb.AppendFormat(" from stockactual as s ");
            //--
            sb.AppendFormat("select {0} s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material], sum(s.cantidaddisponible) as [Cant. Disponible],s.largo as [L],s.ancho as [A],s.grueso as [G],sum(s.metros) as [Metros],u.codigounidad as [UM],u.decimalestotales as [_decimales] from stockactual as s ", GetColumnasAlmacen());
            //--
            sb.AppendFormat("select {0} s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familia material], sum(s.cantidaddisponible) as [Cant. Disponible],s.largo as [L],s.ancho as [A],s.grueso as [G],sum(s.metros) as [Metros],u.codigounidad as [UM],u.decimalestotales as [_decimales] from stockactual as s ", GetColumnasAlmacen());


            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            }
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");

           
            return sb.ToString();
        }

        #endregion

        #region Cadena agrupacion

        public string CadenaAgrupacionSinagrupacion()
        {
            return string.Empty;
        }

        public string CadenaAgrupacionAgrupadoArticulos()
        {
            return " group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion,u.codigounidad,u.decimalestotales " + 
                        GetColumnasAgrupacionAlmacen() +
                        ", dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote)";
        }

        public string CadenaAgrupacionAgrupadoArticulosLote()
        {
            return " group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion, s.lote,u.codigounidad,u.decimalestotales" +
                        GetColumnasAgrupacionAlmacen() +
                   ", dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote)";
        }

        public string CadenaAgrupacionAgrupadoArticulosLoteMedidas()
        {
            return " group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion, s.lote,s.largo,s.ancho,s.grueso,u.codigounidad,u.decimalestotales" + 
                        GetColumnasAgrupacionAlmacen() +
                    ", dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote)";
        }

        public string CadenaAgrupacionAgrupadoArticulosMedidas()
        {
            return " group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion, s.largo,s.ancho,s.grueso,u.codigounidad,u.decimalestotales" + 
                GetColumnasAgrupacionAlmacen() +
                ", dbo._fn_enum_TipoAlmacenlote(s.tipoalmacenlote)";
        }

        #endregion

        #endregion
    }
}
