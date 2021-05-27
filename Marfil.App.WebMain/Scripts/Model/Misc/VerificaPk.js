

app.controller('VerificaPk', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {
    $scope.codigo = "";

    var rellenacod = function (longitud,tipo,codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }
    $scope.existeBundleCustomPk = function (longitud, tipo, urlApi, urlRedirect) {
        //TODO formatear codigo


        $scope.codigo = rellenacod(longitud, tipo, $scope.codigo);
        $scope.customId = $("[name='Lote']").val() +";"+$scope.codigo;

        if (urlApi)
            $http.get(urlApi + "/" + $scope.customId)
                .success(function (response) {
                    if (response.Existe) {
                        var resultValue;
                        bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                            if (result) {
                                window.location = urlRedirect + "/" + $scope.customId;
                            } else {
                                $timeout(function () {
                                    var element = window.document.getElementById("idcontrol");
                                    if (element)
                                        element.focus();
                                    location.reload(true);
                                });
                            }
                        });


                    }
                }).error(function (data, status, headers, config) {

                });
    }
    $scope.existeCustomPk = function (pais,longitud, tipo, urlApi, urlRedirect) {
        //TODO formatear codigo


        $scope.codigo = rellenacod(longitud, tipo, $scope.codigo);
        $scope.customId = pais + '-' + $scope.codigo;

        if (urlApi)
            $http.get(urlApi + "/" + $scope.customId)
                .success(function (response) {
                    if (response.Existe) {
                        var resultValue;
                        bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                            if (result) {
                                window.location = urlRedirect + "/" + $scope.customId;
                            } else {
                                $timeout(function () {
                                    var element = window.document.getElementById("idcontrol");
                                    if (element)
                                        element.focus();
                                    location.reload(true);
                                });
                            }
                        });


                    }
                }).error(function (data, status, headers, config) {
                   
                });
    }
    $scope.existePk = function (longitud,tipo, urlApi, urlRedirect) {
        //TODO formatear codigo


        $scope.codigo = rellenacod(longitud,tipo,$scope.codigo);
        if (urlApi)
        $http.get(urlApi + "/" + $scope.codigo)
            .success(function (response) {
                if (response.Existe) {
                    var resultValue;
                    bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                        if (result) {
                            window.location = urlRedirect + "/" + $scope.codigo;
                        } else {
                            $timeout(function () {
                                var element = window.document.getElementById("idcontrol");
                                if (element)
                                    element.focus();
                                location.reload(true);
                            });
                        }
                    });


                }
            }).error(function (data, status, headers, config) {
              
            });
    }
}]);