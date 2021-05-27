app.controller('RetencionCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {
    $scope.porcentajeretenciontiporetencion = "0";

    $scope.init = function (porcentajeretenciontiporetencion) {
        $scope.porcentajeretenciontiporetencion = porcentajeretenciontiporetencion.toLocaleString();
    }
}]);