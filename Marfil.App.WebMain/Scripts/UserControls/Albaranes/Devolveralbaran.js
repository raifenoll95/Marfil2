app.controller('DevolveralbaranCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    eventAggregator.RegisterEvent("_lanzarbusquedadevolveralbaran", function (data) {

        $('#devolveralbaran').modal('show');
        $scope.values = data.Values;        
        $scope.columns = data.Columns;
        $scope.Url = data.Url;
        $scope.Idalbaran = data.Idalbaran;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, "devolveralbaran", data.Url, data.Titulo);
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
    $scope.values = [];
    $scope.columns = [];
    $scope.cantidadesPermitidas = [];
    $scope.ErrorCantidadesADevolver = "";

    //start grid especificas
    $scope.gridOptions = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true };
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

        if ($scope.gridApi.selection.getSelectedCount() > 0) {            
           // $.redirect($scope.Url, {'id':$scope.Idalbaran, 'lineas': JSON.stringify($scope.gridApi.selection.getSelectedRows()) });
            //window.location = String.format("{0}?id={1}&lineas={2}", $scope.Url, $scope.Idalbaran, JSON.stringify($scope.gridApi.selection.getSelectedRows()));

            $scope.ErrorCantidadesADevolver = "";
            for (var i = 0; i < $scope.gridApi.selection.getSelectedCount() ; i++)
            {
                var row = $scope.gridApi.selection.getSelectedRows()[i];
                var cantidadDevolver = row.Cantidad                
                var cantidadMaxima = $scope.cantidadesPermitidas[row.Id -1]
                //console.log('cantidadDevolver: ' + cantidadDevolver)
                //console.log('cantidadMaxima: ' + cantidadMaxima)
                if (cantidadDevolver > cantidadMaxima) {
                    //console.log('Más de lo permitido')
                    $scope.ErrorCantidadesADevolver += ("La cantidad a devolver del artículo " + $scope.values[row.Id - 1].Descripcion +
                        " es superior a la cantidad del artículo en el albarán" + '\n')                                       
                }                
            }
            
            if ($scope.ErrorCantidadesADevolver.length == 0) {                
                $("#devolucionform [name='id']").val($scope.Idalbaran);
                $("#devolucionform [name='lineas']").val(JSON.stringify($scope.gridApi.selection.getSelectedRows()));
                $("#devolucionform").submit();
                $('#devolveralbaran').modal('hide');
            }
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
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {
        $scope.loading = true;
        $scope.campoIdentificador = campoIdentificador;
        $scope.IdComponenteasociado = IdComponenteasociado;
        $scope.IdFormulariomodal = IdFormulariomodal;
        $scope.Titulo = Titulo;
        
        $scope.cantidadesPermitidas = [];
        for (var i = 0; i < $scope.values.length; i++)
        {            
            $scope.cantidadesPermitidas.push($scope.values[i].Cantidad);
        }

        $scope.selected = null;
        $scope.gridApi.grid.clearAllFilters();
        $scope.loading = false;
        $scope.gridOptions.columnDefs = $scope.columns;
        $scope.gridOptions.data = $scope.values;
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
    }

}]);


