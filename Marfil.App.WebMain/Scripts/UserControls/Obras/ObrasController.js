app.controller('ObrasCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Fkclientes = "";
    $scope.Fkdirecciones = "";
    $scope.Urldirecciones = "";
    $scope.direcciones = [];

    $scope.init = function (fkclientes, fkdireccion,urldirecciones) {
        $scope.Fkclientes = fkclientes;
        $scope.Fkdirecciones = fkdireccion;
        $scope.Urldirecciones = urldirecciones;
        $scope.direcciones = [];
    }


    $scope.$watch("Fkdirecciones", function(newvalue,oldvalue) {
        if ($("[name='Nombreobra']").val() === "") {
            for (var i = 0; i < $scope.direcciones.length; i++) {
                if ($scope.direcciones[i].Id == newvalue) {
                    $("[name='Nombreobra']").val($scope.direcciones[i].Descripcion);
                }
            }
        }
    });

    eventAggregator.RegisterEvent("Fkclientes-cv", function(message) {
        eventAggregator.Publish("Fkagentes-Buscar", message.Fkcuentasagente ? message.Fkcuentasagente : "");
        eventAggregator.Publish("Fkcomerciales-Buscar", message.Fkcuentascomercial ? message.Fkcuentascomercial : "");
        eventAggregator.Publish("Fkregimeniva-Buscar", message.Fkregimeniva ? message.Fkregimeniva : "");
    });

    eventAggregator.RegisterEvent("Fkclientes", function (message) {

        $http({
            url: $scope.Urldirecciones,
                method: "GET",
                params: { fkentidad: message, tipotercero: 0 }
            })
            .success(function(response) {
                if (response.values.length <= 0) {
                    $scope.Fkdirecciones = "";
                    $scope.direcciones = [];

                } else {
                    $scope.direcciones = response.values;
                    $scope.direcciones.splice(0, 0, { Id: "", Descripcion: "", Defecto: false });
                }
                
            }).error(function(data, status, headers, config) {
                $scope.Fkdirecciones = "";
                $scope.direcciones = [];
            });

    });


}]);

