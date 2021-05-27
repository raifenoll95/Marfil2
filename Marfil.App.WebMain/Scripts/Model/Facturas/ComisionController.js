app.controller('ComisionCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.netobasecomision = "0";
    $scope.porcentajecomisionagente = "0";
    $scope.importecomisionagente = "0";
    $scope.porcentajecomisioncomercial = "0";
    $scope.importecomisioncomercial = "0";
    $scope.decimalesmonedas = "0";
    

    $scope.init = function (netobasecomision, porcentajecomsionagente, importecomisionagente,porcentajecomsioncomercial,importecomisioncomercial,decimalesmonedas) {
        
        $scope.netobasecomision = netobasecomision.toLocaleString();
        $scope.porcentajecomisionagente = porcentajecomsionagente.toLocaleString();
        $scope.importecomisionagente = importecomisionagente.toLocaleString();
        $scope.porcentajecomisioncomercial = porcentajecomsioncomercial.toLocaleString();
        $scope.importecomisioncomercial = importecomisioncomercial.toLocaleString();
        $scope.decimalesmonedas = decimalesmonedas;
    }

    $scope.$watch('porcentajecomisionagente', function (newVal, oldVal) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        $scope.importecomisionagente = (+(Globalize.parseFloat($scope.netobasecomision.toLocaleString()) * (Globalize.parseFloat($scope.porcentajecomisionagente.toLocaleString())) / 100.0).toFixed($scope.decimalesmonedas)).toLocaleString();

    }, true);

    $scope.$watch('porcentajecomisioncomercial', function (newVal, oldVal) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        $scope.importecomisioncomercial = (+(Globalize.parseFloat($scope.netobasecomision.toLocaleString()) * (Globalize.parseFloat($scope.porcentajecomisioncomercial.toLocaleString())) / 100.0).toFixed($scope.decimalesmonedas)).toLocaleString();

    }, true);

    

}]);