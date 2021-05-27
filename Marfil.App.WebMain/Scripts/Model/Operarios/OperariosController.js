app.controller('OperariosCtrl', ['$scope', '$rootScope', '$http', '$interval',  function($scope, $rootScope, $http, $interval) {

    $scope.ultimabaja = "";
    $scope.ultimaalta = "";
    $scope.dias = "";
    

    $scope.init = function (ultimabaja, ultimaalta) {
        $scope.ultimabaja = ultimabaja;
        $scope.ultimaalta = ultimaalta;
        $(document).ready(function() {
            $scope.calcularDiferencia();
        });
        
    }

    $scope.calcularDiferencia = function () {
        var date1 = Globalize.parseDate($scope.ultimabaja);
        var date2 = Globalize.parseDate($scope.ultimaalta);
        if (date1 != null && date2 != null) {
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            $scope.dias = Math.ceil(timeDiff / (1000 * 3600 * 24));
        } else {
            $scope.dias = 0;
        }
    }

    $scope.$watch('ultimabaja', function (newVal, oldVal) {
        $scope.calcularDiferencia();
    }, true);

    $scope.$watch('ultimaalta', function (newVal, oldVal) {
        $scope.calcularDiferencia();
    }, true);

}]);
