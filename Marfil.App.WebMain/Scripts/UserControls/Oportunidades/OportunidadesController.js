app.controller('OportunidadesCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.init = function () {
    }


    $scope.ver = function (id) {
        var item = $scope.searchItem(empresa, tipotercero, fkentidad, id);
        $scope.nuevo = $.parseJSON(JSON.stringify(item));;
        $("#" + $scope.controlId).modal();
    }

}]);

