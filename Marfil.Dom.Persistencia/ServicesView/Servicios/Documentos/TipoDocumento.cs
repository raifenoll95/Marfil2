using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;
using Resources;
using RTiposdocumentos = Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI.TiposDocumentos;
using RTiposdocumentoscontables = Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI.TiposDocumentosContables;
using RComisiones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ListadoComisiones;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
using RBundle = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bundle;
using RReservas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;
using RTraspasos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Traspasosalmacen;
using RInventarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inventarios;
using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using RImputacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ImputacionCostes;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos
{
    public enum TipoDocumento
    {
        [StringValue(typeof(RTiposdocumentos), "EnumPresupuestosVentas")]
        PresupuestosVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumPedidosVentas")]
        PedidosVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumAlbaranesVentas")]
        AlbaranesVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumFacturasVentas")]
        FacturasVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumPresupuestosCompras")]
        PresupuestosCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumPedidosCompras")]
        PedidosCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumAlbaranesCompras")]
        AlbaranesCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumFacturasCompras")]
        FacturasCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumReservas")]
        Reservas,
        [StringValue(typeof(RTiposdocumentos), "EnumTraspasos")]
        Traspasosalmacen,
        [StringValue(typeof(RInventarios), "TituloEntidadSingular")]
        Inventarios,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        Transformaciones,
        [StringValue(typeof(RKit), "TituloEntidadSingular")]
        Kits,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        Transformacioneslotes,
        [StringValue(typeof(RDivisionLotes), "TituloEntidadSingular")]
        DivisionLotes,
        [StringValue(typeof(RImputacionCostes), "TituloEntidadSingular")]
        ImputacionCostes,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumAsientos")]
        Asientos=1000,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumInmovilizado")]
        Inmovilizado=1001,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumIvaSoportado")]
        IvaSoportado = 1002,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumIvaRepercutido")]
        IvaRepercutido = 1003,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionCobros")]
        PrevisionCobros = 1004,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        PrevisionPagos = 1005,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        ListadoMargen = 1025,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingularAcabados")]
        TransformacionesAcabados = 2000

    }
    public enum TipoDocumentoImpresion
    {
        [StringValue(typeof(RTiposdocumentos), "EnumPresupuestosVentas")]
        PresupuestosVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumPedidosVentas")]
        PedidosVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumAlbaranesVentas")]
        AlbaranesVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumFacturasVentas")]
        FacturasVentas,
        [StringValue(typeof(RTiposdocumentos), "EnumPresupuestosCompras")]
        PresupuestosCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumPedidosCompras")]
        PedidosCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumAlbaranesCompras")]
        AlbaranesCompras,
        [StringValue(typeof(RTiposdocumentos), "EnumFacturasCompras")]
        FacturasCompras,
        [StringValue(typeof(RComisiones), "TituloListado")]
        ListadoComisiones =20,
        [StringValue(typeof(RKit), "TituloEntidadSingular")]
        Kit = 30,
        [StringValue(typeof(RBundle), "TituloEntidadSingular")]
        Bundle = 40,
        [StringValue(typeof(RReservas), "TituloEntidadSingular")]
        Reservasstock = 50,
        [StringValue(typeof(RTraspasos), "TituloEntidadSingular")]
        Traspasosalmacen = 60,
        [StringValue(typeof(RInventarios), "TituloEntidadSingular")]
        Inventarios = 70,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        Transformaciones = 80,
        [StringValue(typeof(RDivisionLotes), "TituloEntidadSingular")]
        DivisionLotes = 85,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        Transformacioneslotes = 90,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumAsientos")]
        Asientos = 1000,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumInmovilizado")]
        Inmovilizado = 1001,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumIvaSoportado")]
        IvaSoportado = 1002,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumIvaRepercutido")]
        IvaRepercutido = 1003,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionCobros")]
        PrevisionCobros = 1004,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        PrevisionPagos = 1005,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        Mayor = 1010,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        SumasYSaldos = 1015,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        BalancePedidos = 1020,
        [StringValue(typeof(RTiposdocumentoscontables), "EnumPrevisionPagos")]
        ListadoMargen = 1025,
        [StringValue(typeof(RImputacionCostes), "TituloEntidadSingular")]
        ImputacionCostes = 1030,
        [StringValue(typeof(RCobrosYPagos), "TituloEntidadCartera")]
        CarteraVencimientos = 1050,
        [StringValue(typeof(RCobrosYPagos), "TituloRemesa")]
        Remesa = 1060,
        [StringValue(typeof(RTransformaciones), "TituloEntidadSingularAcabados")]
        TransformacionesAcabados = 2000
    }



}
