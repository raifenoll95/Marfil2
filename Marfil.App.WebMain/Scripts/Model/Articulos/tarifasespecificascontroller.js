app.controller('TarifasespecificasCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.articulo = "";
    $scope.urltarifas = "";
    $scope.tipoflujo = 0;
    $scope.tarifas = [];
    $scope.init = function (urltarifas, articulo, tipoflujo) {
        $scope.urltarifas = urlpuertos;
        $scope.articulo = articulo;
        $scope.tipoflujo = tipoflujo;
    }

    $scope.$watch('articulo', function () {
        $http({
            url: $scope.urltarifas,
            method: "GET",
            params: { tipoflujo: $scope.tipoflujo, articulo: $scope.articulo }
        })
          .success(function (response) {
              $scope.tarifas  = response;
          }).error(function (data, status, headers, config) {
              $scope.tarifas = [];
          });
    });

}]);