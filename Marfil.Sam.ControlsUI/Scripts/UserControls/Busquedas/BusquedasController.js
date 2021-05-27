app.controller('BusquedasCtrl', ['$scope','$rootScope', '$http', '$interval', 'uiGridConstants', function ($scope, $rootScope,$http, $interval, uiGridConstants) {

    
    $rootScope.$on("CallFormSearch", function (event, data) {
        $('#' + data.obj.IdFormulariomodal).modal('show');
        $scope.load(data.obj.campoIdentificador, data.obj.IdComponenteasociado, data.obj.IdFormulariomodal, data.obj.Url);
        });
    //configuracion Control
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";

    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.gridOptions.multiSelect = false;
    $scope.gridOptions.modifierKeysToMultiSelect = false;
    $scope.gridOptions.noUnselect = true;
    $scope.gridOptions.enableFiltering = true;
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;

        });

    };
    $scope.gridOptions.rowTemplate = '<div ng-keypress="alert(\'aaaaa\');" ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function(row) {
            $scope.selected = row;
            $scope.aceptar();
            $('#'+$scope.IdFormulariomodal).modal('hide');
        },
        onKeyDown: function() {
            alert("pasa");
        }
    };
    $scope.aceptar = function () {
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscar", $scope.selected.entity[$scope.campoIdentificador]);
    };

    $scope.cancelar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        $('#' + $scope.IdComponenteasociado).focus();
    };

    $scope.loading = true;
    $scope.load = function (campoIdentificador,IdComponenteasociado,IdFormulariomodal,Url) {
        $scope.loading = true;

        $scope.campoIdentificador = campoIdentificador;
        $scope.IdComponenteasociado = IdComponenteasociado;
        $scope.IdFormulariomodal = IdFormulariomodal;
        $scope.Url = Url;

        $http.get($scope.Url)
            .success(function (response) {
                $scope.selected = null;
                $scope.gridApi.grid.clearAllFilters();
                $scope.loading = false;
                $scope.gridOptions.columnDefs = response.columns;
                $scope.gridOptions.data = response.values;
                $scope.gridApi.core.refresh();
                $scope.gridApi.core.handleWindowResize();

            }).error(function (data, status, headers, config) {

                alert(data);
            });
    }

}]);

app.controller('BotonbuscarCtrl', [
    '$scope', '$rootScope', function($scope, $rootScope) {
        $scope.buscar = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url) {
            var obj = {
                campoIdentificador: campoIdentificador,
                IdComponenteasociado: IdComponenteasociado,
                IdFormulariomodal: IdFormulariomodal,
                Url: Url
            }

            $rootScope.$emit("CallFormSearch", { obj });
        }
    }
]);