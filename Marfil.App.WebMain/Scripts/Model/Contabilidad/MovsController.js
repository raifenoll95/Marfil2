app.controller('MovsCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {
    $scope.isnuevo = false;
    $scope.Codigodescripcionasiento = "";
    $scope.Tipodocumento = "";

    $scope.init = function (isnuevo, Codigodescripcionasiento){//},referencia) {
        $scope.isnuevo = isnuevo;
        //$scope.urlreferescarlineas = urlrefrescar;
        //$scope.referencia = referencia;
        $scope.Codigodescripcionasiento = Codigodescripcionasiento;
     }

  
    eventAggregator.RegisterEvent("Fkseriescontables-cv", function (message) {
        $("[name='Tipodocumento']").val(message.Tipodocumento);
        $("#Fkmonedas").val(message.Fkmonedas);
    });


    eventAggregator.RegisterEvent("Fkmonedas-cv", function (message) {
        if (!$scope.firstEditClientes && message) {
            $("[name='Tipocambio']").val(message.CambioMonedaBase);
        }
    });

    $scope.$watch("Codigodescripcionasiento", function (newValue, oldValue) {
        if (newValue===oldValue) {
            return;
        }
        var descripcion = $("#Codigodescripcionasiento").find('option:selected').text();
        $("#Descripcionasiento").val(descripcion);
    });
}]);

var ImportesDebeHaber = function (lineas, importedebe, importehaber) {
    var importe = Globalize.format(Funciones.Redondear(0, articuloActual.Decimalesmonedas));
    if (importedebe !=0)
    {
        var cHaber = lineas.GetEditor("SHaber");
        cHaber.SetValue(importe);
    }
    if (importeHaber != 0) {
        var cDebe = lineas.GetEditor("SDebe");
        cDebe.SetValue(importe);
    }
}

var obtenerSeccionanalitica = function (codigo, urlseccionesanaliticas, lineas) {
    if (codigo != null) {
        console.log("Código en la función obtener" + codigo);
        $.get(urlseccionesanaliticas + "?id=" + codigo).success(function (result) {
            // Daba error
            //var cFkseccionesanaliticas = lineas.GetEditor("Fkseccionesanaliticas");
            //cFkseccionesanaliticas.SetValue(codigo);
            if (result !== 'undefined' && result !== '') {
                console.log("ok");
                $("#seannombre").val(result.Nombre);
                $("#seangrupo").val(result.Grupo);
                lineas.FocusEditor("SDebe");
            } else {
                console.log("ko");
                cFkseccionesanaliticas.SetValue("");
                $("#seannombre").val("");
                $("#seangrupo").val("");
                lineas.FocusEditor("Fkseccionesanaliticas");
            }
        }).error(function(jqXHR, textStatus, errorThrown) {
            console.log("error mostrarSeccionanalitica");
            $("#seannombre").val("");
            $("#seangrupo").val("");
            var cFkseccionesanaliticas = lineas.GetEditor("Fkseccionesanaliticas");
            cFkseccionesanaliticas.SetValue("");
            lineas.FocusEditor("Fkseccionesanaliticas");
        })};
}

var obtenerComentario = function (codigo, urlcomentariosasientos, lineas) {
    if ( codigo != null) {
        $.get(urlcomentariosasientos + "?id=" + codigo).success(function (result) {
         
        var cCodigocomentario = lineas.GetEditor("Codigocomentario");
        cCodigocomentario.SetValue(codigo);
        var cComentario = lineas.GetEditor("Comentario");

        if (result.length > 0) {;
            cComentario.SetValue(result[0].Text);
            lineas.FocusEditor("Comentario");
        } else {
            cCodigocomentario.SetValue("");
            cComentario.SetValue("");
            lineas.FocusEditor("Codigocomentario");
        }

    }).error(function (jqXHR, textStatus, errorThrown) {
            console.log("error obtenerCodigocomentario");
        })};
}

// muestra informacion abajo
// mostrar datos informativos en el pie _desglose
var mostrarCuenta = function (codigocuenta, urlcuentas, lineas) {
    lineas.SetEnabled(true);      

    $("#maesfkcuentas").val("");
    $("#maesdescripcion").val("");
    $("#maesdebe").val(0);
    $("#maeshaber").val(0);
    $("#maessaldo").val(0);

    if ( codigocuenta != null) {
        $.get(urlcuentas + "?id=" + codigocuenta).success(function (result) {
            $("#maesfkcuentas").val(codigocuenta);
            $("#maesdescripcion").val(result.Descripcion);            
            $("#maesdebe").val(result.SDebe)
            $("#maeshaber").val(result.SHaber);
            $("#maessaldo").val(result.SSaldo);
        }).error(function (jqXHR, textStatus, errorThrown) {
            console.log("error mostrarCuenta");
            Fkcuentas.SetValue("");
            Fkcuentas.Focus();
        })};
}