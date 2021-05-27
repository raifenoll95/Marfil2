//Raimundo Fenoll

app.controller('AsignarContenedoresCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

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
    $scope.articulos = [];
    $scope.articulosduplicados = [];
    $scope.Id;
    $scope.urlValidar;
    $scope.data;

    $scope.init = function (urlValidar) {
        $scope.urlValidar = urlValidar;
    }

    //Primera pantalla
    eventAggregator.RegisterEvent("_lanzarbusquedarasignarcontendores", function (data) {

        $('#asignarcontenedores').modal('show');
        $scope.data = data;
        $scope.values = data.Values;
        $scope.columns = data.Columns;
        $scope.Url = data.Url;
        $scope.Idalbaran = data.Idalbaran;
        $scope.articulos = $scope.values;

        $scope.load(data.campoIdentificador, data.IdComponenteasociado, "reclamaralbaran", data.Url, data.Titulo);
    });

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

    //Primera pantalla
    $scope.siguiente = function () {

        $scope.articulos = $scope.gridApi.selection.getSelectedRows();

        $scope.articulos.forEach(function (articulo) {
            articulo["contenedororiginal"] = articulo.Contenedor;
            articulo["sellooriginal"] = articulo.Sello;
            articulo["pesonetooriginal"] = articulo.Pesoneto;
            articulo["cajaoriginal"] = articulo.Caja;
        });

        $('#asignarcontenedores').modal('hide');
        $('#introducircampos').modal('show');
        document.getElementById("contenedor").value = "";
        document.getElementById("contenedor").value = "";
        document.getElementById("contenedor").value = "";
        document.getElementById("myForm").reset();
    };

    $scope.cancelarlineas = function () {
        $('#asignarcontenedores').modal('hide');
    }

    //Segunda pantalla
    $scope.anteriorintroducir = function () {
        document.getElementById("myForm").reset();
        $('#introducircampos').modal('hide');
        $('#asignarcontenedores').modal('show');
    }
    $scope.cancelarlineasintroducir = function () {
        document.getElementById("myForm").reset();
        $('#introducircampos').modal('hide');
    }
    $scope.siguienteintroducir = function () {

        var contenedor = document.getElementById("contenedor").value;
        var sello = document.getElementById("sello").value;
        var pesoneto = document.getElementById("pesoneto").value;
        var caja = document.getElementById("caja").value;

        //Validar
        if ($scope.Validar(contenedor, sello, pesoneto, caja)) {
            $scope.articulos.forEach(function (articulo) {
                articulo["showEdit"] = true;
                articulo["Contenedor"] = contenedor;
                articulo["Sello"] = sello;
                articulo["Pesoneto"] = pesoneto;
                articulo["Caja"] = caja;
                articulo["anothercolor"] = true;

                if (articulo["Contenedor"] == null || articulo["Contenedor"] == "") {
                    articulo["Contenedor"] = articulo["contenedororiginal"];
                }
                if (articulo["Sello"] == null || articulo["Sello"] == "") {
                    articulo["Sello"] = articulo["sellooriginal"];
                }
                if (articulo["Pesoneto"] == null || articulo["Pesoneto"] == "") {
                    articulo["Pesoneto"] = articulo["pesonetooriginal"];
                }
                if (articulo["Caja"] == null || articulo["Caja"] == "") {
                    articulo["Caja"] = articulo["cajaoriginal"];
                }
            });

            document.getElementById("myForm").reset();

            $('#introducircampos').modal('hide');
            $('#asignarcontenedoresultimo').modal('show');
        }
    }

    //Validar campos
    $scope.Validar = function (contenedor, sello, pesoneto, caja) {

        var validacion = true;

        if (contenedor.length > 12 && !!contenedor.length) {
            validacion = false;
            bootbox.alert("El campo contenedor debe tener un máximo de 12 caracteres");
        }
        if (sello.length > 10 && !!sello.length) {
            validacion = false;
            bootbox.alert("El campo sello debe tener un máximo de 10 caracteres");
        }
        if (!Number.isInteger(parseInt(pesoneto)) && !!pesoneto.length) {
            validacion = false;
            bootbox.alert("El campo peso neto debe ser de tipo dígito");
        }
        if (!Number.isInteger(parseInt(caja)) && !!caja.length) {
            validacion = false;
            bootbox.alert("El campo caja debe ser de tipo dígito");
        }

        return validacion;
    }
    
    //Tercera pantalla
    $scope.cancelarultimo = function () {
        $('#asignarcontenedoresultimo').modal('hide');
    }

    $scope.anteriorultimo = function () {
        $('#asignarcontenedoresultimo').modal('hide');
        $('#introducircampos').modal('show');
    }

    $scope.asignar = function () {

        $scope.articulos.forEach(function (art) {
            if (art.Pesoneto != null || art.Pesoneto != "") {
                art.Pesoneto = art.Pesoneto.toString().replace(".", ",");
            }   
        });

        var lineas = JSON.stringify($scope.articulos); 

        $('#asignarcontenedoresultimo').modal('hide');

        $("#asignarcontenedoresform [name='id']").val($scope.Idalbaran);
        $("#asignarcontenedoresform [name='lineas']").val(lineas);
        $("#asignarcontenedoresform").submit();
        $('#asignarcontenedoresultimo').modal('hide');
    }

    //Editar articulo
    $scope.toggleEdit = function (articulo) {

        articulo.showEdit = articulo.showEdit ? false : true;
    }

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


