app.controller('LoteCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Urllotes = "";
    $scope.Urlagregarlotes = "";

    $scope.loading = true;
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true };
    $scope.gridOptions.multiSelect = true;
    $scope.gridOptions.modifierKeysToMultiSelect = true;
    $scope.gridOptions.noUnselect = false;
    $scope.gridOptions.selectionRowHeaderWidth = 35;
    $scope.gridOptions.enableFiltering = true;
    $scope.Errorgeneral = "";
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = $scope.gridApi.selection.getSelectedCount();
           
        });
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.selected = $scope.gridApi.selection.getSelectedCount();
        });
    };

    $scope.gridOptions.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
        }

    };

    $scope.init = function (url, urlagregarlotes) {
        $scope.Urllotes = url;
        $scope.Urlagregarlotes = urlagregarlotes;
    }

    $scope.RealizarBusqueda = function () {
        $scope.loading = true;
        var parametros = {
            Fkalmacen: $("[name='Fkalmacen']").val(),
            FkarticulosDesde: $("[name='FkarticulosDesde']").val(),
            FkarticulosHasta: $("[name='FkarticulosHasta']").val(),
            FkfamiliaDesde: $("[name='FkfamiliasDesde']").val(),
            FkfamiliaHasta: $("[name='FkfamiliasHasta']").val(),
            LoteDesde: $("[name='LoteDesde']").val(),
            LoteHasta: $("[name='LoteHasta']").val(),
            Solotablas: $("[name='Solotablas']").val(),
            Flujo: '0'
        };
        var Solotablas = $("[name='Solotablas']").val();
        if (Solotablas == "True") {
            parametros.Lote = $("[name='Lote']").val();
        }
        $http({
            url: $scope.Urllotes, method: "GET", params: parametros
        })
       .success(function (response) {
           $scope.selected = null;
           $scope.gridApi.grid.clearAllFilters();

           $scope.loading = false;

           $scope.gridOptions.columnDefs = response.columns;
           $scope.gridOptions.data = response.values;
           $scope.gridApi.grid.enableHorizontalScrollbar = 2;
           $scope.gridOptions.minWidth = 150;
           $scope.gridApi.core.refresh();
           $scope.gridApi.core.handleWindowResize();
           $timeout(function () {

               $("[name='columnheader-0']")[0].focus();
               $("[name='columnheader-0']").on("keydown", function (e) {
                   if (e.keyCode === 40) {
                       $scope.scrollToFocus(0, 0);
                   }
               });

           });


       }).error(function (data, status, headers, config) {
           $scope.gridOptions.data = [];
           $scope.loading = false;
       });


    }

    $scope.Cancelar = function () {
        $("#_buscarlotes").modal('hide');
    }

    $scope.Aceptar = function () {
        $scope.loading = true;
        var parametros = {
            Fkalmacen: $("[name='Fkalmacen']").val(),
            FkarticulosDesde: $("[name='FkarticulosDesde']").val(),
            FkarticulosHasta: $("[name='FkarticulosHasta']").val(),
            FkfamiliaDesde: $("[name='FkfamiliasDesde']").val(),
            FkfamiliaHasta: $("[name='FkfamiliasHasta']").val(),
        };

        $http({
            url: $scope.Urlagregarlotes,
            method: "POST",
            data: $scope.gridApi.selection.getSelectedRows()
        })
            .success(function (response) {
                if (response.error) {
                    $scope.Errorgeneral = response.error;
                    return;
                }

                GridView.Refresh();
                $("#_buscarlotes").modal('hide');
            });
    }

    eventAggregator.RegisterEvent("Realizarbusquedalotesbundle", function(msg) {
        $scope.RealizarBusqueda();
    });

}]);