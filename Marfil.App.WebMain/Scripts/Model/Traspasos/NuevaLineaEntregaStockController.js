
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
    $scope.Modificarmedidas = false;
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

    $scope.gridOptions.rowTemplate = '<div role=\"gridcell\"  ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptions.appScopeProvider = {
        onDblClick: function (row) {
            $scope.selected = row;
            $scope.aceptar();
        }

    };

    $scope.init = function(url,urllotes,urlultimoprecio,tipodocumento) {
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
        eventAggregator.Publish("Lote-Buscar","");
        $scope.data = [];
        $scope.gridOptions.data = [];
        $scope.Fkarticuloserrores = "";
        $scope.Cantidaderrores = "";
        $scope.Largoerrores = "";
        $scope.Anchoerrores = "";
        $scope.Gruesoerrores = "";
        $scope.Precioerrores = "";
        $scope.Generalerrores = "";
        $scope.Mostrardetalle = false;
        $scope.Modificarmedidas = false;
    }

    $('#_entregastock').on('show.bs.modal', function () {
        $scope.ClearFormulario();
        
    });
    $('#_entregastock').on('hidden.bs.modal', function () {
        GridViewLineas.Focus();
    });

    eventAggregator.RegisterEvent("_nuevalinea", function (ms) {

        $('#_entregastock').show('toggle');
    });

    $scope.Buscarlote =function() {

        $scope.loading = true;
        var parametros = {
            Fkalmacen: $("[name='Fkalmacen']").val(),
            FkarticulosDesde: $("[name='Fkarticulosentrada']").val(),
            FkarticulosHasta: $("[name='Fkarticulosentrada']").val(),
            Id: $("[name='Lote']").val()
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
                   $scope.gridOptions.noUnselect= true;
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

    eventAggregator.RegisterEvent("Lote", function (ms) {
        
            $scope.Mostrardetalle = false;
        
    });

    eventAggregator.RegisterEvent("Lote-cv", function(ms) {
        eventAggregator.Publish("Fkarticulosentrada-Buscar", ms.Fkarticulos);
        $scope.Tipopieza = ms.Tipopieza;
        $scope.Mostrardetalle = false;
        $scope.Buscarlote();
    });

    eventAggregator.RegisterEvent("Fkarticulosentrada-cv", function (ms) {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        if (ms) {
            $scope.Fkarticulos = ms.Id;
            $scope.Descripcion = ms.Descripcion;
            $scope.Cantidad = 1;
            $scope.Decimalesmedidas = ms.Decimalestotales;
            $scope.Decimalesmonedas = ms.Decimalesmonedas;
            $scope.Editarlargo = ms.Permitemodificarlargo;
            $scope.Largo = Funciones.RedondearGlobalize(ms.Largo, $scope.Decimalesmedidas);
            $scope.Editarancho = ms.Permitemodificarancho;
            $scope.Ancho = Funciones.RedondearGlobalize(ms.Ancho, $scope.Decimalesmedidas);
            $scope.Editargrueso = ms.Permitemodificargrueso;
            $scope.Grueso = Funciones.RedondearGlobalize(ms.Grueso, $scope.Decimalesmedidas);
            $scope.Precio = Funciones.RedondearGlobalize(ms.Precio, $scope.Decimalesmonedas);
            $scope.Fktiposiva = "";
            $scope.Fkunidades = ms.Fkunidades;
            $scope.Unidades = ms.Unidadesdescripcion;
            $scope.Formulas = ms.Formulas;
            $scope.Fktiposiva = ms.Fktiposiva;
            $scope.Tipofamilia = ms.Tipofamilia;
            $scope.Gestionlotes = ms.Tipogestionlotes != 0;
            $scope.Tipogestionlotes = ms.Tipogestionlotes;
            $scope.Editarcantidad = !ms.Articulocomentario;
            $scope.Editarprecio = !ms.Articulocomentario;
            $scope.Editardescuento = !ms.Articulocomentario;
            $scope.Editarloteautomatico = ms.Fkcontador && ms.Fkcontador!=null;
            $scope.Loteautomatico = ms.Fkcontador && ms.Fkcontador != null;
            $scope.Modificarmedidas = ms.Tipofamilia < 2;

            if (ms.Tipofamilia == 1) {
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
    });

    $scope.$watch("Cantidad", function (value,old) {
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
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Precio", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.$watch("Descuentoarticulo", function (value, old) {
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.RecalculaMetrosSubtotal = function () {
        Globalize.cultureSelector = $("meta[name='accept-language']").prop("content");
        $scope.Metros = Funciones.RedondearGlobalize(FFormulasService.CreateFormula($scope.Formulas).calculate($scope.Cantidad, Globalize.parseFloat($scope.Largo), Globalize.parseFloat($scope.Ancho), Globalize.parseFloat($scope.Grueso),Globalize.parseFloat($scope.Metros), $scope.Decimalesmedidas), $scope.Decimalesmedidas);
    }

    $scope.SearchPrecios = function(){
       
        if ($scope.Fkarticulos && $scope.Fkarticulos!="") {
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

    eventAggregator.RegisterEvent("Precioarticulo-Buscar",function(ms) {
        $scope.Precio = ms.toLocaleString();
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.ValidarDatos = function () {

        var lineas = $scope.gridApi.selection.getSelectedRows();
        $scope.Fkarticuloserrores = "";
        $scope.Cantidaderrores = "";
        $scope.Largoerrores = "";
        $scope.Anchoerrores = "";
        $scope.Gruesoerrores = "";
        $scope.Precioerrores = "";
        $scope.Generalerrores = "";
        var resultado = true;
        var campoobligatorio = "Este campo es obligatorio";
        $scope.Fkarticulos = $("#Fkarticulosentrada-cv").val();
        if ($scope.Fkarticulos == "") {
            resultado = false;
            $scope.Fkarticuloserrores = campoobligatorio;
        }

        if (!$scope.Cantidad) {
            resultado = false;
            $scope.Cantidaderrores = campoobligatorio;
        }
        else if ($scope.Gestionlotes && lineas[0]["Cantidad"] < $scope.Cantidad) {
            resultado = false;
            $scope.Generalerrores = "La cantidad indicada es superior a la que hay actualmente en el stock";
        }
        
        if ($scope.Modificarmedidas) {
            if (!$scope.Largo) {
                resultado = false;
                $scope.Largoerrores = campoobligatorio;
            }
            if (!$scope.Ancho) {
                resultado = false;
                $scope.Anchoerrores = campoobligatorio;
            }
            if (!$scope.Grueso) {
                resultado = false;
                $scope.Gruesoerrores = campoobligatorio;
            }
        }

        if ($("[name='Fkalmacen']").val() == "") {
            resultado = false;
            $scope.Generalerrores = "No ha indicado ningún almacén para la entrada";
        }

        return resultado;
    }

    $scope.Aceptar = function() {
        if ($scope.ValidarDatos()) {
            $scope.EnviarDatos();
        }
    }

    $scope.EnviarDatos = function () {
        var Model = {};
        $scope.Fkarticulos = $("[name='Fkarticulosentrada']").val();
        Model.Fkarticulos = $("[name='Fkarticulosentrada']").val();
        Model.Fkalmacen = $("[name='Fkalmacen']").val();
        Model.Fkzonas = $("[name='Fkzonas']").val();
        Model.Lote = $("[name='Lote']").val();
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
                    GridViewLineas.Refresh();
                    
                    $('#_entregastock').modal('hide');
                }



            }).error(function(data, status, headers, config,statusText ) {
                $scope.Errorgeneral = data.error;
            });
    }

}]);

