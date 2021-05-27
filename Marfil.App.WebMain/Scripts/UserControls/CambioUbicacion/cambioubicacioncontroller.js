app.controller('CambioubicacionCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    $scope.Fkalmacen = "";
    $scope.Fkzonaalmacen = 0;
    $scope.Lote = "";
    $scope.Urllotes = "";
    $scope.Urlzonasalmacen = "";

    $scope.Lotecargado = false;
    $scope.Descripcion = "";
    $scope.Cantidad = 0;
    $scope.Largo = 0;
    $scope.Ancho = 0;
    $scope.Grueso = 0;
    $scope.Fkzonaactual = "";
    $scope.Fkzonaactualdescripcion = "";
    $scope.Zonas = [];

    eventAggregator.RegisterEvent("Fkalmacen", function (msg) {
        $scope.Cargarzonas(msg);
    });

    $scope.Init = function(urllote,urlzonas) {
        $scope.Urllotes = urllote;
        $scope.Urlzonasalmacen = urlzonas;
    }

    $scope.Cargarzonas = function (fkalmacen) {
        var datos = {
            Fkalmacen: fkalmacen
        };

        $http({
            url: $scope.Urlzonasalmacen, method: "GET", params: datos
        })
          .success(function (response) {
              $scope.Zonas = response.values;
             
                $scope.Fkzonaalmacen = 0;
            }).error(function (data, status, headers, config) {
                $scope.Zonas = [];
                $scope.Fkzonaalmacen = 0;
          });
    }

    $scope.Comprobarlote = function () {
        var datos = {
            Referencialote: $scope.Lote,
            Fkalmacen: $("[name='Fkalmacen'").val()
        };

        $http({
            url: $scope.Urllotes, method: "GET", params:datos
        })
          .success(function (response) {
              $scope.Lotecargado = true;
              $scope.Descripcion = response.Descripcion;
              $scope.Fkzonaactual = response.Fkzonaalmacen;
              $scope.Cantidad =response.Cantidad;
              $scope.Largo = Funciones.RedondearGlobalize(response.Largo, response.Decimalesmedidas);
              $scope.Ancho = Funciones.RedondearGlobalize(response.Ancho, response.Decimalesmedidas);
              $scope.Grueso = Funciones.RedondearGlobalize(response.Grueso, response.Decimalesmedidas);
              
            $("[name='Fkzonaalmacen']").focus();
              eventAggregator.Publish("Fkzonaactual-Buscar", $scope.Fkzonaactual);
          }).error(function (data, status, headers, config) {
              $scope.Lotecargado = false;
              messagesService.show(TipoMensaje.Error,"Ups!","Ocurrió un problema con el lote "+ $scope.Lote);
            });
    }

    //end api facturar
}]);


