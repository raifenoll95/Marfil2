using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Resources;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using Marfil.Inf.Genericos;

using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    internal class LineasLotes
    {
        public string Cuenta { get; set; }
        public DateTime? Fecha { get; set; }
        public double? Precio { get; set; }
        public string Referencia { get; set; }
        public string Codigodocumento { get; set; }
        public double? Cantidad { get; set; }
        public double? Largo { get; set; }
        public double? Ancho { get; set; }
        public double? Grueso { get; set; }
        public double? Metros { get; set; }
        public double? Preciovaloracion { get; set; }
    }

    public class ListadoLotesModel : IToolbar
    {
        #region Properties

        public ToolbarModel Toolbar { get; set; }

        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteDesde")]
        public string LoteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteHasta")]
        public string LoteHasta { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranes), Name = "EnStock")]
        public bool EnStock { get; set; }

        public IContextService Context { get; set; }

        #endregion

        #region CTR

        public ListadoLotesModel()
        {
            Toolbar = new ToolbarModel();
            Toolbar.Titulo = General.Lotes;
            Toolbar.Operacion = TipoOperacion.Custom;
            Toolbar.CustomAction = true;
            Toolbar.CustomActionName = "Index";
        }

        #endregion

        #region Api

        public IEnumerable<string> GetFiltros()
        {
            var list = new List<string>();

            if (!string.IsNullOrEmpty(LoteDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.LoteDesde, LoteDesde));
            if (!string.IsNullOrEmpty(LoteHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.LoteHasta, LoteHasta));

            if (!string.IsNullOrEmpty(FkarticulosDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
            if (!string.IsNullOrEmpty(FkarticulosHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
            if (!string.IsNullOrEmpty(FkfamiliasHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
            if (!string.IsNullOrEmpty(FkmaterialesHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasDesde, FkcaracteristicasDesde));
            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasHasta, FkcaracteristicasHasta));

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresDesde, FkgrosoresDesde));
            if (!string.IsNullOrEmpty(FkgrosoresHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresHasta, FkgrosoresHasta));

            if (!string.IsNullOrEmpty(FkacabadosDesde))
                list.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosDesde, FkacabadosDesde));
            if (!string.IsNullOrEmpty(FkacabadosHasta))
                list.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosHasta, FkacabadosHasta));
            return list;
        }

        #endregion
    }

    public struct StLotesDocumentosRelacionados
    {
        public TipoOperacionService? Tipodocumento { get; set; }
        public string Referencia { get; set; }
        public string Url { get; set; }
    }

    public enum EstadoStock
    {
        [StringValue(typeof(RAlbaranes),"EnStock")]
        EnStock,
        [StringValue(typeof(RAlbaranes), "SinStock")]
        SinStock,
        [StringValue(typeof(RAlbaranes), "Reservado")]
        Reservado,
        [StringValue(typeof(RAlbaranes), "Transformacion")]
        Transformacion
    }

    public class LotesModel : IToolbar
    {
        #region Members

        private GaleriaModel _galeria;

        #endregion

        #region Properties

        public ToolbarModel Toolbar { get; set; }

        public string Lote { get; set; }        

        public string Empresa { get; set; }
        [Display(ResourceType = typeof(RAlbaranes), Name = "Lote")]
        public string Lotereferencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkproveedores")]
        public string Codigoproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechadocumento")]
        public DateTime? Fechaentrada { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Referencia")]
        public string Referenciaentrada { get; set; }

        public string Codigodocumentoentrada { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Precio")]
        public double? Precioentrada { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkclientes")]
        public string Codigocliente { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechadocumento")]
        public DateTime? Fechasalida { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Referencia")]
        public string Referenciasalida { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
        public EstadoStock Estado { get; set; }

        public string Codigodocumentosalida { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Precio")]
        public double? Preciosalida { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkvariedades")]
        public string Fkvariedades { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkincidenciasmaterial")]
        public string Fkincidenciasmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktonomaterial")]
        public string Fktonomaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktipograno")]
        public string Fktipograno { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkalmacen")]
        public string Fkalmacenes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkalmacen")]
        public string NombreAlmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkzonas")]
        public string Fkalmaceneszona { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Loteproveedor")]
        public string Loteproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Zona")]
        public string Zona { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkcalificacioncomercial")]
        public string Fkcalificacioncomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        public List<StLotesDocumentosRelacionados> Documentosrelacionados { get; set; }

        public Guid? Fkcarpetas { get; set; }

        public GaleriaModel Galeria
        {
            get
            {
                _galeria = new GaleriaModel();
                if (Fkcarpetas.HasValue)
                {
                    _galeria.Empresa = Empresa;
                    _galeria.DirectorioId = Fkcarpetas.Value;
                }
                return _galeria;
            }
        }


        [Display(ResourceType = typeof(RAlbaranes), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        
        //totales
    [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Netocompra")]
        public double? Netocompra { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "CosteNeto")]
        public double? Costeneto
        {
            get
            {
                return (Netocompra ?? 0)
                       + (Costeadicionalportes ?? 0)
                       + (Costeadicionalmaterial ?? 0)
                       + (Costeadicionalotro ?? 0)
                       + (Costeadicionalvariable ?? 0);
            }
        }

        public double? Costeadicionalportes { get; set; }
        public double? Costeadicionalmaterial { get; set; }
        public double? Costeadicionalotro { get; set; }
        public double? Costeadicionalvariable { get; set; }

        public string SNetocompra { get { return (Netocompra ?? 0).ToString("N2"); } }
        public string SCosteneto { get { return (Costeneto ?? 0).ToString("N2"); } }
        public string SCosteadicionalportes { get { return (Costeadicionalportes ?? 0).ToString("N2"); } }
        public string SCosteadicionalmaterial { get { return (Costeadicionalmaterial ?? 0).ToString("N2"); } }
        public string SCosteadicionalotro { get { return (Costeadicionalotro ?? 0).ToString("N2"); } }
        public string SCosteadicionalvariable { get { return (Costeadicionalvariable ?? 0).ToString("N2"); } }

        //compra
        public double? Netocompracompra
        {
            get
            {
                if(MetrosEntrada!=0)
                    return Math.Round((Netocompra/MetrosEntrada) ?? 0, 2);

                return 0;
            }
        }

        public double? Costeadicionalportescompra
        {
            get
            {
                if(MetrosEntrada!=0)
                    return Math.Round((Costeadicionalportes/MetrosEntrada) ?? 0, 2);

                return 0;
            }
        }

        public double? Costeadicionalmaterialcompra
        {
            get
            {
                if(MetrosEntrada!=0)
                return Math.Round((Costeadicionalmaterial/MetrosEntrada) ?? 0, 2);
                return 0;
            }
        }

        public double? Costeadicionalotrocompra
        {
            get
            {
                if(MetrosEntrada!=0)
                return Math.Round((Costeadicionalotro/MetrosEntrada) ?? 0, 2);

                return 0;
            }
        }

        public double? Costeadicionalvariablecompra
        {
            get
            {
                if(MetrosEntrada!=0)
                    return Math.Round((Costeadicionalvariable/MetrosEntrada) ?? 0, 2);

                return 0;
            }
        }

        public double? Costenetocompra
        {
            get
            {
                return (Netocompracompra ?? 0)
                    + (Costeadicionalportescompra ?? 0)
                    + (Costeadicionalmaterialcompra ?? 0)
                    + (Costeadicionalotrocompra ?? 0)
                    + (Costeadicionalvariablecompra ?? 0);
            }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Preciovaloracion")]
        public double? Preciovaloracion { get; set; }

        public double? PreciovaloracionConCostesAdicionales
        {
            get
            {
                return (Preciovaloracion ?? 0)
                    + (Costeadicionalportescompra ?? 0)
                    + (Costeadicionalmaterialcompra ?? 0)
                    + (Costeadicionalotrocompra ?? 0)
                    + (Costeadicionalvariablecompra ?? 0);
            }
        }

        public string SNetocompracompra { get { return (Netocompracompra ?? 0).ToString("N2"); } }
        public string SCostenetocompra { get { return (Costenetocompra ?? 0).ToString("N2"); } }
        public string SCosteadicionalportescompra { get { return (Costeadicionalportescompra ?? 0).ToString("N2"); } }
        public string SCosteadicionalmaterialcompra { get { return (Costeadicionalmaterialcompra ?? 0).ToString("N2"); } }
        public string SCosteadicionalotrocompra { get { return (Costeadicionalotrocompra ?? 0).ToString("N2"); } }
        public string SCosteadicionalvariablecompra { get { return (Costeadicionalvariablecompra ?? 0).ToString("N2"); } }

        public string SPreciovaloracion { get { return (PreciovaloracionConCostesAdicionales ?? 0).ToString("N2"); } }

        //produccion    

        public int Formula { get; set; }

        //Se añade este campo para los objetos sin gestión de lote    
        public double? MetrosProduccionTotales
        {
            get
            {
                if (Formula == (int)TipoStockFormulas.Cantidad)
                    return CantidadEntrada;
                if (Formula == (int)TipoStockFormulas.Linear)
                    return CantidadEntrada * LargoProduccion;
                if (Formula == (int)TipoStockFormulas.Superficie)
                    return CantidadEntrada * LargoProduccion * AnchoProduccion;
                if (Formula == (int)TipoStockFormulas.Volumen)
                    return CantidadEntrada * LargoProduccion * AnchoProduccion * GruesoProduccion;

                return MetrosProduccion;
            }
        }

        public double? Netocompraproduccion
        {
            get
            {
                if(MetrosProduccion!=0)
                    return Math.Round((Netocompra/ MetrosProduccionTotales) ?? 0, 2);
                return 0;
            }
        }

        public double? Costeadicionalportesproduccion
        {
            get
            {
                if(MetrosProduccion!=0)
                return Math.Round((Costeadicionalportes/ MetrosProduccionTotales) ?? 0, 2);

                return 0;
            }
        }

        public double? Costeadicionalmaterialproduccion
        {
            get
            {
                if(MetrosProduccion!=0)
                return Math.Round((Costeadicionalmaterial/ MetrosProduccionTotales) ?? 0, 2);

                return 0;
            }
        }

        public double? Costeadicionalotroproduccion
        {
            get
            {
                if(MetrosProduccion!=0)
                    return Math.Round((Costeadicionalotro/ MetrosProduccionTotales) ?? 0, 2);
                return 0;
            }
        }

        public double? Costeadicionalvariableproduccion
        {
            get
            {
                if(MetrosProduccion!=0)
                return Math.Round((Costeadicionalvariable/MetrosProduccion) ?? 0, 2);

                return 0;
            }
        }

        public double? Costenetoproduccion
        {
            get
            {
                return (Netocompraproduccion ?? 0)
                    + (Costeadicionalportesproduccion ?? 0)
                    + (Costeadicionalmaterialproduccion ?? 0)
                    + (Costeadicionalotroproduccion ?? 0)
                    + (Costeadicionalvariableproduccion ?? 0);
            }
        }

        public string SNetocompraproduccion { get { return (Netocompraproduccion ?? 0).ToString("N2"); } }
        public string SCostenetoproduccion { get { return (Costenetoproduccion ?? 0).ToString("N2"); } }
        public string SCosteadicionalportesproduccion { get { return (Costeadicionalportesproduccion ?? 0).ToString("N2"); } }
        public string SCosteadicionalmaterialproduccion { get { return (Costeadicionalmaterialproduccion ?? 0).ToString("N2"); } }
        public string SCosteadicionalotroproduccion { get { return (Costeadicionalotroproduccion ?? 0).ToString("N2"); } }
        public string SCosteadicionalvariableproduccion { get { return (Costeadicionalvariableproduccion ?? 0).ToString("N2"); } }

        //medidas
        public int Decimales { get; set; }
        public string Unidades { get; set; }
        public double? CantidadProduccion { get; set; }
        public double? LargoProduccion { get; set; }
        public double? AnchoProduccion { get; set; }
        public double? GruesoProduccion { get; set; }
        public double? MetrosProduccion { get; set; }
        public double? MetrosDisponibles { get; set; }

        public double? CantidadEntrada { get; set; }
        public double? LargoEntrada { get; set; }
        public double? AnchoEntrada { get; set; }
        public double? GruesoEntrada { get; set; }
        public double? MetrosEntrada { get; set; }

        public double? CantidadSalida { get; set; }
        public double? LargoSalida { get; set; }
        public double? AnchoSalida { get; set; }
        public double? GruesoSalida { get; set; }
        public double? MetrosSalida { get; set; }

        public string SLargoProduccion { get { return LargoProduccion?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SAnchoProduccion { get { return AnchoProduccion?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SGruesoProduccion { get { return GruesoProduccion?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SMetrosProduccion { get { return (MetrosProduccionTotales)?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SMetrosDisponibles { get { return MetrosDisponibles?.ToString("N" + Decimales) ?? string.Empty; } }

        public string SLargoEntrada { get { return LargoEntrada?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SAnchoEntrada { get { return AnchoEntrada?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SGruesoEntrada { get { return GruesoEntrada?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SMetrosEntrada { get { return MetrosEntrada?.ToString("N" + Decimales) ?? string.Empty; } }

        public string SLargoSalida { get { return LargoSalida?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SAnchoSalida { get { return AnchoSalida?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SGruesoSalida { get { return GruesoSalida?.ToString("N" + Decimales) ?? string.Empty; } }
        public string SMetrosSalida { get { return MetrosSalida?.ToString("N" + Decimales) ?? string.Empty; } }

        

        #endregion

        public LotesModel()
        {
            Toolbar = new ToolbarModel();
            Toolbar.Titulo = General.Lotes;
            Toolbar.Operacion = TipoOperacion.Ver;

        }
    }
}
