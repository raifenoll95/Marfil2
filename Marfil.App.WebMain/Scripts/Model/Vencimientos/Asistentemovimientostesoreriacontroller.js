//Rai

app.controller('AsistenteMovimientosTesoreriaCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    $scope.UrlCompletarDatosCircuito;
    $scope.urlObtenerCuentaCargo;
    $scope.urlObtenerCuentaAbono;
    $scope.urlObtenerDescripcionCobrador;
    $scope.urlObtenerEjercicio;


    //Seccion Mandato
    $scope.urlApiBancos;
    $scope.Fkclientes;
    $scope.bancoscliente;
    $scope.fkbancosmandatos;
    $scope.cobradorNecesario = false;

    $scope.importeCuentaCargo2Necesario = false;
    $scope.importeCuentaAbono2Necesario = false;

    $scope.descCargo;
    $scope.descAbono;

    $scope.generarMovimiento = true;


    //Init
    $scope.init = function (UrlCompletarDatosCircuito, urlObtenerCuentaCargo, urlObtenerCuentaAbono, urlObtenerDescripcionCobrador, urlObtenerEjercicio) {
        $scope.UrlCompletarDatosCircuito = UrlCompletarDatosCircuito;
        $scope.urlObtenerCuentaCargo = urlObtenerCuentaCargo;
        $scope.urlObtenerCuentaAbono = urlObtenerCuentaAbono;
        $scope.urlObtenerDescripcionCobrador = urlObtenerDescripcionCobrador;
        $scope.urlObtenerEjercicio = urlObtenerEjercicio;
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
            var circuito = $('#Circuitotesoreria').val();

            $http.get($scope.UrlCompletarDatosCircuito + "?circuito=" + circuito).success(function (data) {
                var atributos = JSON.parse(data);

                $("#btnbuscarFkcuentatesoreria").attr("disabled", !atributos.cobrador);
                $("#Fkcuentatesoreria").attr("disabled", !atributos.cobrador);

                if (atributos.cobrador && filas[0].FkcuentaTesoreria != null) {
                    $('#Fkcuentatesoreria').val(filas[0].FkcuentaTesoreria);
                    $http.get($scope.urlObtenerDescripcionCobrador + "?cuenta=" + filas[0].FkcuentaTesoreria).success(function (data) {
                        var modelo = JSON.parse(data);
                        window.document.getElementById("cv-Fkcuentatesoreria-descripcion").textContent = modelo.Descripcion;
                    });
                }

                $("#btnbuscarFkseriescontables").attr("disabled", !atributos.remesa);
                $("#Fkseriescontables").attr("disabled", !atributos.remesa);

                if ($("#Fkseriescontables").is(":disabled") == false) {
                    $http.get($scope.urlObtenerEjercicio).success(function (data) {
                        var modelo = JSON.parse(data);
                        $("#Fkseriescontables").val(modelo.FkseriescontablesREM);
                        window.document.getElementById("cv-Fkseriescontables-descripcion").textContent = modelo.DescSerieContable;
                    });
                    //cv-Fkseriescontables-descripcion
                };

                $("#FechaRemesa").attr("disabled", !atributos.remesa);

                //Si está habilitada la fechaRemesa, se pone por defecto el día de hoy
                if ($("#FechaRemesa").is(":disabled") == false) {
                    var fecha = $("#FechaContable").val();
                    $("#FechaRemesa").val(fecha);
                };

                $("#campofechapago").attr("disabled", !atributos.actualizar);
                $("#FechaPago").attr("disabled", !atributos.actualizar);

                $("#btnbuscarBanco").attr("disabled", !atributos.datosdocumento);
                $("#Banco").attr("disabled", !atributos.datosdocumento);
                $("#Letra").attr("disabled", !atributos.datosdocumento);

                $("#ImporteCargo2").attr("disabled", !atributos.importecargo2);
                $("#ImporteAbono2").attr("disabled", !atributos.importeabono2);
                $("#Comentario").attr("disabled", !atributos.codigomanual);

                if(atributos.importecargo2) {
                    $scope.importeCuentaCargo2Necesario = true;
                }

                if(atributos.importeabono2) {
                    $scope.importeCuentaAbono2Necesario = true;
                }

            }).error(function (error) {
                console.log("error call to AsistenteMovimientoController");
            });

            //Cuenta Cargo
            $http.get($scope.urlObtenerCuentaCargo + "?circuito=" + circuito).success(function (data) {
                var modelo = JSON.parse(data);
                if (modelo != null) {
                    $('#Cuentacargo2').val(modelo.Id);
                    window.document.getElementById("cv-Cuentacargo2-descripcion").textContent = modelo.Descripcion;
                    $scope.descCargo = modelo.Descripcion;
                }               
            }).error(function (error) {
                console.log("error call to obtener cuenta cargo 2");
            });

            //Cuenta Abono
            $http.get($scope.urlObtenerCuentaAbono + "?circuito=" + circuito).success(function (data) {
                var modelo = JSON.parse(data);
                if (modelo != null) {
                    $('#Cuentaabono2').val(modelo.Id);
                    window.document.getElementById("cv-Cuentaabono2-descripcion").textContent = modelo.Descripcion;
                    $scope.descAbono = modelo.Descripcion;
                }
            }).error(function (error) {
                console.log("error call to obtener cuenta abono 2");
            });
        }      
    });

    //Ultima pantalla, van a generar los registros de cartera
    eventAggregator.RegisterEvent("_generarmovimientostesoreria", function (data) {

        var fkCuentasTesoreria = $('#Fkcuentatesoreria').val();
        var fkSeriesContables = $('#Fkseriescontables').val();

        if ($("#Fkseriescontables").is(":disabled") == false) {

            if (fkCuentasTesoreria != null && fkSeriesContables != null && fkCuentasTesoreria != "" && fkSeriesContables != "") {
                $scope.generarmovimientostesoreria();
            } else {
                alert("Cobrador y Serie deben tener valor");
            }

        } else {
            $scope.generarmovimientostesoreria();
            /*if (fkCuentasTesoreria != null && fkCuentasTesoreria != "") {
                
            } else {
                alert("Cobrador debe tener valor");
            }*/
        }
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
               var anularRemesa = $('#ReferenciaRemesa').val();
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

                   //Anular remesa se marcan todos los registros
                   if (anularRemesa != null && anularRemesa != "") {
                       var index = 0;
                       //$scope.gridApi.grid.modifyRows($scope.gridOptionsvencimientos.data);
                       $scope.gridOptionsvencimientos.data.forEach(function () {
                           $scope.gridApi.selection.selectRow($scope.gridOptionsvencimientos.data[index]);
                           index++;
                       });
                       //$scope.gridOptionsvencimientos = { enableRowSelection: false, enableRowHeaderSelection: false, enableCellEdit: false };
                   }

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
        if ($scope.urlUltimaSituacion != undefined) {
            $http.get($scope.urlUltimaSituacion + "?circuito=" + JSON.stringify(message)).success(function (data) {
                var modeloSituacion = JSON.parse(data);
                $('#SituacionFinal').val(modeloSituacion.Cod);
                window.document.getElementById("cv-SituacionFinal-descripcion").textContent = modeloSituacion.Descripcion;
            }).error(function (error) {
                console.log(error);
            });
        }
    });


    //api cartera
    $scope.generarmovimientostesoreria = function () {

        var rows = $scope.gridApi.selection.getSelectedRows();

        var vencimientosId = $.map(rows, function (v) {
            return v.Id;
        }).join(';');

        if ($scope.importeCuentaCargo2Necesario && $("#ImporteCargo2").val() == "" || $("#ImporteCargo2").val() == null) {
            $("#ImporteCargo2").val(0);
        }
        if ($scope.importeCuentaAbono2Necesario && $("#ImporteAbono2").val() == "" || $("#ImporteAbono2").val() == null) {
            $("#ImporteAbono2").val(0);
        }

        var tipo = $("#Tipo").val();
        var cuircuitotesoreria = $("#Circuitotesoreria").val();
        var fkmodopago = $("#Fkmodospago").val();
        var tercerodesde = $("#TerceroDesde").val();
        var tercerohasta = $("#TerceroHasta").val();
        var fechadesde = $("#FechaDesde").val();
        var fechahasta = $("#FechaHasta").val();
        var fechacontable = $("#FechaContable").val();
        var fkcuentatesoreria = $("#Fkcuentatesoreria").val();
        var fkseriescontables = $("#Fkseriescontables").val();
        var fecharemesa = $("#FechaRemesa").val();
        var importecargo2 = $("#ImporteCargo2").val();
        var importeabono2 = $("#ImporteAbono2").val();
        var banco = $("#Banco").val();
        var letra = $("#Letra").val();
        var fechapago = $("#FechaPago").val();
        var comentario = $("#Comentario").val();

        $("#movimientostesoreriaform input[name='Tipo']").val(tipo);
        $("#movimientostesoreriaform input[name='Circuitotesoreria']").val(cuircuitotesoreria);
        $("#movimientostesoreriaform input[name='Fkmodospago']").val(fkmodopago);
        $("#movimientostesoreriaform input[name='TerceroDesde']").val(tercerodesde);
        $("#movimientostesoreriaform input[name='TerceroHasta']").val(tercerohasta);
        $("#movimientostesoreriaform input[name='FechaDesde']").val(fechadesde);
        $("#movimientostesoreriaform input[name='FechaHasta']").val(fechahasta);
        $("#movimientostesoreriaform input[name='FechaContable']").val(fechacontable);
        $("#movimientostesoreriaform input[name='Vencimientos']").val(vencimientosId);
        $("#movimientostesoreriaform input[name='Fkcuentatesoreria']").val(fkcuentatesoreria);
        $("#movimientostesoreriaform input[name='Fecharemesa']").val(fecharemesa);
        $("#movimientostesoreriaform input[name='Fkseriescontables']").val(fkseriescontables);
        $("#movimientostesoreriaform input[name='ImporteCargo2']").val(importecargo2);
        $("#movimientostesoreriaform input[name='ImporteAbono2']").val(importeabono2);
        $("#movimientostesoreriaform input[name='Banco']").val(banco);
        $("#movimientostesoreriaform input[name='Letra']").val(letra);
        $("#movimientostesoreriaform input[name='FechaPago']").val(fechapago);
        $("#movimientostesoreriaform input[name='Comentario']").val(comentario);

        $("#movimientostesoreriaform").submit();
    }
    //end api movimientos tesoreria
}]);


