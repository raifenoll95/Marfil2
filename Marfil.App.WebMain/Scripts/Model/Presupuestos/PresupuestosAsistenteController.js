//Rai

app.controller('PresupuestosAsistenteCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    $scope.generarComponentes = true;
    $scope.componentesprevios;
    $scope.presupuestoId;
    $scope.IntegridadreferencialId;
    $scope.Idlineaarticulo;
    $scope.urlComponentes;
    $scope.urlPrecio;
    $scope.urlPresupuestar;
    $scope.urlObtenerMedida;
    $scope.urlIndex;
    $scope.componentes;
    $scope.cantidad;
    $scope.LargoPadre;
    $scope.AnchoPadre;
    $scope.GruesoPadre;
    $scope.Precio;

    //Init
    $scope.init = function (urlComponentes, urlPrecio, urlPresupuestar, urlObtenerMedida, urlIndex) {
        $scope.urlComponentes = urlComponentes;
        $scope.urlPrecio = urlPrecio;
        $scope.urlPresupuestar = urlPresupuestar;
        $scope.urlObtenerMedida = urlObtenerMedida;
        $scope.urlIndex = urlIndex;
    }

    eventAggregator.RegisterEvent("_buscararticulospresupuesto", function (data) {
        $scope.params = data.Params;
        $scope.presupuestoId = JSON.parse(data.Params).PresupuestoId;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, data.IdFormulariomodal, data.Url, data.Titulo);
    });

    eventAggregator.RegisterEvent("_generarComponentes", function (data) {

        if ($scope.gridApi.selection.getSelectedCount() > 1) {
            bootbox.alert("Sólo es posible seleccionar un artículo");
            $scope.generarComponentes = false;
        }

        else {

            var fila = $scope.gridApi.selection.getSelectedRows();

            $scope.IntegridadreferencialId = fila[0].Integridadreferenciaflag;
            $scope.Idlineaarticulo = fila[0].Id;
            $scope.cantidad = fila[0].Cantidad;
            $scope.LargoPadre = fila[0].Largo;
            $scope.AnchoPadre = fila[0].Ancho;
            $scope.GruesoPadre = fila[0].Grueso;

            $scope.LargoPadre = $scope.LargoPadre.toString().replace(",", ".");
            $scope.AnchoPadre = $scope.AnchoPadre.toString().replace(",", ".");
            $scope.GruesoPadre = $scope.GruesoPadre.toString().replace(",", ".");

            $http({
                url: $scope.urlComponentes, method: "GET", params: { articulo: fila[0].Fkarticulos, presupuestoId: $scope.presupuestoId, lineaarticulo: $scope.Idlineaarticulo }
            }).success(function (data) {

                $scope.componentes = JSON.parse(data.listadoComponentes);
                $scope.componentesprevios = JSON.parse(data.componentesprevios);

                if (!$scope.componentesprevios) {

                    $scope.componentes.forEach(function (componente) {
                        componente.Piezas = componente.Piezas * $scope.cantidad;

                        $http({
                            url: $scope.urlPrecio, method: "GET", params: { componente: componente.IdComponente, unidadmedida: componente.UnidadMedida }
                        }).success(function (data) {
                            var precio = JSON.parse(data);

                            componente.Largo = componente.Largo.toString().replace(",", ".");
                            componente.Ancho = componente.Ancho.toString().replace(",", ".");
                            componente.Grueso = componente.Grueso.toString().replace(",", ".");
                            componente.PrecioInicial = precio;

                            $scope.CalcularPrecioMetros(componente);

                        }).error(function (error) {
                            console.log("error call ");
                        });
                    })
                }

                $scope.componentes.forEach(function (componente) {
                    componente["showEdit"] = true;
                    $scope.CalcularMetros(componente);
                })
            
            }).error(function (error) {
                console.log("error call ");
            });                 
        }        
    });

    $scope.toggleEdit = function (componente) {
        componente.showEdit = componente.showEdit ? false : true;

        //Recalculamos metros y precio
        if (componente.showEdit == true) {
            $scope.CalcularPrecioMetros(componente);
        }

        componente.BuscarArticulo = false;
    }

    //MOD Nov2022 - Cambiamos el redondeo a 3 decimales
    $scope.CalcularMetros = function (componente) {
        $http({
            url: $scope.urlObtenerMedida, method: "GET", params: { idArticulo: componente.IdComponente }
        }).success(function (data) {
            var unidad = data.unidad;

            if (unidad == "01") {
                componente.Metros = componente.Piezas;
            }

            //Precio = Precio * cantidad * Metros (largo * ancho)
            else if (unidad == "02") {
                componente.Metros = (componente.Piezas * componente.Largo * componente.Ancho).toFixed(3);
            }

            //Precio = Precio * cantidad * Metros (largo * ancho * grueso)
            else if (unidad == "03") {
                componente.Metros = (componente.Piezas * componente.Largo * componente.Ancho * componente.Grueso).toFixed(3);
            }

            //Precio = Precio * cantidad * (largo)
            else if (unidad == "05") {
                componente.Metros = (componente.Piezas * componente.Largo).toFixed(3);
            }

            else {
                bootbox.alert("no se ha especificado la medida para el articulo " + componente.IdComponente);
            }

        }).error(function (error) {
            console.log("error call ");
        });
    }

    //Recalculamos metros y precio
    /*MOD Nov2022 - Se modifican las condiciones que permiten editar las medidas según la UM porque estaban justo al contrario
    y se comentan la igualaciones que no tienen sentido. Cambiamos el redondeo de los metros a 3 decimales*/
    $scope.CalcularPrecioMetros = function (componente) {

        $http({
            url: $scope.urlObtenerMedida, method: "GET", params: { idArticulo: componente.IdComponente }
        }).success(function (data) {
            var unidad = data.unidad;

            if (unidad == "01") {
                componente.Largo = 0; //$scope.LargoPadre.toString().replace(",", ".");
                componente.Ancho = 0; //$scope.AnchoPadre.toString().replace(",", ".");
                componente.Grueso = 0; //$scope.GruesoPadre.toString().replace(",", ".");
                componente.Metros = componente.Piezas;
                componente.Precio = (componente.PrecioInicial * componente.Metros).toFixed(2);
                var incrementomerma = ((componente.Merma * componente.Precio) / 100).toFixed(2);
                componente.Precio = (parseFloat(componente.Precio) + parseFloat(incrementomerma)).toFixed(2);
            }

            //Precio = Precio * cantidad * Metros (largo * ancho)
            else if (unidad == "02") {
                componente.Largo = componente.Largo;
                componente.Ancho = componente.Ancho;
                componente.Grueso = $scope.GruesoPadre.toString().replace(",", ".");
                componente.Metros = (componente.Piezas * componente.Largo * componente.Ancho).toFixed(3);
                componente.Precio = (componente.PrecioInicial * componente.Metros).toFixed(2);
                var incrementomerma = ((componente.Merma * componente.Precio) / 100).toFixed(2);
                componente.Precio = (parseFloat(componente.Precio) + parseFloat(incrementomerma)).toFixed(2);
            }

            //Precio = Precio * cantidad * Metros (largo * ancho * grueso)
            else if (unidad == "03") {
                componente.Largo = componente.Largo;
                componente.Ancho = componente.Ancho;
                componente.Grueso = componente.Grueso;
                componente.Metros = (componente.Piezas * componente.Largo * componente.Ancho * componente.Grueso).toFixed(3);
                componente.Precio = (componente.PrecioInicial * componente.Metros).toFixed(2);
                var incrementomerma = ((componente.Merma * componente.Precio) / 100).toFixed(2);
                componente.Precio = (parseFloat(componente.Precio) + parseFloat(incrementomerma)).toFixed(2);
            }

            //Precio = Precio * cantidad * (largo)
            else if (unidad == "05") {
                componente.Largo = componente.Largo;
                componente.Ancho = 0; //$scope.AnchoPadre.toString().replace(",", ".");
                componente.Grueso = 0; //$scope.GruesoPadre.toString().replace(",", ".");
                componente.Metros = (componente.Piezas * componente.Largo).toFixed(3);
                componente.Precio = (componente.PrecioInicial * componente.Metros).toFixed(2);
                var incrementomerma = ((componente.Merma * componente.Precio) / 100).toFixed(2);
                componente.Precio = (parseFloat(componente.Precio) + parseFloat(incrementomerma)).toFixed(2);
            }

            else {
                bootbox.alert("no se ha especificado la medida para el articulo " + componente.IdComponente);
            }

        }).error(function (error) {
            console.log("error call ");
        });
    }

    //Ultima pantalla, van a generar los registros de cartera
    eventAggregator.RegisterEvent("_presupuestar", function (data) {
        $scope.presupuestar();
    });

    //Seccion del Scope Grid
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";
    $scope.Titulo = "";
    $scope.gridOptionsarticulos = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptionsarticulos = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true };
    $scope.gridOptionsarticulos.multiSelect = true;
    $scope.gridOptionsarticulos.modifierKeysToMultiSelect = true;
    $scope.gridOptionsarticulos.noUnselect = false;
    $scope.gridOptionsarticulos.selectionRowHeaderWidth = 35;
    $scope.gridOptionsarticulos.enableFiltering = true;
    $scope.cantidadarticulos = 0;

    //-------------SECCION GRID--------------

    $scope.gridOptionsarticulos.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;
            $scope.cantidadarticulos = $scope.gridApi.selection.getSelectedCount();
        });

        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.cantidadarticulos = $scope.gridApi.selection.getSelectedCount();
        });

        $scope.gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApi.selection.selectRow(newRowCol.row.entity);
        });
    };

    $scope.gridOptionsarticulos.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptionsarticulos.appScopeProvider = {
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
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptionsarticulos.columnDefs[colIndex]);
    };


    $scope.loading = true;
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {
        $scope.loading = true;
        $scope.cantidadarticulos = 0;
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

                    $scope.gridOptionsarticulos.columnDefs = response.columns;
                    $scope.gridOptionsarticulos.data = response.values;
                    $scope.gridApi.grid.enableHorizontalScrollbar = 2;
                    $scope.gridOptionsarticulos.minWidth = 150;
                    $scope.gridApi.core.refresh();
                    $scope.gridApi.core.handleWindowResize();
                    $scope.gridOptionsarticulos.core.refresh();
                    $scope.gridOptionsarticulos.core.notifyDataChange(uiGridConstants.dataChange.ALL);
                    $timeout(function () {

                        $("[name='columnheader-0']")[0].focus();
                        $("[name='columnheader-0']").on("keydown", function (e) {
                            if (e.keyCode === 40) {
                                $scope.scrollToFocus(0, 0);
                            }
                        });

                    });
                }).error(function (data, status, headers, config) {
                    $scope.gridOptionsarticulos.data = [];
                    $scope.loading = false;
                });
        }
        else {
            $http.get($scope.Url)
                .success(function (response) {
                    $scope.selected = null;

                    $scope.gridApi.grid.clearAllFilters();
                    $scope.loading = false;
                    $scope.gridOptionsarticulos.columnDefs = response.columns;
                    $scope.gridOptionsarticulos.data = response.values;
                    $scope.gridApi.grid.enableHorizontalScrollbar = 2;
                    $scope.gridOptionsarticulos.minWidth = 150;
                    $scope.gridApi.core.refresh();
                    $scope.gridApi.core.handleWindowResize();
                    $scope.gridOptionsarticulos.core.refresh();
                    $timeout(function () {

                        $("[name='columnheader-0']")[0].focus();
                        $("[name='columnheader-0']").on("keydown", function (e) {
                            if (e.keyCode === 40) {
                                $scope.scrollToFocus(0, 0);
                            }
                        });

                    });
                }).error(function (data, status, headers, config) {
                    $scope.gridOptionsarticulos.data = [];
                    $scope.loading = false;
                });
        }

        var refresh = function () {
            $scope.refresh = true;
            $timeout(function () {
                $scope.refresh = false;
            }, 0);
        };
    }

    //Añadir linea
    $scope.Nuevalinea = function () {

        var Model = {};
        Model.IdComponente = 0;
        Model.DescripcionComponente = 0;
        Model.Piezas = 0;
        Model.Largo = 0;
        Model.Ancho = 0;
        Model.Grueso = 0;
        Model.Merma = 0;
        Model.Precio = 0;
        Model.BuscarArticulo = true;
        $scope.componentes.push(Model);
    }

    $scope.BuscarComponente = function () {

        $("#_entradastock").modal('show');

        //resetear valores
        $("#Fkarticulosentrada").val("");
        document.getElementById("Cantidad").innerHTML = ($scope.cantidad);
        document.getElementById("Largo").innerHTML = ($scope.LargoPadre);
        document.getElementById("Ancho").innerHTML = ($scope.AnchoPadre);
        document.getElementById("Grueso").innerHTML = ($scope.GruesoPadre);
        document.getElementById("Precio").innerHTML = "";
    }

    $scope.AceptarComponente = function () {

        $scope.componentes.forEach(function (componente) {
            //El que se está editando
            if (componente.BuscarArticulo) {
                componente.IdComponente = $("#Fkarticulosentrada").val();
                componente.DescripcionComponente = window.document.getElementById("cv-Fkarticulosentrada-descripcion").textContent;

                componente.Piezas = !$scope.isNullOrEmpty(document.getElementById("Cantidad").value) ? document.getElementById("Cantidad").value : 0;
                componente.Largo = !$scope.isNullOrEmpty(document.getElementById("Largo").value) ? parseFloat(document.getElementById("Largo").value.toString().replace(",", ".")) : 0;
                componente.Ancho = !$scope.isNullOrEmpty(document.getElementById("Ancho").value) ? parseFloat(document.getElementById("Ancho").value.toString().replace(",", ".")) : 0;
                componente.Grueso = !$scope.isNullOrEmpty(document.getElementById("Grueso").value) ? parseFloat(document.getElementById("Grueso").value.toString().replace(",", ".")) : 0;
                componente.Merma = !$scope.isNullOrEmpty(document.getElementById("Merma").value) ? parseFloat(document.getElementById("Merma").value.toString().replace(",", ".")) : 0;
                componente.PrecioInicial = !$scope.isNullOrEmpty(document.getElementById("Precio").value) ? parseFloat(document.getElementById("Precio").value.toString().replace(",", ".")) : 0;

                $scope.CalcularPrecioMetros(componente);
                componente["showEdit"] = true;
            }
            componente.BuscarArticulo = false;
        });

        $("#_entradastock").modal('hide');
    }


    $scope.isNullOrEmpty = function (parametro) {
        return (!parametro);
    }


    //api presupuestos
    $scope.presupuestar = function () {
        var req = {
            method: 'POST',
            url: $scope.urlPresupuestar,
            data: JSON.stringify({
                componentes: $scope.componentes, PresupuestoId: $scope.presupuestoId,
                integridadreferencial: $scope.IntegridadreferencialId, idArticulo: $scope.Idlineaarticulo}),
            contentType: 'application/json',
            dataType: 'json'
        }

        $http(req).
            success(function (response) {
                window.location.replace($scope.urlIndex);
            }).error(function (data, status, headers, config, statusText) {
                $scope.Errorgeneral = data.error;
            });
    }

    //end api presupesto / fabricar
}]);


