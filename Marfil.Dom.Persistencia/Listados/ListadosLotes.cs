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
using RGeneral = Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosLotes : ListadosModel
    {

        #region Properties

        #region Default
        public override string TituloListado => Menuaplicacion.listadolotes;

        public override string IdListado
        {
            get
            {
                var resultado = string.Empty;
                resultado = FListadosModel.InformeLotes;
                return resultado;
            }
        }

        public override string WebIdListado => FListadosModel.InformeLotes;

        #endregion Default

        public string Order { get; set; }
        
        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteDesde")]
        public string LotesDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteHasta")]
        public string LotesHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        #endregion Properties

        public ListadosLotes()
        {

        }

        public ListadosLotes(IContextService context) : base(context)
        {

        }


        #region Helper
        #region SQL

        #region Filtros
        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.AppendFormat(" h.empresa='{0}' ", Empresa);



            if (!string.IsNullOrEmpty(LotesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("lotesdesde", LotesDesde);
                sb.Append(" h.lote >= @lotesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.LoteDesde, LotesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(LotesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("loteshasta", LotesHasta);
                sb.Append(" h.lote <= @loteshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.LoteHasta, LotesHasta));
                flag = true;
            }


            if (!string.IsNullOrEmpty(Fkarticulos))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulos", Fkarticulos);
                sb.Append(" h.fkarticulos >= @fkarticulos  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkarticulos, Fkarticulos));
                flag = true;
            }

            return sb.ToString();
        }
        #endregion Filtros

        #region SELECT
        internal override string GenerarSelect()
        {
            return CadenaSinagrupacion();
        }
        #endregion SELECT

        #region Cadena Select


        #region CadenaAgrupacion
        private string CadenaSinagrupacion()
        {
            var sb = new StringBuilder();
            // basada en la de la vista lotes
            sb.AppendFormat(" SELECT ");
            sb.AppendFormat("h.fkarticulos AS Codarticulo, ");
            sb.AppendFormat("a.descripcionabreviada AS Articulo, ");
            sb.AppendFormat("h.lote, ");
            sb.AppendFormat("h.loteid, ");
            
            // Entrada
            sb.AppendFormat("ISNULL(entrada.cantidad, te.cantidad) AS CantidadEntrada, ");
            sb.AppendFormat("ISNULL(entrada.largo, te.largo) AS LargoEntrada, ");
            sb.AppendFormat("ISNULL(entrada.ancho, te.ancho) AS AnchoEntrada, ");
            sb.AppendFormat("ISNULL(entrada.grueso, te.grueso) AS GruesoEntrada, ");
            sb.AppendFormat("ISNULL(entrada.metros, te.metros) AS MetrosEntrada, ");

            // Produccion
            //sb.AppendFormat("h.cantidaddisponible AS CantidadProduccion, ");
            sb.AppendFormat("h.largo AS LargoProduccion, ");
            sb.AppendFormat("h.ancho AS AnchoProduccion, ");
            sb.AppendFormat("h.grueso AS GruesoProduccion, ");
            sb.AppendFormat("h.metros AS MetrosProduccion, ");

            // Salida
            sb.AppendFormat("ISNULL(salida.cantidad, ts.cantidad) AS CantidadSalida, ");
            sb.AppendFormat("ISNULL(salida.largo, ts.largo) AS LargoSalida, ");
            sb.AppendFormat("ISNULL(salida.ancho, ts.ancho) AS AnchoSalida, ");
            sb.AppendFormat("ISNULL(salida.grueso, ts.grueso) AS GruesoSalida, ");
            sb.AppendFormat("ISNULL(salida.metros, ts.metros) AS MetrosSalida, ");
            sb.AppendFormat("u.codigounidad AS Unidades, ");
            sb.AppendFormat("k.referencia AS Kit, ");
            sb.AppendFormat("b.fkbundle AS Bundle, ");

            sb.AppendFormat("CASE WHEN ISNULL(s.cantidaddisponible,0) > 0 THEN '{0}'ELSE '{1}' END AS EnStock, ","Sí","No");

            sb.AppendFormat("CASE h.tipoalmacenlote WHEN 0 THEN 'Mercadería' WHEN 1 THEN 'Propio' WHEN 2 THEN 'Gestionado' END AS TipoLote ");
            sb.AppendFormat("FROM ");
            sb.AppendFormat("dbo.Stockhistorico AS h INNER JOIN ");
            sb.AppendFormat("dbo.Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos INNER JOIN ");
            sb.AppendFormat("dbo.Unidades AS u ON h.fkunidadesmedida = u.id LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.KitLin AS kl ON kl.empresa = h.empresa AND kl.lote = h.lote AND kl.loteid = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.Kit AS k ON k.empresa = kl.empresa AND k.id = kl.fkkit LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.BundleLin AS b ON b.empresa = h.empresa AND b.lote = h.lote AND b.loteid = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.Transformacionesentradalin AS te ON te.empresa = h.empresa AND te.lote = h.lote AND te.tabla = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid LEFT OUTER JOIN ");
            sb.AppendFormat("dbo.Stockactual AS s ON s.empresa=h.empresa and s.lote=h.lote and s.loteid=h.loteid ");

            return sb.ToString();
        }

        #endregion CadenaAgrupacion




        #endregion


        #endregion Helper
        #endregion SQL
    }
}
