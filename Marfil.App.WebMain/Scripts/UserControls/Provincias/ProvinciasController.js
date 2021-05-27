app.controller('ProvinciasCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Fkpais = "";
    $scope.Fkprovincias = "";
    $scope.provincias = [];
    $scope.Urlprovincias = "";
    

    $scope.init = function (urlprovincias, fkpaisdefecto,fkprovincias) {
        
        $scope.Fkpais = fkpaisdefecto;
        $scope.Fkprovincia = fkprovincias;
        $scope.provincias = [];
        
        $scope.Urlprovincias = urlprovincias;
    }

    $scope.$watch('Fkpais', function () {
        $http({
            url: $scope.Urlprovincias,
            method: "GET",
            params: { codigopais: $scope.Fkpais }
        })
          .success(function (response) {
              if (response.values.length <=0) {
                  $scope.Fkprovincia = "";
              }
              $scope.provincias = response.values;
              $scope.provincias.splice(0, 0, { Id: "", Descripcion: "" });
            }).error(function (data, status, headers, config) {
              $scope.Fkprovincia = "";
              $scope.provincias = [];
          });
    });

}]);

