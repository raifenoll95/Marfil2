app.controller('PuertosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {
   
    $scope.$watch('Id', function (newVal, oldVal) {
        $scope.level = newVal.length > $scope.maxLevel ? 0 : newVal.length;//TODO EL: Review maximum level, is parametrized

    }, true);

    $scope.Fkpaises = "";
    $scope.Id = "";

    $scope.init = function (urlpuertos, paisdefecto,id) {
        $scope.urlpuertos = urlpuertos;
        $scope.Fkpaises = paisdefecto;
        $scope.Id = id;
    }

    $scope.$watch("Id", function() {
        $("#idhidden").val($scope.Id);
    });

    $scope.$watch('Fkpaises', function () {
        $http({
            url: $scope.urlpuertos,
            method: "GET",
            params: { codigopais: $scope.Fkpaises }
        })
          .success(function (response) {
              $scope.puertos = response.values;
                $scope.puertos.splice(0,0, { Id: "", Descripcion: "" });
            }).error(function (data, status, headers, config) {
                $scope.Id = "";
                $scope.puertos = [];
              
          });
    });

}]);