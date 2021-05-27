app.controller('SavedialogCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.usuario = "";
    $scope.usuarioid = "";
    $scope.tipodocumento = "";
    $scope.tipoprivacidad = "";
    $scope.nombre = "";
    $scope.tiporeport = "";
    $scope.url = "";
    $scope.defecto=true;
    $scope.init = function (usuarioid,usuario,tipodocumento, tipoprivacidad, nombre, tiporeport,url) {
        $scope.usuarioid = usuarioid;
        $scope.usuario = usuario;
        $scope.tipodocumento = tipodocumento;
        $scope.tipoprivacidad = tipoprivacidad;
        $scope.nombre = nombre;
        $scope.url = url;
        $scope.tiporeport = tiporeport;
        $scope.defecto=false;
        eventAggregator.RegisterEvent("GuardarDialog",function(msg) {
            $scope.aceptar();
        });

        eventAggregator.RegisterEvent("ExportDialog", function (msg) {
            $scope.exportar(msg);
        });

        eventAggregator.RegisterEvent("ImportDialog", function (msg) {
            $scope.importar(msg);
        });
    }

    $scope.aceptar=function() {
        Reportdesigner.PerformCallback(
            JSON.stringify({defecto:$scope.defecto, usuario: $scope.usuarioid, tipodocumento: $scope.tipodocumento, tiporeport: $scope.tiporeport, tipoprivacidad: $scope.tipoprivacidad, nombre: $scope.nombre }));

    }

    $scope.exportar = function (msg) {
        window.location = msg;
    }

    $scope.importar = function (msg) {
        $("#import").val("");
        $("#import").click();

        var form = document.getElementById('imput-form');
        if (form.attachEvent) {
            form.attachEvent("submit", processForm);
        } else {
            form.addEventListener("submit", processForm);
        }
    }

    $scope.processForm = function (e) {
        alert("SUBMIT");
        if (e.preventDefault) e.preventDefault();

        /* do what you want with the form */

        // You must return false to prevent the default form behavior
        return false;
    }

}]);