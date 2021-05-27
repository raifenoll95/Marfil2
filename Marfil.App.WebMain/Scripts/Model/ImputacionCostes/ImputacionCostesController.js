app.controller('ImputacionCostesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.init = function () {
    }

    //Creo que es cuando le das al boton guardar
    $scope.Guardarcostes = function () {
        console.log("guardar");
        $("#forzarcostesadicionales").submit();
    }


    eventAggregator.RegisterEvent("imputacionCostesSalidaLinea", function (message) {

        console.log(message.Ancho);

        if (message.Tipofamilia == "1") {
            $scope.UltimoLoteSalida = message.Lote;
        }

        $scope.Materialsalida = message.Fkarticulos.substring(2, 5);
    });


    //----------------------------------------LINEAS ENTRADA Y SALIDA------------------------------------------------

    $scope.NuevaLineaLote = function () {

        console.log("Nueva linea lote");

        $('#_consinstock').on('shown.bs.modal', function () {
            $("#Fkarticulosentrada").focus();
        });

        $("#_consinstock").modal('show');
        FinalizarHidden();
    }

    $scope.Nuevalineacoste = function () {
        GridViewCosteAdicionalmpCostes.AddNewRow();
    }
    eventAggregator.RegisterEvent("nuevalineacoste", function (msg) {
        GridViewCosteAdicionalmpCostes.AddNewRow();
    });

}]);



//-----------------OTRA COSA-----------------

var articuloActual = null;
var pedirArticulo = function (codigoarticulo, urlarticulos, lineas, setvaloresdefecto) {

    $.get(urlarticulos + "/" + codigoarticulo, { flujo: '0', fkmonedas: $("[name='Fkmonedas']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {
        //$.get().success(function (result) {
        articuloActual = result;

        var escomentario = result.Articulocomentario;
        var cCantidad = lineas.GetEditor("Cantidad");
        var cLote = lineas.GetEditor("Lote");
        var cLargo = lineas.GetEditor("SLargo");
        var cAncho = lineas.GetEditor("SAncho");
        var cGrueso = lineas.GetEditor("SGrueso");
        var cMetros = lineas.GetEditor("SMetros");
        lineas.SetEnabled(true);
        cCantidad.SetEnabled(!escomentario && (!result.Permitemodificarlargo && !result.Permitemodificarancho && !result.Permitemodificargrueso ));
        cLote.SetEnabled(!escomentario);
        
        
        cLargo.SetEnabled(result.Permitemodificarlargo && !escomentario);
        cAncho.SetEnabled(result.Permitemodificarancho && !escomentario);
        cGrueso.SetEnabled(result.Permitemodificargrueso && !escomentario);
        cMetros.SetEnabled(result.Permitemodificarmetros && !escomentario);
        
        
        if (setvaloresdefecto)
            establecerValoresDefecto(lineas);

        lineas.FocusEditor("Descripcion");

    }).error(function (jqXHR, textStatus, errorThrown) {
        lineas.SetEnabled(true);
        Fkarticulos.SetValue("");
        Fkarticulos.Focus();

    });
}


/*
*/

var establecerValoresDefecto = function (lineas) {
    var cDescripcion = lineas.GetEditor("Descripcion");
    var cCantidad = lineas.GetEditor("Cantidad");
    var cLargo = lineas.GetEditor("SLargo");
    var cAncho = lineas.GetEditor("SAncho");
    var cGrueso = lineas.GetEditor("SGrueso");
    var cMetros = lineas.GetEditor("SMetros");
 
    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo,articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho,articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso, articuloActual.Decimalestotales));
    cDescripcion.SetValue(articuloActual.Descripcion);
    if (articuloActual.Articulocomentario) {
        cCantidad.SetValue(0);
    }
    cMetros.SetValue(FFormulasService.CreateFormula(articuloActual.Formulas).calculate(cCantidad.GetValue(), cLargo.GetValue(), cAncho.GetValue(), cGrueso.GetValue(), cMetros.GetValue(), articuloActual.Decimalestotales));

 
    cDescripcion.Focus();
}


// Ojo. problema del idioma (al no tener id busco por el title y este cambiará si no es español)
function FinalizarHidden() {
    $("a").each(function () {
        if (this.title == "Finalizar") {
            $(this).css("visibility", "hidden");
        }
    })
}