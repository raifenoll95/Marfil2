eventAggregator.RegisterEvent("Fktiposiva-changed", function(msg) {
    $.get(msg.Url + "/" + msg.Id).success(function (tipoiva) {

        var porcentajeiva = tipoiva.PorcentajeIva;
        var porcentajerecargoequivalencia = tipoiva.PorcentajeRecargoEquivalencia;
        msg.CtrlPorcentajeiva.SetValue(porcentajeiva);
        msg.CtrlGridview.SetEnabled(true);
    }).error(function (jqXHR, textStatus, errorThrown) {
        msg.CtrlGridview.SetEnabled(true);
    });

});