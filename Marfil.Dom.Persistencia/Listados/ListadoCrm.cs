using Marfil.Dom.Persistencia.Listados.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RCrm = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CRM;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using Marfil.Dom.Persistencia.Model.Configuracion;
using System.Xml.Serialization;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoCrm : ListadosModel
    {

        private List<string> _estados = new List<string>();
        private List<string> _modulos = new List<string>();

        #region CTR

        public ListadoCrm()
        {
        }

        public ListadoCrm(IContextService context) : base(context)
        {
            FechaInforme = DateTime.Today;
            FechaAperturaDesde = new DateTime(FechaInforme.Value.Year, 1, 1);
            FechaAperturaHasta = new DateTime(FechaInforme.Value.Year, 12, 31);
            
        }

        #endregion

        public override string TituloListado => "Listado CRM";
        public override string IdListado => FListadosModel.ListadoCrm;

        #region Propierties


        [Display(ResourceType = typeof(RMovs), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "FechaAperturaDesde")]
        public DateTime? FechaAperturaDesde { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "FechaAperturaHasta")]
        public DateTime? FechaAperturaHasta { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "FechaProxSeguiDesde")]
        public DateTime? FechaProxSeguiDesde { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "FechaProxSeguiHasta")]
        public DateTime? FechaProxSeguiHasta { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "TerceroDesde")]
        public string TerceroDesde { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "TerceroHasta")]
        public string TerceroHasta { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "Cerrado")]
        public string Cerrado { get; set; }



        [XmlIgnore]
        public List<EstadosModel> ListEstados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {


                    var list = estadoService.GetStates(DocumentoEstado.Todos).ToList();
                    list.AddRange(estadoService.GetStates(DocumentoEstado.Oportunidades).ToList());
                    list.AddRange(estadoService.GetStates(DocumentoEstado.Proyectos).ToList());
                    list.AddRange(estadoService.GetStates(DocumentoEstado.Incidencias).ToList());
                    list.AddRange(estadoService.GetStates(DocumentoEstado.Campañas).ToList());


                    

                    var result = new List<EstadosModel>();
                    foreach (var item in list)
                    {
                        if (result.Where(f => f.CampoId == item.CampoId).SingleOrDefault() == null)
                            result.Add(item);
                    }

                     return result;
                }
            }
        }

        [Display(ResourceType = typeof(RCrm), Name = "Estado")]
        public string Estado { get; set; }

        public List<string> Estados
        {
            get { return _estados; }
            set { _estados = value; }
        }

        [Required]
        public List<string> ListModulos
        {
            get
            {
                var list = new List<string>();

                list.Add("Oportunidades");
                list.Add("Proyectos");
                list.Add("Campañas");
                list.Add("Incidencias");


                return list;
            }
        }


      
        [Display(ResourceType = typeof(RCrm), Name = "Modulo")]
        public string Modulo { get; set; }

        [Required]
        public List<string>Modulos
        {
            get { return _modulos; }
            set { _modulos = value; }
        }

        #endregion Propierties

        #region Filters

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            ValoresParametros.Clear();
            Condiciones.Clear();

            if (Modulos.Count != 0)
            {
                sb.Append(" t.empresa = '" + Empresa + "' ");

                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaInforme, FechaInforme));


                if (FechaAperturaDesde.HasValue)
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("fechaaperturadesde", FechaAperturaDesde.Value);
                    sb.Append(" t.fechadocumento>=@fechaaperturadesde ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.FechaAperturaDesde, FechaAperturaDesde));
                }

                if (FechaAperturaHasta.HasValue)
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("fechaaperturahasta", FechaAperturaHasta.Value);
                    sb.Append(" t.fechadocumento<=@fechaaperturahasta ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.FechaAperturaHasta, FechaAperturaHasta));
                }

                if (FechaProxSeguiDesde.HasValue)
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("fechaproxseguidesde", FechaProxSeguiDesde.Value);
                    sb.Append(" t.fechaproximoseguimiento>=@fechaproxseguidesde ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.FechaProxSeguiDesde, FechaProxSeguiDesde));
                }

                if (FechaProxSeguiHasta.HasValue)
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("fechaproxseguihasta", FechaProxSeguiHasta.Value);
                    sb.Append(" t.fechaproximoseguimiento<=@fechaproxseguihasta ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.FechaProxSeguiHasta, FechaProxSeguiHasta));
                }

                if (!string.IsNullOrEmpty(TerceroDesde))
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("tercerodesde", TerceroDesde);
                    sb.Append("  [Cod. tercero] = @tercerodesde  ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.TerceroDesde, TerceroDesde));
                }

                if (!string.IsNullOrEmpty(TerceroHasta))
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("tercerohasta", TerceroHasta);
                    sb.Append("  [Cod. tercero] = @tercerohasta ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.TerceroHasta, TerceroHasta));
                }

                if (Cerrado == "0" || Cerrado == "1")
                {
                    sb.Append(" AND ");
                    ValoresParametros.Add("cerrado", Cerrado);
                    sb.Append(" t.cerrado = @cerrado ");
                    Condiciones.Add(string.Format("{0}: {1}", RCrm.Cerrado, Cerrado));
                }                

            }
            return sb.ToString();
        }

        #endregion Filters

        #region Select

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            if(Modulos.Count == 0)
            {
                sb.AppendFormat(
                   " SELECT '' AS[Referencia], '' AS [Estado], '' AS[Fecha apertura], '' AS[Fecha ult.seg.], '' AS[fecha prox.seg.], "+
                   "  '' AS[Cod.tercero], '' AS[Nombre tercero], "+
                   "  '' AS[Cerrado], '' AS[Asunto], '' AS[Nombre operario], '' AS[Nombre comercial], '' AS[Nombre agente], 1 AS Doc");
            }else { 

            sb.AppendFormat(
                " SELECT referencia AS [Referencia], [Estado], fechadocumento AS [Fecha apertura], fechaultimoseguimiento AS [Fecha ult. seg.], fechaproximoseguimiento AS [fecha prox. seg.],"+
                " [Cod. tercero], [Nombre tercero], (CASE WHEN cerrado = 0 THEN 'N' ELSE 'S' END) AS[Cerrado], asunto AS[Asunto], [Nombre operario], [Nombre comercial], [Nombre agente], 1 AS Doc" +
                " FROM(");
            foreach (var item in Modulos)
            {



                if (item == "Oportunidades")
                {
                    sb.AppendFormat(
                          " SELECT o.empresa, o.fketapa, e.descripcion AS [Estado], o.referencia, o.fechadocumento, o.fechaultimoseguimiento, o.fechaproximoseguimiento, o.fkempresa AS [Cod. tercero], c.descripcion AS [Nombre tercero], " +
                          " o.cerrado, o.asunto, op.descripcion AS[Nombre operario], co.descripcion AS[Nombre comercial], ag.descripcion AS[Nombre agente] " +
                          " FROM Oportunidades AS o " +
                          " LEFT JOIN Cuentas AS c ON c.empresa = o.empresa AND c.id = o.fkempresa " +
                          " LEFT JOIN Cuentas AS op ON op.empresa = o.empresa AND op.id = o.fkoperario " +
                          " LEFT JOIN Cuentas AS co ON co.empresa = o.empresa AND co.id = o.fkcomercial " +
                          " LEFT JOIN Cuentas AS ag ON ag.empresa = o.empresa AND ag.id = o.fkagente" +
                          " LEFT JOIN Estados AS e ON (CAST(e.documento AS varchar) + '-' + e.id) = o.fketapa");
                }

                if (item == "Proyectos")
                {
                    sb.AppendFormat(
                          " SELECT p.empresa, p.fketapa, e.descripcion AS [Estado], p.referencia, p.fechadocumento, p.fechaultimoseguimiento, p.fechaproximoseguimiento, p.fkempresa AS [Cod. tercero], c.descripcion AS [Nombre tercero], " +
                          " p.cerrado, p.asunto, op.descripcion AS[Nombre operario], co.descripcion AS[Nombre comercial], ag.descripcion AS[Nombre agente] " +
                          " FROM Proyectos AS p " +
                          " LEFT JOIN Cuentas AS c ON c.empresa = p.empresa AND c.id = p.fkempresa " +
                          " LEFT JOIN Cuentas AS op ON op.empresa = p.empresa AND op.id = p.fkoperario " +
                          " LEFT JOIN Cuentas AS co ON co.empresa = p.empresa AND co.id = p.fkcomercial " +
                          " LEFT JOIN Cuentas AS ag ON ag.empresa = p.empresa AND ag.id = p.fkagente" +
                          " LEFT JOIN Estados AS e ON (CAST(e.documento AS varchar) + '-' + e.id) = p.fketapa");
                    }

                if (item == "Campañas")
                {
                    sb.AppendFormat(
                          " SELECT c.empresa, c.fketapa, e.descripcion AS [Estado], c.referencia, c.fechadocumento, c.fechaultimoseguimiento, c.fechaproximoseguimiento, '' AS [Cod. tercero], '' AS [Nombre tercero], " +
                          " c.cerrado, c.asunto, op.descripcion AS[Nombre operario], co.descripcion AS[Nombre comercial], ag.descripcion AS[Nombre agente] " +
                          " FROM Campañas AS c " +
                          " LEFT JOIN Cuentas AS op ON op.empresa = c.empresa AND op.id = c.fkoperario " +
                          " LEFT JOIN Cuentas AS co ON co.empresa = c.empresa AND co.id = c.fkcomercial " +
                          " LEFT JOIN Cuentas AS ag ON ag.empresa = c.empresa AND ag.id = c.fkagente"+
                          " LEFT JOIN Estados AS e ON (CAST(e.documento AS varchar) + '-' + e.id) = c.fketapa");
                    }

                if (item == "Incidencias")
                {
                    sb.AppendFormat(
                          " SELECT i.empresa, i.fketapa, e.descripcion AS [Estado], i.referencia, i.fechadocumento, i.fechaultimoseguimiento, i.fechaproximoseguimiento, i.fkempresa AS [Cod. tercero], c.descripcion AS [Nombre tercero], " +
                          " i.cerrado, i.asunto, op.descripcion AS[Nombre operario], co.descripcion AS[Nombre comercial], ag.descripcion AS[Nombre agente] " +
                          " FROM IncidenciasCRM AS i " +
                          " LEFT JOIN Cuentas AS c ON c.empresa = i.empresa AND c.id = i.fkempresa " +
                          " LEFT JOIN Cuentas AS op ON op.empresa = i.empresa AND op.id = i.fkoperario " +
                          " LEFT JOIN Cuentas AS co ON co.empresa = i.empresa AND co.id = i.fkcomercial " +
                          " LEFT JOIN Cuentas AS ag ON ag.empresa = i.empresa AND ag.id = i.fkagente" +
                          " LEFT JOIN Estados AS e ON (CAST(e.documento AS varchar) + '-' + e.id) = i.fketapa");
                    }

                if(item != Modulos[Modulos.Count - 1])
                {
                    sb.AppendFormat(" UNION ALL ");
                }
            }
            sb.AppendFormat(")t");
            }
            return sb.ToString();
        }

        #endregion

        #region Orden

        /*internal override string GenerarOrdenColumnas()
        {
            return "ORDER BY [Doc.], Orden ";
        }*/

        #endregion

    }
}
