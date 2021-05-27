app.controller('BundleCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {
    
    

    $scope.Buscarlotes = function() {

        if ($("[name='Lote']").val() != "") {
            $('#_buscarlotes').on('shown.bs.modal', function () {
                eventAggregator.Publish("Realizarbusquedalotesbundle", "");
            });

            $("#_buscarlotes").modal('show');
        }
        
    }

    eventAggregator.RegisterEvent("buscarlote",function(msg) {
        $scope.Buscarlotes();
    });
}]);