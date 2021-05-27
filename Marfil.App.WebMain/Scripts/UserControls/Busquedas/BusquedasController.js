app.controller('BusquedasCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {


    eventAggregator.RegisterEvent("_lanzarbusqueda", function (data) {

        $('#' + data.IdFormulariomodal).modal('toggle');
        $scope.params = data.Params;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, data.IdFormulariomodal, data.Url, data.Titulo);
    });

    $rootScope.$on("CallFormSearch", function (event, data) {
        $('#' + data.obj.IdFormulariomodal).modal('toggle');
        $scope.params = data.obj.Params;
        $scope.load(data.obj.campoIdentificador, data.obj.IdComponenteasociado, data.obj.IdFormulariomodal, data.obj.Url, data.obj.Titulo);
    });

    //configuracion Control
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";
    $scope.Titulo = "";
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptions.multiSelect = false;
    $scope.gridOptions.modifierKeysToMultiSelect = false;
    $scope.gridOptions.noUnselect = true;
    $scope.gridOptions.enableFiltering = true;
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;
        });

        $scope.gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApi.selection.selectRow(newRowCol.row.entity);
        });
    };

    $scope.gridOptions.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
           
        },

    };

  /*  document.addEventListener('keydown', function (event) {
        if (event.keyCode == 13 && $('#' + $scope.IdFormulariomodal).is(":visible")) {
            
            $scope.aceptar();
            event.preventDefault();
           
        } else if (event.keyCode == 27 && $('#' + $scope.IdFormulariomodal).is(":visible")) {

            $scope.cancelar();
            event.preventDefault();

        }
    }, false);*/


    $scope.aceptar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscarfocus", $scope.selected.entity[$scope.campoIdentificador]);
        eventAggregator.Publish($scope.IdComponenteasociadonombre + "-Buscarfocus", $scope.selected.entity[$scope.campoIdentificador]);
    };

    $scope.cancelar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        $('#' + $scope.IdComponenteasociado).focus();
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscarcancelar",null);
    };

    $scope.scrollToFocus = function (rowIndex, colIndex) {
        var row = $scope.gridApi.grid.getVisibleRows()[rowIndex].entity;
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptions.columnDefs[colIndex]);
    };

    $scope.loading = true;

    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {

        $scope.loading = true;
        $scope.campoIdentificador = campoIdentificador;
        $scope.IdComponenteasociado = IdComponenteasociado;
        $scope.IdFormulariomodal = IdFormulariomodal;
        $scope.Url = Url;
        $scope.Titulo = Titulo;
        if ($scope.params) {
            $http({
                url: $scope.Url, method: "GET", params: JSON.parse($scope.params)
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
        else {

            $http.get($scope.Url)
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


    }

}]);

app.controller('BotonbuscarCtrl', [
    '$scope', '$rootScope', function ($scope, $rootScope) {
        $scope.buscar = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo, Params, controlesAsociados) {
            if (controlesAsociados) {
                var nobj = {};
                if (Params)
                    nobj = JSON.parse(Params);
                var controles = JSON.parse(controlesAsociados);

                for (var i = 0; i < controles.length; i++) {
                    var obj = controles[i];
                    $.each(obj, function (k, v) {
                        nobj[k] = $("#" + v).val();
                    });
                }

                Params = JSON.stringify(nobj);
            }
            var obj2 = {
                campoIdentificador: campoIdentificador,
                IdComponenteasociado: IdComponenteasociado,
                IdFormulariomodal: IdFormulariomodal,
                Url: Url,
                Titulo: Titulo,
                Params: Params
            };

            $rootScope.$emit("CallFormSearch", { obj: obj2 });
        }
    }
]);
