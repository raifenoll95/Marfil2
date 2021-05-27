app.controller('ImportarlineasCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {


    eventAggregator.RegisterEvent("_lanzarbusquedaimportarlineas", function (data) {
        $('#importarlineas').modal('show');
        $scope.params = data.Params;
        
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, "importarlineas", data.Url, data.Titulo, data.Params);
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
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit :true};
    $scope.gridOptions.multiSelect = true;
    $scope.gridOptions.modifierKeysToMultiSelect = true;
    $scope.gridOptions.noUnselect = false;
    $scope.gridOptions.selectionRowHeaderWidth = 35;
    $scope.gridOptions.enableFiltering = true;
    
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = $scope.gridApi.selection.getSelectedCount();

        });
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.selected = $scope.gridApi.selection.getSelectedCount();
        });
        
    };
    $scope.data = [];
    $scope.selected = 0;
    $scope.params = "";
    
    //end grid especificas

   
    
   

    $scope.aceptar = function () {
        
        if ($scope.gridApi.selection.getSelectedCount()>0) {
            
            $('#' + $scope.IdFormulariomodal).modal('hide');
            eventAggregator.Publish($scope.IdComponenteasociado + "-Buscar", $scope.gridApi.selection.getSelectedRows());
        }
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

               alert(data);
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

               alert(data);
           });
        }


    }
    
}]);


