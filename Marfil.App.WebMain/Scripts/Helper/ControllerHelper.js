//Raimundo Fenoll Albaladejo. Funciona pero esta hecho un poco lio con los Movimientos Contables pero tuve que hacer esto porque la cantidad de registers, publish,
//functions y demas que estaban hechas en MovsLin no hay quien lo entienda y mas sin comentarios!!!

app.controller('HelperCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.urlDigitosEmpresa;
    $scope.urlExisteCuenta;
    $scope.digitosCuentaEmpresa;
    $scope.IdCompletoCuenta;
    $scope.modeloCuenta;

    $scope.init = function (urlDigitos, urlExisteCuenta) {
        $scope.urlDigitosEmpresa = urlDigitos;
        $scope.urlExisteCuenta = urlExisteCuenta;
        CalculaDigitosEmpresa();
    }

    //Nos rellena conn 0 el numero de la cuenta introducida (41,2)
    var rellenacod = function (longitud, tipo, codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }

    //Numero de digitos que admite la empresa
    var CalculaDigitosEmpresa = function () {

        $http.get($scope.urlDigitosEmpresa).success(function (data) {
            $scope.digitosCuentaEmpresa = JSON.parse(data);
        }).error(function (error) {
            console.log("error call controller HelperView");
        });
    }

    //Obtiene el modelo de una cuenta a partir de un ID
    var obtenerModeloCuenta = function (numeroCuenta) {

        $http.get($scope.urlExisteCuenta + "?input=" + numeroCuenta).success(function (modeloCuenta) {
            if (JSON.parse(modeloCuenta) == "") {
                $("[name='GridViewTercero$DXEditor1']").val("");
                $("[name='GridViewTercero$DXEditor2']").val("");
            }

            else {
                $scope.modeloCuenta = JSON.parse(modeloCuenta);
                $("[name='GridViewTercero$DXEditor1']").val($scope.modeloCuenta.Id);
                $("[name='GridViewTercero$DXEditor1']").attr("disabled", true); //num cuenta disabled
                $("[name='GridViewTercero$DXEditor2']").val($scope.modeloCuenta.Descripcion);
                $("[name='GridViewTercero$DXEditor2']").attr("disabled", true); //descripcion cuenta disabled
            }
        }).error(function (error) {
            console.log("error call controller comprobar cuenta");
        });
    }
    
    //Nos suscribimos para obtener el input que ha publicado el cshtml
    eventAggregator.RegisterEvent("InputCodCuenta", function (message) {

        //Obtenemos el numero de la cuenta, si el input es 41.2, NumeroTotalCuenta sera 41000002
        $scope.IdCompletoCuenta = rellenacod($scope.digitosCuentaEmpresa, 0, message);
        obtenerModeloCuenta($scope.IdCompletoCuenta);
        
    });

    //Busquedas Controller ha publicado el numero de la cuenta al pinchar sobre la cuenta seleccionada
    eventAggregator.RegisterEvent("CodTercero-Buscarfocus", function (obj) {
        obtenerModeloCuenta(obj);
    });
}]);




//------------------------------------------------------------------------------------------------------------------------------




