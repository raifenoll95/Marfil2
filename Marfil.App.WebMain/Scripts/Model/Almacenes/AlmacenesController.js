app.controller('AlmacenesCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Fkalmacen = "0";
    $scope.Fkzonaalmacen = "0";
    $scope.Urlzonasalmacen = "";
    $scope.Zonas = [];

    $scope.init = function (url,fkalmacen,fkzonaalmacen) {
        $scope.Fkalmacen = fkalmacen;
        //$scope.Cargarzonas();
        $scope.Fkzonaalmacen = fkzonaalmacen;
        $scope.Urlzonasalmacen = url;
    }
    
    $scope.Cargarzonas =function() {
        $http( {url: $scope.Urlzonasalmacen, method: "GET", params: {fkalmacen:$scope.Fkalmacen}} ).success(function (response) {
                   if (response.values.length > 0) {
                       $scope.Zonas = response.values;
                       $scope.Zonas.splice(0, 0, { Id: "", Descripcion: ""});
                       if (!$scope.Fkzonaalmacen) {
                               $scope.Fkzonaalmacen = result[0].Id;
                       }
                   } else {
                       $scope.Fkzonaalmacen = "";
                       $scope.Zonas = [{ Id: "", Descripcion: "" }];
                   }

               }).error(function (data, status, headers, config) {
                   $scope.Zonas = [];
                   $scope.Fkzonaalmacen = "";
            });
    }

    eventAggregator.RegisterEvent("Fkalmacen", function (message) {
        $scope.Fkalmacen = message;
        $scope.Cargarzonas();
    });
}]);