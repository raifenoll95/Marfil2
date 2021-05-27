app.controller('TiposcuentasCtrl', ['$scope', '$rootScope', '$http', '$interval', '$window', '$timeout', function ($scope, $rootScope, $http, $interval, $window, $timeout) {

    $scope.Categoria = "";
    $scope.Cuenta = "";
    $scope.init = function (categoria,cuenta) {
        $scope.Categoria = categoria;
        $scope.Cuenta = cuenta;
    }

    $scope.$watch("Categoria", function (a, b) {
        $("#Cuenta").prop('disabled', a == '1');
    });

     $scope.rellenar =function () {
         $scope.Cuenta = Funciones.RellenarConCaracter($scope.Cuenta, 4,'A').toUpperCase();
     }
}]);