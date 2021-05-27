app.controller('MainpageCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.MainFormIsEnable = true;

    mainControllerAggregator.OnChange.on(function()
    {
        $scope.MainFormIsEnable = !mainControllerAggregator.ExistsElement();
    });

}]);





