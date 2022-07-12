//Rai

app.controller('AsistenteAsignacionTesoreriaCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    $scope.urlUltimaSituacion;
    $scope.urlCuentaTesoreria;
    $scope.urlSerieContable;
    $scope.urlFormasPago;
    $scope.Importe;


    //Seccion Mandato
    $scope.urlApiBancos;
    $scope.Fkclientes;
    $scope.bancoscliente;
    $scope.fkbancosmandatos;


    //Init
    $scope.init = function (urlUltimaSituacion, urlCuentaTesoreria, urlSerieContable, urlFormasPago) {
        $scope.urlUltimaSituacion = urlUltimaSituacion;
        $scope.urlCuentaTesoreria = urlCuentaTesoreria;
        $scope.urlSerieContable = urlSerieContable;
        $scope.urlFormasPago = urlFormasPago;
    }

    //Se le llama desde el index cshtml. Tiene los datos de la primera pantalla, llamaremos a un controlador que llamara a un servicio
    eventAggregator.RegisterEvent("_buscarvencimientosasociados", function (data) {
        $scope.params = data.Params;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, data.IdFormulariomodal, data.Url, data.Titulo);
    });

    //Segunda pantalla, generamos los datos de la tercera pantalla
    eventAggregator.RegisterEvent("_generarImporteCircuitos", function (data) {

        if ($scope.gridApi.selection.getSelectedCount() >= 1) {

            var filas = $scope.gridApi.selection.getSelectedRows();
            var importe = 0;

            //Importe
            if ($('#Importe').val() == "") {
                filas.forEach(function (item) {
                    importe += item.Importegiro;
                });
            }

            else {
                importe = $('#Importe').val();
            }          

            $('#ImportePantalla3').val(importe);

            //Cuenta tesoreria
            $('#Fkcuentatesoreria').val(filas[0].Fkcuentatesoreria);

            $http.get($scope.urlCuentaTesoreria + "?cuenta=" + filas[0].Fkcuentatesoreria).success(function (data) {
                var modeloCuenta = JSON.parse(data);
                window.document.getElementById("cv-Fkcuentatesoreria-descripcion").textContent = modeloCuenta.Descripcion;
            }).error(function (error) {
                console.log(error);
            });

            var fecha = (filas[0].FechaStrvencimiento.split(" "))[0];

            //Fecha Vencimiento
            $('#FechaVencimientoPantalla3').val(fecha);

            //Serie
            var tipo = $('#Tipo').val();

            $http.get($scope.urlSerieContable + "?tipo=" + tipo).success(function (data) {
                var modeloSerie = JSON.parse(data);
                $('#Fkseriescontables').val(modeloSerie.Tipodocumento);
                window.document.getElementById("cv-Fkseriescontables-descripcion").textContent = modeloSerie.Descripcion;
            }).error(function (error) {
                console.log(error);
            });

            //Cuenta tercera pantalla
            $('#Fkcuentas2').val($('#Fkcuentas').val());
            window.document.getElementById("cv-Fkcuentas2-descripcion").textContent = window.document.getElementById("cv-Fkcuentas-descripcion").textContent;

            //Forma pago
            var formapago = filas[0].Fkformaspago;

            $http.get($scope.urlFormasPago + "?formapago=" + formapago).success(function (data) {
                var modeloFormaPago = JSON.parse(data);
                $('#Fkformaspago').val(modeloFormaPago.Id);
                window.document.getElementById("cv-Fkformaspago-descripcion").textContent = modeloFormaPago.Nombre;
            }).error(function (error) {
                console.log(error);
            });

            var circuito = $('#Circuitotesoreria').val();

            $http.get($scope.urlUltimaSituacion + "?circuito=" + circuito).success(function (data) {
                var modeloSituacion = JSON.parse(data);
                $('#SituacionFinal').val(modeloSituacion.Cod);
                window.document.getElementById("cv-SituacionFinal-descripcion").textContent = modeloSituacion.Descripcion;
            }).error(function (error) {
                console.log(error);
            });

        }      
    });

    //Ultima pantalla, van a generar los registros de cartera
    eventAggregator.RegisterEvent("_generarregistroscartera", function (data) {
        $scope.generarregistroscartera();
    });


    //Register Event del mandato
    eventAggregator.RegisterEvent("_buscarmandato", function (data) {

        $scope.urlApiBancos = data.urlApiBancos;
        $scope.Fkclientes = data.fkclientes;

        $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkclientes)
                .success(function (response) {
                    if (response.values.length > 0) {
                        $scope.bancoscliente = response.values;

                        //Devuelve un array
                        for (var i = 0; i < $scope.bancoscliente.length; i++) {
                            if ($scope.bancoscliente[i].Idmandato == null) {
                                $scope.bancoscliente[i].Idmandato = "";
                            }
                            $scope.bancoscliente[i].Descripcion = $scope.bancoscliente[i].Descripcion + ", " + $scope.bancoscliente[i].Idmandato + ", " + $scope.bancoscliente[i].Tipoadeudocadena + ", " + $scope.bancoscliente[i].Esquemacadena;
                        }
                        $scope.bancoscliente.splice(0, 0, { Id: "", Descripcion: "", Defecto: false });
                        if (!$scope.fkbancosmandatos && $scope.fkbancosmandatos != "") {
                            var result = $.grep($scope.bancoscliente, function (e) { return e.Defecto == true; });
                            if (result.length)
                                $scope.fkbancosmandatos = result[0].Id;
                        }
                    }
                    else {
                        $scope.fkbancosmandatos = "";
                        $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
                    }
                }).error(function (data, status, headers, config) {
                    console.log("error call urlApiBancos")
                });
    });

    //Seccion del Scope Grid
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";
    $scope.Titulo = "";
    $scope.gridOptionsvencimientos = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptionsvencimientos = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true };
    $scope.gridOptionsvencimientos.multiSelect = true;
    $scope.gridOptionsvencimientos.modifierKeysToMultiSelect = true;
    $scope.gridOptionsvencimientos.noUnselect = false;
    $scope.gridOptionsvencimientos.selectionRowHeaderWidth = 35;
    $scope.gridOptionsvencimientos.enableFiltering = true;
    $scope.cantidadvencimientos = 0;


    //-------------SECCION GRID--------------

    $scope.gridOptionsvencimientos.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;
            $scope.cantidadvencimientos = $scope.gridApi.selection.getSelectedCount();
        });

        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.cantidadvencimientos = $scope.gridApi.selection.getSelectedCount();
        });

        $scope.gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApi.selection.selectRow(newRowCol.row.entity);
        });
    };

    $scope.gridOptionsvencimientos.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptionsvencimientos.appScopeProvider = {
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
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptionsvencimientos.columnDefs[colIndex]);
    };

    $scope.loading = true;
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {
        $scope.loading = true;
        $scope.cantidadvencimientos = 0;
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

               $scope.gridOptionsvencimientos.columnDefs = response.columns;
               $scope.gridOptionsvencimientos.data = response.values;
               $scope.gridApi.grid.enableHorizontalScrollbar = 2;
               $scope.gridOptionsvencimientos.minWidth = 150;
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
               $scope.gridOptionsvencimientos.data = [];
               $scope.loading = false;
           });
        }
        else {
            $http.get($scope.Url)
           .success(function (response) {
               $scope.selected = null;

               $scope.gridApi.grid.clearAllFilters();
               $scope.loading = false;
               $scope.gridOptionsvencimientos.columnDefs = response.columns;
               $scope.gridOptionsvencimientos.data = response.values;
               $scope.gridApi.grid.enableHorizontalScrollbar = 2;
               $scope.gridOptionsvencimientos.minWidth = 150;
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
               $scope.gridOptionsvencimientos.data = [];
               $scope.loading = false;
           });
        }


    }

    //Cuando el Circuito Tesoreria cambie, la situacion final de la ultima pantalla debera asignarse
    eventAggregator.RegisterEvent("Circuitotesoreria-Buscarfocus", function (message) {

        $http.get($scope.urlUltimaSituacion + "?circuito=" + JSON.stringify(message)).success(function (data) {
            var modeloSituacion = JSON.parse(data);
            $('#SituacionFinal').val(modeloSituacion.Cod);
            window.document.getElementById("cv-SituacionFinal-descripcion").textContent = modeloSituacion.Descripcion;
        }).error(function (error) {
            console.log(error);
        });
    });


    //api cartera
    $scope.generarregistroscartera = function () {

        var rows = $scope.gridApi.selection.getSelectedRows();

        var vencimientosId = $.map(rows, function (v) {
            return v.Id;
        }).join(';');

        var tipo = $("#Tipo").val();
        var serie = $("#Fkseriescontables").val();
        var cuenta = $("#Fkcuentas").val();
        var importe = $("#Importe").val();
        var importePantalla3 = $("#ImportePantalla3").val();
        var situacion = $("#SituacionFinal").val();
        var cuentatesoreria = $("#Fkcuentatesoreria").val();
        var formaspago = $("#Fkformaspago").val();
        var circuitotesoreria = $("#Circuitotesoreria").val();
        var fechacontable = $("#FechaContable").val();
        var fechavencimiento = $("#FechaVencimientoPantalla3").val();
        var banco = $("#Banco").val();
        var letra = $("#Letra").val();
        var notas = $("#Comentario").val();


        $("#asignacioncarteraform input[name='Tipo']").val(tipo);
        $("#asignacioncarteraform input[name='Vencimientos']").val(vencimientosId);
        $("#asignacioncarteraform input[name='Fkseriescontables']").val(serie);
        $("#asignacioncarteraform input[name='Fkcuentas']").val(cuenta);
        $("#asignacioncarteraform input[name='Importe']").val(importe);
        $("#asignacioncarteraform input[name='ImportePantalla3']").val(importePantalla3);
        $("#asignacioncarteraform input[name='Situacion']").val(situacion);
        $("#asignacioncarteraform input[name='Fkcuentatesoreria']").val(cuentatesoreria);
        $("#asignacioncarteraform input[name='Fkformaspago']").val(formaspago);
        $("#asignacioncarteraform input[name='Circuitotesoreria']").val(circuitotesoreria);
        $("#asignacioncarteraform input[name='FechaContable']").val(fechacontable);
        $("#asignacioncarteraform input[name='FechaVencimiento']").val(fechavencimiento);
        $("#asignacioncarteraform input[name='Banco']").val(banco);
        $("#asignacioncarteraform input[name='Letra']").val(letra);
        $("#asignacioncarteraform input[name='Comentario']").val(notas);

        $("#asignacioncarteraform").submit();
    }

    //end api cartera
}]);


