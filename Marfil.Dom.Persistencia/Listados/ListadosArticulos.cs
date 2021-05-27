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
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Materiales;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosArticulos : ListadosModel
    {
        private List<string> _series = new List<string>();

        #region Properties

        public override string TituloListado => "Listado de artículos";

        public override string IdListado => FListadosModel.Articulos;

        public string Order { get; set; }

        public string Tipodescripcion { get; set; }

        public string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FamiliaMateriales")]
        public string Fkfamiliasmateriales{ get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FamiliaDesde")]
        public string FkfamiliasDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FamiliaHasta")]
        public string FkfamiliasHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "MaterialesDesde")]
        public string FkmaterialesDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "MaterialesHasta")]
        public string FkmaterialesHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "CaracteristicasDesde")]
        public string FkcaracteristicasDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "CaracteristicasHasta")]
        public string FkcaracteristicasHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "GrosoresDesde")]
        public string FkgrosoresDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "GrosoresHasta")]
        public string FkgrosoresHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "AcabadosDesde")]
        public string FkacabadosDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "AcabadosHasta")]
        public string FkacabadosHasta { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Fkgruposmateriales")]
        public string Fkgruposmateriales { get; set; }

        #endregion

        public ListadosArticulos()
        {
            
        }

        public ListadosArticulos(IContextService context) : base(context)
        {
           
            
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true; 
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" a.empresa = '" + Empresa + "' ");
           

            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" a.id >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" a.id <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(a.id,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(a.id,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(a.id,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkgruposmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService=new ApplicationHelper(Context);
                ValoresParametros.Add("fkgruposmateriales", Fkgruposmateriales);
                sb.Append(" a.fkgruposmateriales = @fkgruposmateriales  ");
                Condiciones.Add(string.Format("{0}: {1}", RMateriales.Fkgruposmateriales, Fkgruposmateriales));
                //Condiciones.Add(string.Format("{0}: {1}", RMateriales.Fkgruposmateriales, AppService.GetListGrupoMateriales().SingleOrDefault(f => f.Valor == Fkgruposmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(a.id,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(a.id,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(a.id,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(a.id,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(a.id,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(a.id,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(a.id,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(a.id,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }

            

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            string descripcion;
            if (Tipodescripcion == "1")
                descripcion = "descripcionabreviada";
            else if (Tipodescripcion == "2") descripcion = "descripcion";
            else descripcion = "descripcion2";

            sb.AppendFormat(
                "select a.id as [Cod.],a.{0} as [Articulo],ud.textocorto as [UM],fp.id as [Cod. Familia],fp.descripcion as [Familia],m.id  as [Cod. Material],m.descripcion as [Material]," +
                " m.fkgruposmateriales as [Cod. Grupo material], gm.descripcion as [Grupo material]," +
                " a.fkgruposiva as [Cod. Grupo IVA], gc.descripcion as [Guía contable],a.gestionstock as [Gestion de Stock]," +
                " a.existenciasminimasmetros as [Exists. min metros], a.existenciasmaximasmetros as [Exists. max metros]," +
                " a.existenciasminimasunidades as [Exists. min unidades], a.existenciasmaximasunidades as [Exists. max unidades]" +
                " from articulos as a " +
                " inner join familiasproductos as fp on fp.empresa= a.empresa and fp.id=substring(a.id,0,3) " +
                " left join materiales  as m on m.empresa=a.empresa and m.id=substring(a.id,3,3) " +
                " left join caracteristicaslin as cl on cl.empresa= a.empresa and cl.fkcaracteristicas=substring(a.id,0,3) and cl.id=substring(a.id,6,2) " +
                " left join grosores as g on g.id=substring(a.id,8,2) " +
                " left join acabados as ac on ac.id=substring(a.id,10,2)" +
                " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
                " left join gruposmateriales as gm on gm.valor= m.fkgruposmateriales " +
                " left join guiascontables as gc on gc.empresa=a.empresa and gc.id=fp.fkguiascontables ", descripcion);



            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            switch (Order)
            {
                case "1":
                    return " order by a.id";
                case "2":
                    return " order by a.descripcion";

            }

            return result;
        }
    }
}
