app.controller('ListadosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {
    $scope.urlSaveSettings = "";

    $scope.init = function (urlSaveSettings) {
        $scope.urlSaveSettings = urlSaveSettings;
    }

    $scope.SaveSettings=function() {
        $.get($scope.urlSaveSettings,{nombre:'Defecto'}).success(function (message) {
            Funciones.ShowSuccessMessage("Preferencia guardada correctamente");
        }).error(function (jqXHR, textStatus, errorThrown) {
            Funciones.ShowErrorMessage(textStatus);
        });

    }
}]);