eventAggregator.RegisterEvent("Fktiposiva-changed", function(msg) {
    $.get(msg.Url + "/" + msg.Id).success(function (tipoiva) {

        var porcentajeiva = tipoiva.PorcentajeIva;
        var porcentajerecargoequivalencia = tipoiva.PorcentajeRecargoEquivalencia;
        msg.CtrlPorcentajeiva.SetValue(porcentajeiva);
        msg.CtrlPorcentajeequivalencia.SetValue(porcentajerecargoequivalencia)
        msg.CtrlGridview.SetEnabled(true);
        calculoImporte(msg.CtrlGridview);
    }).error(function (jqXHR, textStatus, errorThrown) {
        msg.CtrlGridview.SetEnabled(true);
    });

});

var calculoImporte = function (lineas) {
    var cBase = lineas.GetEditor("Baseimponible");
    var cPorcentaje = lineas.GetEditor("Porcentajeiva");
    var cPorcentajeretencion = lineas.GetEditor("Porcentajerecargoequivalencia");
    var cCuotaiva = lineas.GetEditor("Cuotaiva");
    var cCuotaretencion = lineas.GetEditor("Importerecargoequivalencia");
    var cSubtotal = lineas.GetEditor("Subtotal");

    var base = cBase.GetValue() * 1;
    var cuota = cBase.GetValue() * (cPorcentaje.GetValue() / 100.0);
    var retencion = cBase.GetValue() * (cPorcentajeretencion.GetValue() / 100.0);
    var subtotal = base + cuota + retencion;

    base = Globalize.format(Funciones.Redondear(base, 3));
    cuota = Globalize.format(Funciones.Redondear(cuota, 3));
    retencion = Globalize.format(Funciones.Redondear(retencion, 3));
    subtotal = Globalize.format(Funciones.Redondear(subtotal, 3));

    cBase.SetValue(base);
    cCuotaiva.SetValue(cuota);
    cCuotaretencion.SetValue(retencion);
    cSubtotal.SetValue(subtotal);
}