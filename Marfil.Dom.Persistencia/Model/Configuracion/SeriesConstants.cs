using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;
using RGrupoMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GrupoMateriales;
using RSeries = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Series;
using RInventarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inventarios;
using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RTransformacioneslotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformacioneslotes;
using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ListadoMovs;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Materiales;
using RImputacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ImputacionCostes;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Almacenes;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TipoDocumentos
    {
        //compras
        [StringValue(typeof(RSeries), "TipoDocumentosPresupuestosCompra")]
        PresupuestosCompras,
        [StringValue(typeof(RSeries), "TipoDocumentosAlbaranesCompra")]
        AlbaranesCompras,
        [StringValue(typeof(RSeries), "TipoDocumentosPedidosCompra")]
        PedidosCompras,
        [StringValue(typeof(RSeries), "TipoDocumentosFacturasCompra")]
        FacturasCompras,
        //ventas
        [StringValue(typeof(RSeries), "TipoDocumentosPresupuestosVentas")]
        PresupuestosVentas,
        [StringValue(typeof(RSeries), "TipoDocumentosReservasVentas")]
        Reservas,
        [StringValue(typeof(RSeries), "TipoDocumentosAlbaranesVentas")]
        AlbaranesVentas,
        [StringValue(typeof(RSeries), "TipoDocumentosPedidosVentas")]
        PedidosVentas,
        [StringValue(typeof(RSeries), "TipoDocumentosFacturasVentas")]
        FacturasVentas,
        //stock
        [StringValue(typeof(RSeries), "TipoDocumentosApuntesAlmacen")]
        ApuntesAlmacen,
        [StringValue(typeof(RSeries), "TipoDocumentosApuntesAlmacen")]
        Traspasosalmacen,
        [StringValue(typeof(RInventarios), "TituloEntidad")]
        Inventarios,
        [StringValue(typeof(RTransformaciones), "TituloEntidad")]
        Transformaciones,
        [StringValue(typeof(RKit), "TituloEntidad")]
        Kit,
        [StringValue(typeof(RTransformacioneslotes), "TituloEntidad")]
        Transformacioneslotes,
        [StringValue(typeof(RMovs), "TituloEntidad")]
        Asientos,
        
        [StringValue(typeof(RArticulos), "TituloEntidad")]
        Articulos,
        [StringValue(typeof(RMateriales), "TituloEntidad")]
        Materiales,
        [StringValue(typeof(RGrupoMateriales), "TituloEntidad")]
        GrupoMateriales,
        [StringValue(typeof(RImputacionCostes), "TituloEntidad")]
        ImputacionCostes,
        [StringValue(typeof(RAlmacenes), "TituloEntidad")]
        Almacenes,
        [StringValue(typeof(RFamilias), "TituloEntidad")]
        Familias,
        [StringValue(typeof(RCobrosYPagos), "TituloEntidadCartera")]
        CarteraVencimientos,
        [StringValue(typeof(RCobrosYPagos), "TituloRemesa")]
        Remesa,
        [StringValue(typeof(RTransformaciones), "TituloEntidadAcabados")]
        TransformacionesAcabados
    }
}
