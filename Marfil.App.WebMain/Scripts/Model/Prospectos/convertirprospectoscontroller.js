

app.controller('ConvertirProspectosClientesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    
    
    $scope.Model = new EmailModel();
    $scope.Enviando = false;
    $scope.Url = "";
    $scope.ResultadoEnvio = 2;
    $scope.Digitoscuentas = 8;
    $scope.Enableaceptar = false;
    $scope.Error = "";
    $scope.init = function (url,digitoscuenta) {
        $scope.Url = url;
        $scope.Digitoscuentas = digitoscuenta;
    }

    eventAggregator.RegisterEvent("Mostrarconvertirprospectocliente", function (message) {
        $scope.Error = "";
        $scope.Enableaceptar = false;
        $scope.Model = message;
        $scope.Enviando = false;
        $scope.ResultadoEnvio = 2;
        $scope.Subiendoarchivo = false;
        $("#procesoconvertirprospectocliente").modal();
    });

    eventAggregator.RegisterEvent("ClienteId", function (message) {
        $scope.Model.ClienteId = message;
        $scope.Enableaceptar = message != "";
    });


    

    $scope.validarEnvio=function() {
        var result = true;
        $scope.Model.ClienteIderror = "";
        if ($scope.Model.ClienteId == "") {
            $scope.Model.ClienteIderror = "Es obligatorio indicar un cliente";
            result = false;
        }

       
        return result;
    }

    $scope.Send = function () {

        //TODO EL: Revisar porque con el * no funciona la conversion
        if ($scope.validarEnvio()) {
            $scope.Enviando = true;
            $scope.ResultadoEnvio = 2;
            

            $.post($scope.Url, $scope.Model).success(function (message) {

                $("#procesoconvertirprospectocliente").modal('hide');
                $scope.Enviando = false;
                messagesService.show(TipoMensaje.Exito, "Bien!", "El cliente se ha creado correctamente");
                eventAggregator.Publish("Fkclientes-Buscar", message.ClienteId);
                
            }).error(function (jqXHR, textStatus, errorThrown) {
                $scope.Enviando = false;
                $scope.ResultadoEnvio = 0;
                $scope.Error = jqXHR.responseJSON;
                $scope.$apply();
            });
        }
    }



    

}]);