app.controller('HelperCtrl4Movs', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.urlDigitosEmpresa;
    $scope.urlExisteCuenta;
    $scope.urlCuentas;
    $scope.digitosCuentaEmpresa;
    $scope.IdCompletoCuenta;
    $scope.modeloCuenta;

    $scope.init = function (urlDigitos, urlExisteCuenta, urlCuentas) {
        $scope.urlDigitosEmpresa = urlDigitos;
        $scope.urlExisteCuenta = urlExisteCuenta;
        $scope.urlCuentas = urlCuentas;
        CalculaDigitosEmpresa();
    }

    //Nos rellena conn 0 el numero de la cuenta introducida (41,2)
    var rellenacod = function (longitud, tipo, codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }

    //Numero de digitos que admite la empresa
    var CalculaDigitosEmpresa = function () {

        $http.get($scope.urlDigitosEmpresa).success(function (data) {
            $scope.digitosCuentaEmpresa = JSON.parse(data);
        }).error(function (error) {
            console.log("error call controller HelperView");
        });
    }

    //Obtiene el modelo de una cuenta a partir de un ID
    var obtenerModeloCuenta = function (numeroCuenta) {

        $http.get($scope.urlExisteCuenta + "?input=" + numeroCuenta).success(function (modeloCuenta) {
            if (JSON.parse(modeloCuenta) == "") {
                $("#maesfkcuentas").val("");
                $("#maesdescripcion").val("");
                $("#maesdebe").val(0);
                $("#maeshaber").val(0);
                $("#maessaldo").val(0);
            }

            else {
                $scope.modeloCuenta = JSON.parse(modeloCuenta);
                mostrarCuenta($scope.modeloCuenta.Id, $scope.urlCuentas, GridViewLineas);
            }
        }).error(function (error) {
            console.log("error call controller comprobar cuenta");
        });
    }

    //Te muestra abajo los detalles de la cuenta (Cuenta, Descripcion, debe, haber, Saldo)
    var mostrarCuenta = function (codigocuenta, urlcuentas, lineas) {
        lineas.SetEnabled(true);

        $("#maesfkcuentas").val("");
        $("#maesdescripcion").val("");
        $("#maesdebe").val(0);
        $("#maeshaber").val(0);
        $("#maessaldo").val(0);

        if (codigocuenta != null) {
            $.get(urlcuentas + "?id=" + codigocuenta).success(function (result) {
                Fkcuentas.SetValue(codigocuenta);
                $("#maesfkcuentas").val(codigocuenta);
                $("#maesdescripcion").val(result.Descripcion);
                $("#maesdebe").val(result.SDebe)
                $("#maeshaber").val(result.SHaber);
                $("#maessaldo").val(result.SSaldo);
            }).error(function (jqXHR, textStatus, errorThrown) {
                console.log("error mostrarCuenta");
                Fkcuentas.SetValue("");
                Fkcuentas.Focus();
            })
        };
    }

    //Nos suscribimos para obtener el input que ha publicado el cshtml
    eventAggregator.RegisterEvent("InputCodCuenta", function (message) {

        if (message == "") {
            $("#maesfkcuentas").val("");
            $("#maesdescripcion").val("");
            $("#maesdebe").val(0);
            $("#maeshaber").val(0);
            $("#maessaldo").val(0);
        }

        //Obtenemos el numero de la cuenta, si el input es 41.2, NumeroTotalCuenta sera 41000002
        $scope.IdCompletoCuenta = rellenacod($scope.digitosCuentaEmpresa, 0, message);
        obtenerModeloCuenta($scope.IdCompletoCuenta);
    });
}]);



//------------------------------------------------------------------------------------------------------------------------------


