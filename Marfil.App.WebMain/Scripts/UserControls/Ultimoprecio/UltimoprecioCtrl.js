app.controller('UltimoprecioCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {


    eventAggregator.RegisterEvent("_lanzarbusquedaultimoprecio", function (data) {
        $('#ultimoprecio').modal('toggle');
        $scope.params = data.Params;
        
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, "ultimoprecio", data.Url, data.Titulo, data.Params);
    });

    
    //configuracion Control
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";
    $scope.Titulo = "";

    $scope.Cuenta = "";
    $scope.Cliente = "";
    $scope.Articulo = "";
    $scope.Descripcion = "";

    //start grid especificas
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: false };
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
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptions.rowTemplate = '<div   role=\"gridcell\" ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
            $('#' + $scope.IdFormulariomodal).modal('hide');
        },

    };
    //end grid especificas

    //start grid sistema venta
    
    $scope.gridSistema = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.gridSistema.multiSelect = false;
    $scope.gridSistema.modifierKeysToMultiSelect = false;
    $scope.gridSistema.noUnselect = true;
    $scope.gridSistema.enableFiltering = true;
    $scope.gridSistema.onRegisterApi = function (gridApi) {
        $scope.gridApiSistemas = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedsistema = row;

        });

        $scope.gridApiSistemas.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApiSistemas.selection.selectRow(newRowCol.row.entity);
        });
    };
    $scope.datasistema = [];
    $scope.selectedsistema = null;
    $scope.params = "";
    $scope.gridSistema.rowTemplate = '<div  role=\"gridcell\" ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridSistema.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selectedsistema = row;
            $scope.aceptar();
            $('#' + $scope.IdFormulariomodal).modal('hide');
        },

    };
    //end grid Sistema venta


    //start grid sistema compra

    $scope.gridSistemaCompra = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.gridSistemaCompra.multiSelect = false;
    $scope.gridSistemaCompra.modifierKeysToMultiSelect = false;
    $scope.gridSistemaCompra.noUnselect = true;
    $scope.gridSistemaCompra.enableFiltering = true;
    $scope.gridSistemaCompra.onRegisterApi = function (gridApi) {
        $scope.gridApiSistemasCompra = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedsistemacompra = row;

        });

        $scope.gridApiSistemasCompra.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApiSistemasCompra.selection.selectRow(newRowCol.row.entity);
        });
    };
    $scope.datasistemacompra = [];
    $scope.selectedsistemacompra = null;
    $scope.paramscompra = "";
    $scope.gridSistemaCompra.rowTemplate = '<div  role=\"gridcell\" ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridSistemaCompra.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selectedsistemacompra = row;
            $scope.aceptar();
            $('#' + $scope.IdFormulariomodal).modal('hide');
        },

    };
    //end grid Sistema copmra
    
   /* document.addEventListener('keydown', function (event) {
        if (event.keyCode == 13 && $('#' + $scope.IdFormulariomodal).is(":visible")) {

            $scope.aceptar();
            $('#' + $scope.IdFormulariomodal).modal('hide');
            event.preventDefault();
        }
        else if (event.keyCode == 27 && $('#' + $scope.IdFormulariomodal).is(":visible")) {

            $scope.cancelar();
            event.preventDefault();

        }
    }, false);*/

    $scope.aceptar = function () {
        var target = $('#ultimoprecio .tab-pane.active').attr("id");
        var valor = 0.0;
        switch (target) {
            case "sistema":
                valor = $scope.selectedsistema.entity[$scope.campoIdentificador];
                break;
            case "sistemacompra":
                valor = $scope.selectedsistemacompra.entity[$scope.campoIdentificador];
                break;
            default:
                valor = $scope.selected.entity[$scope.campoIdentificador];
        }
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscar", valor);
    };

    $scope.cancelar = function () {
        $('#' + $scope.IdFormulariomodal).modal('hide');
        $('#' + $scope.IdComponenteasociado).focus();
        eventAggregator.Publish($scope.IdComponenteasociado + "-Buscarcancelar", null);
    };

    $scope.scrollToFocus = function (rowIndex, colIndex) {
        var row = $scope.gridApi.grid.getVisibleRows()[rowIndex].entity;
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptions.columnDefs[colIndex]);
    };

    $scope.loading = true;
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo, Params) {
        $scope.loading = true;

        $scope.campoIdentificador = campoIdentificador;
        $scope.IdComponenteasociado = IdComponenteasociado;
        $scope.IdFormulariomodal = IdFormulariomodal;
        $scope.Url = Url;
        $scope.Titulo = Titulo;

        var parametros = JSON.parse(Params);
        $scope.Cuenta = parametros.fkcuenta;
        $scope.Cliente = parametros.cliente;
        $scope.Articulo = parametros.articulo;
        $scope.Descripcion = parametros.descripcion;

        if (Params) {
            $http({
                url: $scope.Url, method: "GET", params: parametros
            })
           .success(function (response) {

               $scope.selected = null;
               $scope.gridApi.grid.clearAllFilters();

               $scope.loading = false;

               $scope.gridOptions.columnDefs = response.Especificos.columns;
               $scope.gridOptions.data = response.Especificos.values;
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
               //venta
               $scope.gridSistema.columnDefs = response.SistemaVenta.columns;
               $scope.gridSistema.data = response.SistemaVenta.values;
               $scope.gridApiSistemas.grid.enableHorizontalScrollbar = 2;
               $scope.gridSistema.minWidth = 150;
               $scope.gridApiSistemas.core.refresh();
               $scope.gridApiSistemas.core.handleWindowResize();

               //compras
               $scope.gridSistemaCompra.columnDefs = response.SistemaCompra.columns;
               $scope.gridSistemaCompra.data = response.SistemaCompra.values;
               $scope.gridApiSistemasCompra.grid.enableHorizontalScrollbar = 2;
               $scope.gridSistemaCompra.minWidth = 150;
               $scope.gridApiSistemasCompra.core.refresh();
               $scope.gridApiSistemasCompra.core.handleWindowResize();

           }).error(function (data, status, headers, config) {

               alert(data);
           });
        }
        else {
            $http.get($scope.Url)
           .success(function (response) {
               $scope.selected = null;
               $scope.gridApi.grid.clearAllFilters();
               $scope.loading = false;
               $scope.gridOptions.columnDefs = response.Especificos.columns;
               $scope.gridOptions.data = response.Especificos.values;
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

               //venta
               $scope.gridSistema.columnDefs = response.SistemaVenta.columns;
               $scope.gridSistema.data = response.SistemaVenta.values;
               $scope.gridApiSistemas.grid.enableHorizontalScrollbar = 2;
               $scope.gridSistema.minWidth = 150;
               $scope.gridApiSistemas.core.refresh();
               $scope.gridApiSistemas.core.handleWindowResize();

               
               //compras
               $scope.gridSistemaCompra.columnDefs = response.SistemaCompra.columns;
               $scope.gridSistemaCompra.data = response.SistemaCompra.values;
               $scope.gridApiSistemasCompra.grid.enableHorizontalScrollbar = 2;
               $scope.gridSistemaCompra.minWidth = 150;
               $scope.gridApiSistemasCompra.core.refresh();
               $scope.gridApiSistemasCompra.core.handleWindowResize();

           }).error(function (data, status, headers, config) {

               alert(data);
           });
        }


    }
    $("#tarifas").on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href"); // activated tab
        if (target === "#sistema") {
            $scope.gridApiSistemas.core.refresh();
            $scope.gridApiSistemas.core.handleWindowResize();
        }
        else if (target == "#sistemacompra") {
            $scope.gridApiSistemasCompra.core.refresh();
            $scope.gridApiSistemasCompra.core.handleWindowResize();
        }
        else {
            $scope.gridApi.core.refresh();
            $scope.gridApi.core.handleWindowResize();
        }
    });
}]);


