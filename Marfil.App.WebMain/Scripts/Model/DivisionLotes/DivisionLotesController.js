app.controller('DivisionLotesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.verificarmateriales = false;
    $scope.Materialsalida = "";
    $scope.isnuevo = false;
    $scope.fkbancosmandatos = "";
    $scope.UltimoLoteSalida = "";
    $scope.urlDescargaDefecto = "";


    $scope.init = function (firstEdit, isnuevo, urlDescargaDefecto, ultimoLoteSalida, verificarmateriales, materialsalida) {

        console.log(firstEdit);
        console.log(isnuevo);
        console.log(urlDescargaDefecto);
        console.log(ultimoLoteSalida);
        console.log(verificarmateriales);
        console.log(materialsalida);

        $scope.UltimoLoteSalida = ultimoLoteSalida;
        $scope.isnuevo = isnuevo;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $scope.verificarmateriales = verificarmateriales;
        $scope.Materialsalida = materialsalida;
        $("#PedidosDesde").prop("readonly", true);
        $("#btnbuscarPedidosDesde").prop("disabled", true);

        $("#PedidosHasta").prop("readonly", true);
        $("#btnbuscarPedidosHasta").prop("disabled", true);
    }

    //Creo que es cuando le das al boton guardar
    $scope.Guardarcostes = function () {
        console.log("guardar");
        $("#forzarcostesadicionales").submit();
    }

    eventAggregator.RegisterEvent("Fkseries-cv", function (message) {
        $("[name='Tipodealmacenlote']").val(message.Tipodealmacenlote);
    });


    eventAggregator.RegisterEvent("transformacionSalidaLinea", function (message) {
        if (message.Tipofamilia == "1") {
            $scope.UltimoLoteSalida = message.Lote;
        }

        $scope.Materialsalida = message.Fkarticulos.substring(2, 5);
    });


    eventAggregator.RegisterEvent("_nuevalineasalida", function (msg) {
        $scope.Nuevalinea();
    });


    //----------------------------------------LINEAS ENTRADA Y SALIDA------------------------------------------------

    $scope.Nuevalineasalida = function () {
        
        if ($("[name='Fkoperarios']").val() && $("[name='Fkoperarios']").val() !== "") {
            $('#_entregastock').on('shown.bs.modal', function () {
                $("#Fkarticulosentrada").focus();
            });

            $("#_entregastock").modal('show');
            FinalizarHidden();
        } else {
            bootbox.alert("El campo proveedor es obligatorio");
        }
    }

    $scope.Nuevalineaentrada = function () {

        $('#_entradastock').on('shown.bs.modal', function () {
            $("#Fkarticulosentrada").focus();
        });

        if ($scope.UltimoLoteSalida!="")
            eventAggregator.Publish("setloteBloqueSalida", $scope.UltimoLoteSalida);
        if ($scope.verificarmateriales && $scope.Materialsalida != "")
            eventAggregator.Publish("setMaterialSalida", $scope.Materialsalida);
        $("#_entradastock").modal('show');

        FinalizarHidden();
    }


    eventAggregator.RegisterEvent("Fkarticulos-Buscarcancelar", function (message) {
        Fkarticulos.Focus();
    });

    $scope.Nuevalineacoste = function () {
        GridViewCosteAdicional.AddNewRow();
    }
    eventAggregator.RegisterEvent("nuevalineacoste", function (msg) {
        GridViewCosteAdicional.AddNewRow();
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