app.controller('HelperCtrl4Campanyas', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.urlDigitosEmpresa;
    $scope.urlExisteCuenta;
    $scope.digitosCuentaEmpresa;
    $scope.IdCompletoCuenta;
    $scope.modeloCuenta;

    $scope.init = function (urlDigitos, urlExisteCuenta) {
        $scope.urlDigitosEmpresa = urlDigitos;
        $scope.urlExisteCuenta = urlExisteCuenta;
        CalculaDigitosEmpresa();
    }

    //Nos rellena conn 0 el numero de la cuenta introducida (41,2)
    var rellenacod = function (longitud, tipo, codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }

    //Numero de digitos que admite la empresa
    var CalculaDigitosEmpresa = function () {

        $http.get($scope.urlDigitosEmpresa).success(function (data) {
            $scope.digitosCuentaEmpresa = JSON.parse(data);
        }).error(function (error) {
            console.log("error call controller HelperView");
        });
    }

    //Obtiene el modelo de una cuenta a partir de un ID
    var obtenerModeloCuenta = function (numeroCuenta) {

        $http.get($scope.urlExisteCuenta + "?input=" + numeroCuenta).success(function (modeloCuenta) {
            $scope.modeloCuenta = JSON.parse(modeloCuenta);

            GridViewTercero.GetEditor("Codtercero").SetValue($scope.modeloCuenta.Fkentidad);
            GridViewTercero.GetEditor("Descripciontercero").SetValue($scope.modeloCuenta.Descripcion);
            GridViewTercero.GetEditor("Poblacion").SetValue($scope.modeloCuenta.Poblacion);
            GridViewTercero.GetEditor("Fkprovincia").SetValue($scope.modeloCuenta.Provincia);
            GridViewTercero.GetEditor("Fkpais").SetValue($scope.modeloCuenta.Pais);
            GridViewTercero.GetEditor("Email").SetValue($scope.modeloCuenta.Email);
            GridViewTercero.GetEditor("Telefono").SetValue($scope.modeloCuenta.Telefono);

            GridViewTercero.GetEditor("Descripciontercero").SetEnabled(false);
            GridViewTercero.GetEditor("Poblacion").SetEnabled(false);
            GridViewTercero.GetEditor("Fkprovincia").SetEnabled(false);
            GridViewTercero.GetEditor("Fkpais").SetEnabled(false);
            GridViewTercero.GetEditor("Email").SetEnabled(false);
            GridViewTercero.GetEditor("Telefono").SetEnabled(false);

        }).error(function (error) {
            console.log("error call controller comprobar cuenta");
        });
    }

    //Nos suscribimos para obtener el input que ha publicado el cshtml
    eventAggregator.RegisterEvent("InputCodCuenta", function (message) {

        //Obtenemos el numero de la cuenta, si el input es 41.2, NumeroTotalCuenta sera 41000002
        $scope.IdCompletoCuenta = rellenacod($scope.digitosCuentaEmpresa, 0, message);
        obtenerModeloCuenta($scope.IdCompletoCuenta);

    });

    //Busquedas Controller ha publicado el numero de la cuenta al pinchar sobre la cuenta seleccionada
    eventAggregator.RegisterEvent("CodTercero-Buscarfocus", function (obj) {
        obtenerModeloCuenta(obj);
    });
}]);



//------------------------------------------------------------------------------------------------------------------------------

app.controller('HelperCtrl4SituacionesTesoreria', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.urlCobro;
    $scope.urlPago;

    $scope.init = function (urlCobro, urlPago) {
        $scope.urlCobro = urlCobro;
        $scope.urlPago = urlPago;
    }

    $('#Valorinicialcobros').click(function () {    
        if ($('#Valorinicialcobros').prop("checked")) {
            $http.get($scope.urlCobro).success(function (data) {
                if (JSON.parse(data) == "cobro") {
                    bootbox.alert("Ya existe una situación inicial de cobro");
                    $('#Valorinicialcobros').attr("checked", false);
                }
            }).error(function (error) {
                console.log(error);
            });
        }
    });

    $('#Valorinicialpagos').click(function () {
        if ($('#Valorinicialpagos').prop("checked")) {
            $http.get($scope.urlPago).success(function (data) {
                if (JSON.parse(data) == "pago") {
                    bootbox.alert("Ya existe una situación inicial de pago");
                    $('#Valorinicialpagos').attr("checked", false);
                }
            }).error(function (error) {
                console.log(error);
            });
        }
    });
    
}]);


//------------------------------------------------------------------------------------------------------------------------------

app.controller('HelperCtrl4CircuitosTesoreria', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.urlDigiditosEmpresa;
    $scope.digitos;

    $scope.init = function (urlDigiditosEmpresa) {
        $scope.urlDigiditosEmpresa = urlDigiditosEmpresa;

        $http.get($scope.urlDigiditosEmpresa).success(function (data) {
            $scope.digitos = JSON.parse(data);
            console.log("digitos:" + $scope.digitos);
        }).error(function (error) {
            console.log("error call controller HelperView");
        });
    }

    //Llega lo que ha introducido el usuario
    eventAggregator.RegisterEvent("InputCodCuentaCargo1", function (message) {

        if (message.length > $scope.digitos) {
            $("#Cuentacargo1").val("");
            bootbox.alert("El número de cuenta Cargo 1 supera la longitud de las cuentas definidas para la empresa");
        }
    });

    //Llega lo que ha introducido el usuario
    eventAggregator.RegisterEvent("InputCodCuentaAbono1", function (message) {

        if (message.length > $scope.digitos) {
            $("#Cuentaabono1").val("");
            bootbox.alert("El número de cuenta Abono 1 supera la longitud de las cuentas definidas para la empresa");
        }
    });
}]);


//------------------------------------------------------------------------------------------------------------------------------
