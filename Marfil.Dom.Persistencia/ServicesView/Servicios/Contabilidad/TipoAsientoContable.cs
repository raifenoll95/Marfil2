using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;
using Resources;
using RTiposasientoscontables = Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI.TiposAsientosContables;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ListadoMovs;
//using RKit = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Kit;
//using RBundle = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bundle;
//using RReservas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;
//using RTraspasos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Traspasosalmacen;
//using RInventarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Inventarios;
//using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad
{


    public enum TipoAsientoContable
    {
     
        [StringValue(typeof(RTiposasientoscontables), "EnumNormal")]
        Normal,
        [StringValue(typeof(RTiposasientoscontables), "EnumSimulacion")]
        Simulacion,
        [StringValue(typeof(RTiposasientoscontables), "EnumAsientoVinculado")]
        AsientoVinculado,
        [StringValue(typeof(RTiposasientoscontables), "EnumAperturaProvisional")]
        AperturaProvisional,
        [StringValue(typeof(RTiposasientoscontables), "EnumApertura")]
        Apertura,
        [StringValue(typeof(RTiposasientoscontables), "EnumRegularizacionExistencias")]
        RegularizacionExistencias,
        [StringValue(typeof(RTiposasientoscontables), "EnumRegularizacionGrupos6y7")]
        RegularizacionGrupos6y7,
        [StringValue(typeof(RTiposasientoscontables), "EnumCierre")]
        Cierre
    }


    public enum TipoAsientoImpresion
    {
        [StringValue(typeof(RTiposasientoscontables), "EnumNormal")]
        Normal,
        [StringValue(typeof(RTiposasientoscontables), "EnumSimulacion")]
        Simulacion,
        [StringValue(typeof(RTiposasientoscontables), "EnumAsientoVinculado")]
        AsientoVinculado,
        [StringValue(typeof(RTiposasientoscontables), "EnumAperturaProvisional")]
        AperturaProvisional,
        [StringValue(typeof(RTiposasientoscontables), "EnumApertura")]
        Apertura,
        [StringValue(typeof(RTiposasientoscontables), "EnumRegularizacionExistencias")]
        RegularizacionExistencias,
        [StringValue(typeof(RTiposasientoscontables), "EnumRegularizacionGrupos6y7")]
        RegularizacionGrupos6y7,
        [StringValue(typeof(RTiposasientoscontables), "EnumCierre")]
        Cierre,
        [StringValue(typeof(RMovs), "TituloListado")]
        ListadoComisiones = 20
        //[StringValue(typeof(RKit), "TituloEntidadSingular")]
        //Kit = 30,
        //[StringValue(typeof(RBundle), "TituloEntidadSingular")]
        //Bundle = 40,
        //[StringValue(typeof(RReservas), "TituloEntidadSingular")]
        //Reservasstock = 50,
        //[StringValue(typeof(RTraspasos), "TituloEntidadSingular")]
        //Traspasosalmacen = 60,
        //[StringValue(typeof(RInventarios), "TituloEntidadSingular")]
        //Inventarios = 70,
        //[StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        //Transformaciones = 80,
        //[StringValue(typeof(RTransformaciones), "TituloEntidadSingular")]
        //Transformacioneslotes = 90

    }



}
