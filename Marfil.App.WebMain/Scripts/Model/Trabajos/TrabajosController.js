app.controller('TrabajosCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Tipotrabajo = "0";
    $scope.Tipoimputacion = "0";

    $scope.init = function (tipotrabajo,tipoimputacion) {
        $scope.Tipotrabajo = tipotrabajo;
        $scope.Tipoimputacion = tipoimputacion;
    }

    $scope.$watch("Tipotrabajo", function (old, newvalue) {
        if ($scope.Tipotrabajo == "1") {
            $scope.Tipoimputacion = "1";
        }
    });
}]);