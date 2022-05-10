
app.controller('EntregaStockCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.Urlapi = "";
    $scope.Urlapiagregar = "";
    $scope.Urlultimoprecio = "";

    $scope.Fkarticulos = "";
    $scope.Descripcion = "";
    $scope.Fkalmacen = "";
    $scope.Loteautomatico = true;
    $scope.Editarloteautomatico = true;
    $scope.Lote = "";
    $scope.Cantidad = 1;
    $scope.Editarmetros = false;
    $scope.Editarlargo = true;
    $scope.Largo = 0;
    $scope.Editarancho = true;
    $scope.Ancho = 0;
    $scope.Editargrueso = true;
    $scope.Editarcantidad = true;
    $scope.Editarprecio = true;
    $scope.Editardescuento = true;
    $scope.Grueso = 0;
    $scope.Metros = 0;
    $scope.Precio = 0;
    $scope.Descuentoarticulo = "0";
    $scope.Subtotal = 0;
    $scope.Decimalesmonedas = 2;
    $scope.Decimalesmedidas = 2;
    $scope.Fkunidades = "";
    $scope.Fktiposiva = "";
    $scope.Unidades = "";
    $scope.Formulas = "";
    $scope.Tipofamilia = 0;
    $scope.Lotesautomaticos = [];
    $scope.Loteautomaticoid = "";
    $scope.Tipodocumento = "";
    $scope.Bundle = "";
    $scope.Gestionlotes = true;
    $scope.Tipogestionlotes = 0;
    $scope.Mostrardetalle = false;
    $scope.Tipopieza = 0;
    $scope.Modificarmedidas = true;
    $scope.Canal = "";
    $scope.Caja = "";
    //errores
    $scope.Fkarticuloserrores = "";
    $scope.Cantidaderrores = "";
    $scope.Largoerrores = "";
    $scope.Anchoerrores = "";
    $scope.Gruesoerrores = "";
    $scope.Precioerrores = "";
    $scope.Generalerrores = "";
    $scope.Loteerrores = "";
    $scope.Lotedetransformacion = "";
    //grid
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

    $scope.Carga = false;
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            if ($scope.Tipopieza >= 1) {
                $scope.gridApi.selection.selectAllRows();
            }
            $scope.selected = $scope.gridApi.selection.getSelectedCount();

        });
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            if ($scope.Tipopieza >= 1) {
                $scope.gridApi.selection.selectAllRows();
            }
            $scope.selected = $scope.gridApi.selection.getSelectedCount();


        });
    };

    $scope.gridOptions.rowTemplate = '<div role=\"gridcell\"   ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
        }

    };

    $scope.init = function (url, urllotes, urlultimoprecio, tipodocumento) {
        console.log("llega al init");
        $scope.Urlapi = url;
        $scope.Urlapiagregar = urllotes;
        $scope.Urlultimoprecio = urlultimoprecio;
        $scope.Tipodocumento = tipodocumento;
        $scope.Lotesautomaticos = [{ IdLote: "", Texto: "<Lote automático>" }];
        eventAggregator.RegisterEvent("Precioarticulo-Buscar", function (valor) {
            $scope.Precio = valor;
        });
    }

    $scope.ClearFormulario = function () {
        $scope.Bundle = "";
        eventAggregator.Publish("Lote-Buscar", "");
        $scope.data = [];
        $scope.gridOptions.data = [];
        $scope.Fkarticuloserrores = "";
        $scope.Cantidaderrores = "";
        $scope.Largoerrores = "";
        $scope.Anchoerrores = "";
        $scope.Gruesoerrores = "";
        $scope.Precioerrores = "";
        $scope.Generalerrores = "";
        $scope.lote
        $scope.Mostrardetalle = false;
        $scope.Modificarmedidas = true;
        $("[name='Loteentrega']").val("");
        $("[name='Fkarticulossalida']").val("");
    }

    $scope.GridVista = function () {
        return typeof GridViewLineas === 'undefined' || GridViewLineas === null ? GridViewLineasSalida : GridViewLineas;
    }

    $('#_entregastock').on('show.bs.modal', function () {
        console.log("nueva linea js show modal");
        $scope.Carga = true;
        $scope.ClearFormulario();

    });
    $('#_entregastock').on('hidden.bs.modal', function () {
        $scope.GridVista().Focus();
    });

    eventAggregator.RegisterEvent("_nuevalinea", function (ms) {
        console.log("nueva linea js");
        $('#_entregastock').show('toggle');
    });

    $scope.Buscarlote = function () {
        console.log("buscar lote");
        $scope.loading = true;
        var parametros = {
            Fkalmacen: $("[name='Fkalmacen']").val(),
            FkarticulosDesde: $("[name='Fkarticulossalida']").val(),
            FkarticulosHasta: $("[name='Fkarticulossalida']").val(),
            Id: $("[name='Loteentrega']").val(),
            Flujo: '0'
        };

        $http({
            url: $scope.Urlapi, method: "GET", params: parametros
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
                if (response.values.length > 0) {
                    $scope.Largo = response.values[0]["Largo"];
                    $scope.Ancho = response.values[0]["Ancho"];
                    $scope.Grueso = response.values[0]["Grueso"];
                    $scope.Metros = response.values[0]["Metros"];
                }

                $timeout(function () {
                    if (response.values.length == 1) {
                        $scope.gridApi.selection.selectAllRows();
                    }
                    if ($scope.Tipopieza >= 1) {
                        $scope.gridApi.selection.selectAllRows();
                        $scope.gridOptions.noUnselect = true;
                    }
                    else
                        $scope.gridOptions.noUnselect = false;
                    $scope.gridApi.core.refresh();
                    $scope.gridApi.core.handleWindowResize();
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

    eventAggregator.RegisterEvent("Loteentrega", function (ms) {
        if (ms) {
            $scope.Mostrardetalle = true;
        } else {
            $scope.Mostrardetalle = false;
        }
    });

    eventAggregator.RegisterEvent("Loteentrega-cv", function (ms) {
        eventAggregator.Publish("Fkarticulossalida-Buscar", ms.Fkarticulos);
        $scope.Buscarlote();
        $scope.Tipopieza = ms.Tipopieza;
        $scope.Mostrardetalle = ms.Tipopieza != 2;
        $scope.Cantidad = ms.Cantidad;

    });

    eventAggregator.RegisterEvent("Fkarticulossalida-cv", function (ms) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        if (ms) {
            $scope.Fkarticulos = ms.Id;
            $scope.Descripcion = ms.Descripcion;
            $scope.Decimalesmedidas = ms.Decimalestotales;
            $scope.Decimalesmonedas = ms.Decimalesmonedas;
            $scope.Editarlargo = ms.Permitemodificarlargo;
            $scope.Largo = Funciones.RedondearGlobalize(ms.Largo, $scope.Decimalesmedidas);
            $scope.Editarancho = ms.Permitemodificarancho;
            $scope.Ancho = Funciones.RedondearGlobalize(ms.Ancho, $scope.Decimalesmedidas);
            $scope.Editargrueso = ms.Permitemodificargrueso;
            $scope.Editarmetros = ms.Permitemodificarmetros;
            $scope.Grueso = Funciones.RedondearGlobalize(ms.Grueso, $scope.Decimalesmedidas);
            $scope.Precio = Funciones.RedondearGlobalize(ms.Precio, $scope.Decimalesmonedas);
            $scope.Descuentoarticulo = Funciones.RedondearGlobalize(ms.Descuento, $scope.Decimalesmonedas);
            $scope.Fktiposiva = "";
            $scope.Fkunidades = ms.Fkunidades;
            $scope.Unidades = ms.Unidadesdescripcion;
            $scope.Formulas = ms.Formulas;
            $scope.Fktiposiva = ms.Fktiposiva;
            $scope.Tipofamilia = ms.Tipofamilia;
            $scope.Gestionlotes = ms.Tipogestionlotes != 0;
            $scope.Lotefraccionable = ms.Lotefraccionable;
            $scope.Tipogestionlotes = ms.Tipogestionlotes;
            $scope.Editarcantidad = !ms.Articulocomentario;
            $scope.Editarprecio = !ms.Articulocomentario;
            $scope.Editardescuento = !ms.Articulocomentario;
            $scope.Editarloteautomatico = ms.Fkcontador && ms.Fkcontador != null;
            $scope.Loteautomatico = ms.Fkcontador && ms.Fkcontador != null;
            $scope.Modificarmedidas = ms.Tipofamilia < 2;

            if (ms.Tipofamilia == 1) {
                $scope.Cantidad = 1;
            }

            if (!$scope.Gestionlotes) {
                $scope.Cantidad = 1;
            }

            if (ms.Articulocomentario) {
                $scope.Cantidad = "0";
                $scope.Precio = "0";
                $scope.Descuentoarticulo = "0";
            }
            $scope.RecalculaMetrosSubtotal();
        }

    });



    $scope.$watch("Loteautomatico", function (value, old) {
        $scope.Lote = "";
        if (value && $scope.Lotedetransformacion)
            $scope.Lote = $scope.Lotedetransformacion;

    });

    $scope.$watch("Cantidad", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Largo", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Ancho", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Grueso", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });
    $scope.$watch("Metros", function (value, old) {
        var metros = isNaN($scope.Metros) ? Globalize.parseFloat($scope.Metros) : $scope.Metros;
        var bruto = Globalize.parseFloat($scope.Precio) * metros;
        var descuento = Globalize.parseFloat($scope.Descuentoarticulo);
        bruto = bruto - (bruto * descuento / 100.0);
        $scope.Subtotal = Funciones.RedondearNumerico(bruto, 2);
    });
    $scope.$watch("Precio", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Descuentoarticulo", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.RecalculaMetrosSubtotal = function () {
        if (!$scope.Carga) return;
        var largo = isNaN($scope.Largo) ? Globalize.parseFloat($scope.Largo) : $scope.Largo;
        var ancho = isNaN($scope.Ancho) ? Globalize.parseFloat($scope.Ancho) : $scope.Ancho;
        var grueso = isNaN($scope.Grueso) ? Globalize.parseFloat($scope.Grueso) : $scope.Grueso;
        var metros = isNaN($scope.Metros) ? Globalize.parseFloat($scope.Metros) : $scope.Metros;
        $scope.Metros = Funciones.RedondearNumerico(FFormulasService.CreateFormula($scope.Formulas).calculate($scope.Cantidad, largo, ancho, grueso, metros, $scope.Decimalesmedidas), $scope.Decimalesmedidas);
        metros = isNaN($scope.Metros) ? Globalize.parseFloat($scope.Metros) : $scope.Metros;
        var bruto = Globalize.parseFloat($scope.Precio) * metros;
        var descuento = Globalize.parseFloat($scope.Descuentoarticulo);
        bruto = bruto - (bruto * descuento / 100.0);
        $scope.Subtotal = Funciones.RedondearNumerico(bruto, 2);
    }

    $scope.SearchPrecios = function () {

        if ($scope.Fkarticulos && $scope.Fkarticulos != "") {
            var obj = {
                campoIdentificador: "Precio",
                IdComponenteasociado: "Precioarticulo",
                IdFormulariomodal: "ultimoprecio",
                Url: $scope.Urlultimoprecio,
                Titulo: "Últimos precios",
                Params: "{\"fkcuenta\":\"" + $("[name='Fkclientes']").val() + "\",\"articulo\":\"" + $scope.Fkarticulos + "\",\"tipodocumento\":\"" + $scope.Tipodocumento + "\",\"cliente\":\"" + $("#cv-Fkclientes-descripcion").html() + "\",\"descripcion\":\"" + $scope.Descripcion + "\"}"
            };

            eventAggregator.Publish("_lanzarbusquedaultimoprecio", obj);
        }

    }

    eventAggregator.RegisterEvent("Precioarticulo-Buscar", function (ms) {
        $scope.Precio = ms.toLocaleString();
        $scope.RecalculaMetrosSubtotal();
    });

    eventAggregator.RegisterEvent("setloteBloqueSalida", function (msg) {
        $scope.Lotedetransformacion = msg;

    });

    $scope.ValidarDatos = function () {
        console.log("Validando datos");
        var lineas = $scope.gridApi.selection.getSelectedRows();
        $scope.Fkarticuloserrores = "";
        $scope.Cantidaderrores = "";
        $scope.Largoerrores = "";
        $scope.Anchoerrores = "";
        $scope.Gruesoerrores = "";
        $scope.Precioerrores = "";
        $scope.Generalerrores = "";
        var resultado = true;
        console.log($scope.Tipopieza);
        if ($scope.Tipopieza == 0) {
            var campoobligatorio = "Este campo es obligatorio";
            $scope.Fkarticulos = $("#Fkarticulossalida").val();
            console.log($scope.Fkarticulos);
            if ($scope.Fkarticulos == "" || $scope.Fkarticulos == undefined) {
                resultado = false;
                $scope.Fkarticuloserrores = campoobligatorio; NuevaLineaEntregaStockController
            }

            if (!$scope.Cantidad) {

                resultado = false;
                $scope.Cantidaderrores = campoobligatorio;
            }
            else if ($scope.Gestionlotes && lineas[0]["Cantidad"] < $scope.Cantidad) {
                resultado = false;
                $scope.Cantidaderrores = "La cantidad indicada es superior a la que hay actualmente en el stock";
            }

            if ($scope.Modificarmedidas && !isNaN($scope.Metros)) {
                if ($scope.Largo < 0 || $scope.Largo == undefined) {
                    resultado = false;
                    $scope.Largoerrores = campoobligatorio;
                }
                if ($scope.Ancho < 0 || $scope.Ancho == undefined) {
                    resultado = false;
                    $scope.Anchoerrores = campoobligatorio;
                }
                if ($scope.Grueso < 0 || $scope.Grueso == undefined) {
                    resultado = false;
                    $scope.Gruesoerrores = campoobligatorio;
                }
            }

            if ($scope.Precio == "" && $scope.Tipopieza < 2) {
                resultado = false;
                $scope.Precioerrores = campoobligatorio;
            }

            if ($("[name='Fkalmacen']").val() == "") {
                resultado = false;
                $scope.Generalerrores = "No ha indicado ningún almacén para la entrada";
            }

        }


        return resultado;
    }

    $scope.Aceptar = function () {

        if ($scope.ValidarDatos()) {
            $scope.EnviarDatos();
        }
    }

    $scope.EnviarDatos = function () {
        var Model = {};
        console.log("Enviando datos");
        $scope.Fkarticulos = $("[name='Fkarticulossalida']").val();
        Model.Fkarticulos = $("[name='Fkarticulossalida']").val();
        Model.Fkalmacen = $("[name='Fkalmacen']").val();
        Model.Fkzonas = $("[name='Fkzonas']").val();
        Model.Lote = $("[name='Loteentrega']").val();
        Model.Modificarmedidas = $scope.Modificarmedidas;
        Model.Cantidad = $scope.Cantidad;
        Model.Largo = $scope.Largo;
        Model.Ancho = $scope.Ancho;
        Model.Grueso = $scope.Grueso;
        Model.Metros = $scope.Metros;
        Model.Precio = $scope.Precio;
        Model.Descuento = $scope.Descuentoarticulo;
        Model.Descuentoprontopago = $("[name='Porcentajedescuentoprontopago']").val();
        Model.Descuentocomercial = $("[name='Porcentajedescuentocomercial']").val();
        Model.Decimalesmonedas = $scope.Decimalesmonedas;
        Model.Portes = $("[name='Costeportes']").val();
        Model.Lineas = $scope.gridApi.selection.getSelectedRows();
        Model.Tipopieza = $scope.Tipopieza;
        Model.Fkcuenta = $("[name='Fkclientes']").val();
        Model.Fkmonedas = $("[name='Fkmonedas']").val();
        Model.Fkregimeniva = $("[name='Fkregimeniva']").val();
        Model.Flujo = 0;
        Model.Caja = $scope.Caja;
        Model.Tipofamilia = $scope.Tipofamilia;
        Model.Canal = $scope.Canal;
        var req = {
            method: 'POST',
            url: $scope.Urlapiagregar,
            data: JSON.stringify(Model),
            contentType: 'application/json',
            dataType: 'json'
        }
        $http(req)
            .success(function (response) {
                if (response.error) {
                    $scope.Generalerrores = response.error;
                } else {
                    eventAggregator.Publish("transformacionSalidaLinea", Model);
                    $scope.GridVista().Refresh();
                    if (typeof GridViewTotales !== 'undefined' && GridViewTotales !== null)
                        GridViewTotales.Refresh();
                    $('#_entregastock').modal('hide');

                }



            }).error(function (data, status, headers, config, statusText) {
                $scope.Errorgeneral = data.error;
            });
    }

}]);

