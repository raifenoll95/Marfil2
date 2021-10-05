app.controller('TransformacionesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    
    $scope.isnuevo = false;
    $scope.fkbancosmandatos = "";
    $scope.Fkproveedores = "";
    $scope.cliente = "";
    $scope.UltimoLoteSalida = "";
    $scope.urlDescargaDefecto = "";
    $scope.Acabadodesde = "";
    
    $scope.init = function (firstEdit, cliente, isnuevo,  urlDescargaDefecto)
    {
        $scope.firstEditSeries = firstEdit && cliente !== "";
        $scope.firstEditClientes = firstEdit && cliente!=="";
        $scope.isnuevo = isnuevo;
        $scope.Fkproveedores = cliente;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $("#PedidosDesde").prop("readonly", true);
        $("#btnbuscarPedidosDesde").prop("disabled", true);

        $("#PedidosHasta").prop("readonly", true);
        $("#btnbuscarPedidosHasta").prop("disabled", true);
    
    }

  
    eventAggregator.RegisterEvent("Fkseries-cv", function (message) {
        
    });

    eventAggregator.RegisterEvent("Fkproveedores", function (message) {
        $scope.Fkproveedores = message;
        //debugger;
        console.log("abbbbbb");
        //debugger;
        if (message == null || message === "") {
            $scope.cliente = "";
        }
    });

    eventAggregator.RegisterEvent("transformacionSalidaLinea", function (message) {
        if (message.Tipofamilia == "1") {
            $scope.UltimoLoteSalida = message.Lote;
            $scope.Materialsalida = message.Material;
        }
    });

    eventAggregator.RegisterEvent("Fkproveedores-cv", function (message) {
        $scope.cliente = message.Fkcuentas + "-" + message.Descripcion;
            if (!$scope.firstEditClientes) {
                $scope.Fkproveedores = message.Fkcuentas;
                eventAggregator.Publish("Fktransportista-Buscar", message.Fktransportistahabitual ? message.Fktransportistahabitual : "");
            }
         
            $scope.firstEditClientes = false;
    });

    $scope.Guardarcostes = function () {
        $("#forzarcostesadicionales").submit();
    }

    eventAggregator.RegisterEvent("Fktrabajos-cv", function (message) {
        $scope.Acabadodesde = message.Fkacabadoinicial;
        $("#FkAcabadoDesde").val($scope.Acabadodesde);
        $("#FkAcabadoHasta").val($scope.Acabadodesde);
    });

    eventAggregator.RegisterEvent("_nuevalineasalida", function (msg) {
        $scope.Nuevalinea();
    });

    $scope.Nuevalineasalida = function () {

        console.log("nueva linea de salida");

        if ($("#Fktrabajos").val() == "") {
            bootbox.alert("Es obligatorio indicar un Trabajo");
            return;
        }

        if ($("[name='Fkproveedores']").val() && $("[name='Fkproveedores']").val() !== "") {
            $('#_entregastock').on('shown.bs.modal', function () {
                $("#Fkarticulosentrada").focus();
            });

            $("#_entregastock").modal('show');
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
      
        $("#_entradastock").modal('show');
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
var pedirArticulo=function(codigoarticulo,urlarticulos,lineas,setvaloresdefecto) {
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
