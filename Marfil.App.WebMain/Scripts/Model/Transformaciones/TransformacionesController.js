app.controller('TransformacionesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.verificarmateriales = false;
    $scope.Materialsalida = "";
    $scope.isnuevo = false;
    $scope.fkbancosmandatos = "";
    $scope.Fkproveedores = "";
    $scope.cliente = "";
    $scope.UltimoLoteSalida = "";
    $scope.urlDescargaDefecto = "";
    $scope.init = function (firstEdit, cliente, isnuevo, urlDescargaDefecto, ultimoLoteSalida, verificarmateriales, materialsalida) {

        //Si es nueva transformacion:
        //firstEdit false, cliente vacio, isNuevo true, urlDescargaDefecto ;;, ultimoLoteSalida nada, verificarmateriales true, materialSalida nada

        console.log(firstEdit);
        console.log(cliente);
        console.log(isnuevo);
        console.log(urlDescargaDefecto);
        console.log(ultimoLoteSalida);
        console.log(verificarmateriales);
        console.log(materialsalida);

        $scope.UltimoLoteSalida = ultimoLoteSalida;
        $scope.firstEditSeries = firstEdit && cliente !== "";
        $scope.firstEditClientes = firstEdit && cliente!=="";
        $scope.isnuevo = isnuevo;
        $scope.Fkproveedores = cliente;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $scope.verificarmateriales = verificarmateriales;
        $scope.Materialsalida = materialsalida;
        $("#PedidosDesde").prop("readonly", true);
        $("#btnbuscarPedidosDesde").prop("disabled", true);

        $("#PedidosHasta").prop("readonly", true);
        $("#btnbuscarPedidosHasta").prop("disabled", true);
    }

    $scope.Guardarcostes = function () {
        //console.log(message);
        $("#forzarcostesadicionales").submit();
    }







    eventAggregator.RegisterEvent("Fkseries-cv", function (message) {
        //console.log(message);
        $("[name='Tipodealmacenlote']").val(message.Tipodealmacenlote);
    });




    eventAggregator.RegisterEvent("Fkproveedores", function (message) {
        //console.log(message);
        $scope.Fkproveedores = message;
        if (message == null || message === "") {
            $scope.cliente = "";
        }
    });





    eventAggregator.RegisterEvent("transformacionSalidaLinea", function (message) {
        //console.log(message);
        if (message.Tipofamilia == "1") {
            $scope.UltimoLoteSalida = message.Lote;
        }

        $scope.Materialsalida = message.Fkarticulos.substring(2, 5);
    });






    //TE RECOGE LA CUENTA DEL PROVEEDOR + LA DESCRIPCION = $scope.cliente
    eventAggregator.RegisterEvent("Fkproveedores-cv", function (message) {
        $scope.cliente = message.Fkcuentas + "-" + message.Descripcion;
        //console.log($scope.cliente);
            if (!$scope.firstEditClientes) {
                $scope.Fkproveedores = message.Fkcuentas;
          //      console.log($scope.Fkproveedores);
                eventAggregator.Publish("Fktransportista-Buscar", message.Fktransportistahabitual ? message.Fktransportistahabitual : "");
            }
         
            $scope.firstEditClientes = false;
    });








    eventAggregator.RegisterEvent("_nuevalineasalida", function (msg) {
        //console.log(msg),
        $scope.Nuevalinea();
    });








    $scope.Nuevalineasalida = function () {
        console.log("entra nuevo registro");
        if ($("[name='Fkproveedores']").val() && $("[name='Fkproveedores']").val() !== "") {
            $('#_entregastock').on('shown.bs.modal', function () {
                $("#Fkarticulossalida").focus();
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











var articuloActual = null;
var pedirArticulo = function (codigoarticulo, urlarticulos, lineas, setvaloresdefecto) {
    //console.log("Articulo");
    //console.log(codigoarticulo);
    //console.log(urlarticulos);

    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkproveedores']").val(),flujo:'0', fkmonedas: $("[name='Fkmonedas']").val(),fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

        articuloActual = result;

        var escomentario = result.Articulocomentario;
        var cCantidad = lineas.GetEditor("Cantidad");
        var cLote = lineas.GetEditor("Lote");
        var cLargo = lineas.GetEditor("SLargo");
        var cAncho = lineas.GetEditor("SAncho");
        var cGrueso = lineas.GetEditor("SGrueso");
        var cMetros = lineas.GetEditor("SMetros");
        lineas.SetEnabled(true);
        cCantidad.SetEnabled(!escomentario);
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

eventAggregator.RegisterEvent("Fktransportista-cv", function(message) {
    $("[name='Nombretransportista']").val(message.Descripcion);
});

// Ojo. problema del idioma (al no tener id busco por el title y este cambiará si no es español)
function FinalizarHidden() {
    $("a").each(function () {
        if (this.title == "Finalizar") {
            $(this).css("visibility", "hidden");
        }
    })
}