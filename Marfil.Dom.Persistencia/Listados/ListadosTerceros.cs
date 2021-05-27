using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosTerceros : ListadosModel
    {
        

        #region Properties

        public override string TituloListado => "Listado de terceros";

        public override string IdListado => FListadosModel.Terceros;

        [Display(ResourceType = typeof(Rcuentas), Name = "Tiposcuentas")]
        public TiposCuentas Tipocuenta { get; set; }

        public string Order { get; set; }

        [Display(ResourceType = typeof(Rcuentas), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(Rcuentas), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "DescripcionDesde")]
        public string CuentaDescripcionDesde { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "DescripcionHasta")]
        public string CuentaDescripcionHasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkfamiliacliente")]
        public string FamiliaCliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkzonacliente")]
        public string ZonaCliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentasagente")]
        public string Agente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkunidadnegocio")]
        public string Fkunidadnegocio { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fktipoempresa")]
        public string Fktipoempresa { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentascomercial")]
        public string Comercial { get; set; }

        [Display(ResourceType = typeof(RDireccion), Name = "Fkpais")]
        public string Fkpais { get; set; }

        [Display(ResourceType = typeof(RDireccion), Name = "Fkprovincia")]
        public string Fkprovincia { get; set; }

        [Display(ResourceType = typeof(Rcuentas), Name = "BloqueoModel")]
        public bool Bloqueado { get; set; }


        #endregion

        public ListadosTerceros()
        {

        }

        public ListadosTerceros(IContextService context) : base(context)
        {
            Tipocuenta = TiposCuentas.Clientes;
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" c.empresa = '" + Empresa + "' ");

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" c.fkcuentas>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" c.fkcuentas<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDescripcionDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadescripciondesde", CuentaDescripcionDesde );
                sb.Append(" cu.descripcion >=  @cuentadescripciondesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.DescripcionDesde, CuentaDescripcionDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDescripcionHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadescripcionhasta", CuentaDescripcionHasta + "ZZZZZZ");//Truño para que que coja todos los elementos. Por ejemplo : si ponemos hasta "e", no va a coger los que empiecen por "EA" si no ponemos eso
                sb.Append(" cu.descripcion <=  @cuentadescripcionhasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.DescripcionHasta, CuentaDescripcionHasta));
                flag = true;
            }

            if (Tipocuenta != TiposCuentas.Cuentastesoreria &&
                Tipocuenta != TiposCuentas.Agentes &&
                Tipocuenta != TiposCuentas.Aseguradoras &&
                Tipocuenta != TiposCuentas.Operarios &&
                Tipocuenta != TiposCuentas.Comerciales)
            {
                if (!string.IsNullOrEmpty(Fkunidadnegocio))
                {
                    if (flag)
                        sb.Append(" AND ");
                    AppService = new ApplicationHelper(Context);
                    ValoresParametros.Add("fkunidadnegocio", Fkunidadnegocio);
                    sb.Append(" c.fkunidadnegocio = @fkunidadnegocio ");
                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkunidadnegocio, AppService.GetListUnidadNegocio().SingleOrDefault(f => f.Valor == Fkunidadnegocio).Descripcion));
                    flag = true;
                }

                if (!string.IsNullOrEmpty(Fktipoempresa))
                {
                    if (flag)
                        sb.Append(" AND ");
                    AppService = new ApplicationHelper(Context);
                    ValoresParametros.Add("fktipoempresa", Fktipoempresa);
                    sb.Append(" c.fktipoempresa = @fktipoempresa ");
                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fktipoempresa, AppService.GetListTiposEmpresas().SingleOrDefault(f => f.Valor == Fktipoempresa).Descripcion));
                    flag = true;
                }
            }

            if (Tipocuenta == TiposCuentas.Clientes ||Tipocuenta==TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores)
            {

                if (!string.IsNullOrEmpty(FamiliaCliente))
                {
                    if (flag)
                        sb.Append(" AND ");
                    var columnafamilia = Tipocuenta == TiposCuentas.Proveedores ? "fkfamiliaproveedor" : "fkfamiliacliente";
                    ValoresParametros.Add("familiacliente", FamiliaCliente);
                    sb.AppendFormat(" c.{0} = @familiacliente ", columnafamilia);

                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkfamiliacliente, FamiliaCliente));
                    flag = true;
                }

                if (!string.IsNullOrEmpty(ZonaCliente))
                {
                    if (flag)
                        sb.Append(" AND ");
                    var columnazona = Tipocuenta == TiposCuentas.Proveedores ? "fkzonaproveedor" : "fkzonacliente";
                    ValoresParametros.Add("zonacliente", ZonaCliente);
                    sb.AppendFormat(" c.{0} = @zonacliente ", columnazona);
                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkzonacliente, ZonaCliente));
                    flag = true;
                }
            }

            if (Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos)
            {
                if (!string.IsNullOrEmpty(Agente))
                {
                    if (flag)
                        sb.Append(" AND ");

                    ValoresParametros.Add("agente", Agente);
                    sb.Append(" c.fkcuentasagente = @agente ");
                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkcuentasagente, Agente));
                    flag = true;
                }

                if (!string.IsNullOrEmpty(Comercial))
                {
                    if (flag)
                        sb.Append(" AND ");

                    ValoresParametros.Add("comercial", Comercial);
                    sb.Append(" c.fkcuentascomercial = @comercial ");
                    Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkcuentascomercial, Comercial));
                    flag = true;
                }
            }
            

            if (!string.IsNullOrEmpty(Fkpais))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("pais", Fkpais);
                sb.Append(" di.fkpais  =@pais ");
                Condiciones.Add(string.Format("{0}: {1}", RDireccion.Fkpais, AppService.GetListPaises().SingleOrDefault(f => f.Valor == Fkpais).Descripcion));
                flag = true;
                if (!string.IsNullOrEmpty(Fkprovincia))
                {
                    if (flag)
                        sb.Append(" AND ");
                    using (var serviceProvincias = new ProvinciasService(Context, MarfilEntities.ConnectToSqlServer(Context.BaseDatos)))
                    {
                        var prov=serviceProvincias.get(Fkpais + "-" + Fkprovincia) as ProvinciasModel;
                        Condiciones.Add(string.Format("{0}: {1}", RDireccion.Fkprovincia, prov.Nombre));
                    }

                    ValoresParametros.Add("provincia", Fkprovincia);
                    sb.Append(" di.fkprovincia  =@provincia ");
                    
                    flag = true;
                }

            }

            if (Bloqueado)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("bloqueado", Bloqueado);
                sb.Append(" cu.bloqueada= '"+ Bloqueado.ToString().ToLower() +"'");
                Condiciones.Add(Bloqueado
                    ? string.Format("{0}", Rcuentas.BloqueoModel)
                    : string.Format("No {0}", Rcuentas.BloqueoModel));
                flag = true;
            }

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            /// Atencion!
            /// A la hora de generar la select hay que tener en cuenta el tipo de cuenta sobre el que vamos a hacer la consulta
            /// ya que es posible que no todas las columnas sean comunes a la aplicación.
            
            var condicionestiposempresas = Tipocuenta != TiposCuentas.Cuentastesoreria &&
                Tipocuenta != TiposCuentas.Agentes &&
                Tipocuenta != TiposCuentas.Aseguradoras &&
                Tipocuenta != TiposCuentas.Operarios &&
                Tipocuenta != TiposCuentas.Comerciales ? " left join tiposempresas te on te.valor= c.fktipoempresa left join unidadesnegocio un on un.valor= c.fkunidadnegocio " : string.Empty;
            var columnastiposempresas = Tipocuenta != TiposCuentas.Cuentastesoreria &&
                Tipocuenta != TiposCuentas.Agentes &&
                Tipocuenta != TiposCuentas.Aseguradoras &&
                Tipocuenta != TiposCuentas.Operarios &&
                Tipocuenta != TiposCuentas.Comerciales ? " , un.descripcion as [Unidad negocio], te.descripcion as [Tipo empresa]" : string.Empty;

            var condicionesagente = Tipocuenta == TiposCuentas.Clientes  ? " left join cuentas as ag on ag.empresa=c.empresa and ag.id=c.fkcuentasagente left join cuentas as co on co.empresa=c.empresa and co.id=c.fkcuentascomercial " : string.Empty;
            var columnasagente = Tipocuenta == TiposCuentas.Clientes  ? ", ag.descripcion as [Agente], co.descripcion [Comercial]" :string.Empty;

            var columnazona = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ? Tipocuenta == TiposCuentas.Proveedores ? "fkzonaproveedor" : "fkzonacliente" :  string.Empty;
            var condicioneszonacliente = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ? " left join zonasclientes as z on z.valor = c." + columnazona : string.Empty;
            var columnaszonacliente = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ? " ,z.descripcion as [Zona]" : string.Empty;

            var columnafamilia = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ? Tipocuenta == TiposCuentas.Proveedores ? "fkfamiliaproveedor" : "fkfamiliacliente" : string.Empty;
            var columnasfamiliasclientes = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ? " , fc.descripcion as [Familia cliente] " : string.Empty;
            var condicionesvamiliasclientes = Tipocuenta == TiposCuentas.Clientes || Tipocuenta == TiposCuentas.Prospectos || Tipocuenta == TiposCuentas.Proveedores ?  " left join familiasclientes as fc on fc.valor=c." + columnafamilia : string.Empty;
            sb.AppendFormat(
                "select ISNULL(cu.bloqueada,0) as [Bloqueado],cu.id as Id,cu.descripcion as [Descripción],cu.nif as [Nif],di.direccion as [Direccion],di.poblacion [Población],pr.nombre [Provincia],p.descripcion as [País],di.email as [Email],di.telefono as [Teléfono], di.telefonomovil  as [Teléfono movil] {4} {5} {3} {7} from {0} as c " +
                " inner join cuentas as cu on c.empresa= cu.empresa and c.fkcuentas=cu.id and cu.tipocuenta= " +(int)Tipocuenta + " " +
                " left join direcciones as di on di.empresa=c.empresa and di.tipotercero=" + (int)Tipocuenta + " and di.fkentidad=c.fkcuentas and di.defecto='true' " +
                " left join paises as p on p.Valor=di.fkpais " +
                " left join provincias as pr on pr.codigopais=di.fkpais and pr.id=di.fkprovincia" +
                " {1} " +
                " {2} {6} {8} ", Tipocuenta, condicionestiposempresas, condicionesagente,columnastiposempresas, columnasagente, columnaszonacliente, condicioneszonacliente,columnasfamiliasclientes,condicionesvamiliasclientes);

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            switch (Order)
            {
                case "1":
                    return " order by cu.id";
                    
                case "2":
                    return " order by cu.descripcion";
                case "3":
                    return " order by p.descripcion,di.fkprovincia,di.poblacion,cu.id";
                    
            }

            return result;
        }
    }
}
