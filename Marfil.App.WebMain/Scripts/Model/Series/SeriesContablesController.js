app.controller('SeriesContablesCtrl', ['$scope', '$rootScope', '$http', '$interval', '$window', '$timeout', function ($scope, $rootScope, $http, $interval, $window, $timeout) {

    $scope.tipodocumento = "";
    $scope.codigo = "";
    $scope.selected = "";

    $scope.items = [{ Id: "", Descripcion: "" }];
  
   

    $scope.init = function (codigo, tipodocumento ){
    //$scope.init = function (codigo) {

        $scope.tipodocumento = tipodocumento;
        $scope.codigo = codigo;

    }


    var rellenacod = function (longitud, tipo, codigo) {
        var frellenacod = new FRellenacod();
        var rellenacodService = frellenacod.CreateRellenacod(longitud, tipo);
        return rellenacodService.Formatea(codigo);
    }

    $scope.existePk = function (longitud, tipo, urlApi, urlRedirect) {
        $scope.codigo = rellenacod(longitud, tipo, $scope.codigo);
        if (urlApi)
            $http.get(urlApi + "/" + $scope.tipodocumento + "-" + $scope.codigo)
            //$http.get(urlApi + "/" + $scope.codigo)
                .success(function (response) {
                    if (response.Existe) {
                        var resultValue;
                        bootbox.confirm(Messages.EditarRegistroExistente, function (result) {
                            if (result) {
                                window.location = urlRedirect + "/" + $scope.tipodocumento + "-" + $scope.codigo;
                                //window.location = urlRedirect + "/" + $scope.codigo;
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
