app.controller('AsistenteRegularizacionGruposCrtl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {
    
    eventAggregator.RegisterEvent("_buscarcuentasgrupos", function (data) {
        $scope.params = data.Params;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, data.IdFormulariomodal, data.Url, data.Titulo);
    });
    eventAggregator.RegisterEvent("_generarasientocontable", function (data) {
        $scope.generarasiento();
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
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true };
    $scope.gridOptions.multiSelect = true;
    $scope.gridOptions.modifierKeysToMultiSelect = true;
    $scope.gridOptions.noUnselect = false;
    $scope.gridOptions.selectionRowHeaderWidth = 35;
    $scope.gridOptions.enableFiltering = true;
    $scope.registros = 0;

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        
        /*gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;
            $scope.registros = $scope.gridApi.selection.getSelectedCount();
        });

        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.registros = $scope.gridApi.selection.getSelectedCount();
        });

        $scope.gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApi.selection.selectRow(newRowCol.row.entity);
        });*/
    };

    $scope.gridOptions.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
        }
    };

    document.addEventListener('keydown', function (event) {
        if (event.keyCode == 13 && $('#' + $scope.IdFormulariomodal).is(":visible")) {
            
            $scope.aceptar();
           
        }
    }, false);

   $scope.aceptar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscarfocus", $scope.selected.entity[$scope.campoIdentificador]);
    };

    $scope.cancelar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        $('#' + $scope.IdComponenteasociado).focus();
    };

    $scope.scrollToFocus = function (rowIndex, colIndex) {
        var row = $scope.gridApi.grid.getVisibleRows()[rowIndex].entity;
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptions.columnDefs[colIndex]);
    };

    $scope.loading = true;
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {
        $scope.loading = true;
        $scope.registros = 0;
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

               if ($scope.gridOptions.data == []) {

                   $("#registrosseleccionadoserror").html("NO HAY NINGUNA CUENTA/SALDO DE LOS GRUPOS 6 ó 7");
                   e.preventDefault();
               }



           }).error(function (data, status, headers, config) {
               $scope.gridOptions.data = [];
               $scope.loading = false;
               });
        }


    }


    //api pedidor

    $scope.generarasiento = function () {
        //Se marcan todas y se mandan todas
        var index = 0;
        $scope.gridOptions.data.forEach(function () {
            $scope.gridApi.selection.selectRow($scope.gridOptions.data[index]);
            index++;
        });

        var rows = $scope.gridApi.selection.getSelectedRows();
        var fecharegularizacion = $("#Fecharegularizacion").val();
        var seriecontable = $("#Fkseriescontables").val();
        var cuentapyg = $("#CuentaPYG").val();
        var comentariodebepyg = $("[name='ComentarioDebePYG']").val();
        var comentariohaberpyg = $("[name='ComentarioHaberPYG']").val();
        var comentariocuentasdetalle = $("[name='ComentarioCuentasDetalle']").val();

        var cuentasgrupos=$.map(rows, function(v){
            return v.Cuentagrupos;
        }).join(';');
        var saldodeudor = $.map(rows, function (v) {
            return v.SaldoDeudor;
        }).join(';');
        var saldoacreedor = $.map(rows, function (v) {
            return v.SaldoAcreedor;
        }).join(';');


        $("#generarasiento input[name='fecharegularizacion']").val(fecharegularizacion);
        $("#generarasiento input[name='seriecontable']").val(seriecontable);
        $("#generarasiento input[name='cuentapyg']").val(cuentapyg);
        $("#generarasiento input[name='comentariodebepyg']").val(comentariodebepyg);
        $("#generarasiento input[name='comentariohaberpyg']").val(comentariohaberpyg);
        $("#generarasiento input[name='comentariocuentasdetalle']").val(comentariocuentasdetalle);

        $("#generarasiento input[name='cuentasgrupos']").val(cuentasgrupos);
        $("#generarasiento input[name='saldodeudor']").val(saldodeudor);
        $("#generarasiento input[name='saldoacreedor']").val(saldoacreedor);
        

        $("#generarasiento").submit();
    }
    //end api facturar
}]);


