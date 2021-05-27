app.controller('GrosoresCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.espesordisco = 0;
    $scope.espesortelar = 0;
    $scope.grosor = 0;
    $scope.descripcion = "";
    $scope.descripcion2 = "";
    $scope.descripcionabreviada = "";
    $scope.coeficientebloque = 0;
    $scope.coeficientetelar = 0;
    $scope.customFunction;
    $scope.codigo = "";

    $scope.init = function (codigo,espesordisco, espesortelar, grosor,descripcion,descripcion2,descripcionabreviada) {
        
        $scope.grosor = grosor;
        $scope.espesordisco = espesordisco;
        $scope.espesortelar = espesortelar;
        $scope.descripcion = descripcion;
        $scope.descripcion2 = descripcion2;
        $scope.descripcionabreviada = descripcionabreviada;
        $scope.codigo = codigo;
    }

    $scope.calcularCoeficiente = function (espesor) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        return (((($scope.grosor) * 100) + espesor) / (2.0 + espesor)).toFixed(3);
    }

    $scope.$watch('grosor', function (newVal, oldVal) {
       
            $scope.coeficientebloque = $scope.calcularCoeficiente($scope.espesordisco);
            $scope.coeficientetelar = $scope.calcularCoeficiente($scope.espesortelar);
           
       
       
    }, true);

    $scope.CalculateNames=function() {
        if ($scope.descripcion === "")
            customFunction($scope, "descripcion");
        if ($scope.descripcion2 === "")
            customFunction($scope, "descripcion2");
        if ($scope.descripcionabreviada === "")
            customFunction($scope, "descripcionabreviada");
    }

  

    

}]);