app.controller('TareasCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Imputacion = "0";
    $scope.Unidad = "0";

    $scope.init = function (imputacion,unidad) {
        $scope.Imputacion = imputacion;
        $scope.Unidad = unidad;
    }

    //$scope.$watch("Tipotrabajo", function (old,newvalue) {
    //    if ($scope.Tipotrabajo == "1") {
    //        $scope.Tipoimputacion = "1";
    //    }
    //});

    
   
}]);