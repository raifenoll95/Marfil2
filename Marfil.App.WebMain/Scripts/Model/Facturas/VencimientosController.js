app.controller('VencimientosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.urlApiBancos;
    $scope.Fkclientes;
    $scope.bancoscliente;
    $scope.fkbancosmandatos;

    $scope.init = function (urlApiBancos,fkclientes)
    {
        $scope.urlApiBancos = urlApiBancos;
        $scope.Fkclientes = fkclientes;

        $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkclientes)
                .success(function (response) {
                    if (response.values.length > 0) {
                        $scope.bancoscliente = response.values;

                        //Devuelve un array
                        for (var i = 0; i < $scope.bancoscliente.length; i++) {
                            $scope.bancoscliente[i].Descripcion = $scope.bancoscliente[i].Descripcion + ", " + $scope.bancoscliente[i].Idmandato + ", " + $scope.bancoscliente[i].Tipoadeudocadena + ", " + $scope.bancoscliente[i].Esquemacadena;
                        }
                        $scope.bancoscliente.splice(0, 0, { Id: "", Descripcion: "", Defecto: false });
                        if (!$scope.fkbancosmandatos && $scope.fkbancosmandatos != "") {
                            var result = $.grep($scope.bancoscliente, function (e) { return e.Defecto == true; });
                            if (result.length)
                                $scope.fkbancosmandatos = result[0].Id;
                        }
                    }
                    else {
                        $scope.fkbancosmandatos = "";
                        $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
                    }
                }).error(function (data, status, headers, config) {
                    console.log("error call urlApiBancos")
                });
    }
}]);