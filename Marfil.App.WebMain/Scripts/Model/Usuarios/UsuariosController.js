app.controller('UsuariosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.email = "";
    $scope.smtp = "";
    $scope.usuariomail="";
    $scope.passwordmail = "";
    $scope.puerto = "";
    $scope.ssl = false;
    $scope.archivohtml = null;
    $scope.nombre = "";
    $scope.urltest = "";
    $scope.resulttest = "";
    $scope.resultflag =undefined;
    $scope.firsttest = false;
    $scope.init = function (email,smtp,usuariomail,passwordmail,puerto,ssl,nombre,urltest)
    {
        $scope.email = email;
        $scope.smtp = smtp;
        $scope.usuariomail = usuariomail;
        $scope.passwordmail = passwordmail;
        $scope.puerto = puerto;
        $scope.nombre = nombre;
        $scope.ssl = ssl;
        $scope.urltest = urltest;
    }

    $scope.$watch("archivohtml", function(a, b) {
        
    });

    $scope.Cambiararchivo=function() {
        
    }

    $scope.Probaremail = function () {


        $scope.resulttest = "Enviando test...";
        $scope.firsttest = true;
        $http({
            url: $scope.urltest,
            method: "GET",
            params: {
                email: $scope.email,
                smtp: $scope.smtp,
                usuario: $scope.usuariomail,
                password: $scope.passwordmail,
                puerto: $scope.puerto,
                nombre: $scope.nombre,
                ssl: $scope.ssl
            }
        })
         .success(function (response) {
             $scope.resulttest = "";
             $scope.resultflag = true;
         }).error(function (data, status, headers, config) {
             $scope.resulttest = "";
             $scope.resultflag = false;
         });
    }

}]);


