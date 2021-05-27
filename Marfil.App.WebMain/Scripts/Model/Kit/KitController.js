app.controller('KitCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {
    
    

    $scope.Buscarlotes = function() {
        $('#_buscarlotes').on('shown.bs.modal', function () {
            //$("#FkarticulosDesde").focus();
        });
        $("#_buscarlotes").modal('show');
    }

    eventAggregator.RegisterEvent("buscarlote",function(msg) {
        $scope.Buscarlotes();
    });
}]);