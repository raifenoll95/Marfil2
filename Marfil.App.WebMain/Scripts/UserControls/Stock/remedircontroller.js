app.controller('RemedirCtrl', ['$scope', '$rootScope', '$http', '$interval', 'uiGridConstants', '$timeout', function ($scope, $rootScope, $http, $interval, $uiGridConstants, $timeout) {

    eventAggregator.RegisterEvent("_buscarlotes", function (data) {
        $scope.params = data.Params;
        $scope.load(data.campoIdentificador, data.IdComponenteasociado, data.IdFormulariomodal, data.Url, data.Titulo);
    });
    eventAggregator.RegisterEvent("_remedirlotes", function (data) {
        $scope.remedir(data.returnUrl);
    });


    $scope.Nuevolargo = "";
    $scope.Nuevoancho = "";
    $scope.Nuevogrueso = "";
    $scope.Permiteeditarlargo = true;
    $scope.Permiteeditarancho = true;
    $scope.Permiteeditargrueso = true;
    $scope.Largooriginal = 0;
    $scope.Anchooriginal = 0;
    $scope.Gruesooriginal = 0;
    $scope.Sumarlargo = true;
    $scope.Sumarancho = true;
    $scope.Sumargrueso = true;
    $scope.Fkarticulos = "";
    //configuracion Control
    $scope.campoIdentificador = "";
    $scope.IdComponenteasociado = "";
    $scope.IdFormulariomodal = "";
    $scope.Url = "";
    $scope.Urlarticulos = "";
    $scope.Titulo = "";
    $scope.gridOptionsremedir = { enableRowSelection: true, enableRowHeaderSelection: false };
    $scope.data = [];
    $scope.selected = null;
    $scope.params = "";
    $scope.gridOptionsremedir = { enableRowSelection: true, enableRowHeaderSelection: true, enableCellEdit: true, enableSelectAll: false };
    $scope.gridOptionsremedir.multiSelect = true;
    $scope.gridOptionsremedir.modifierKeysToMultiSelect = true;
    $scope.gridOptionsremedir.noUnselect = false;
    $scope.gridOptionsremedir.selectionRowHeaderWidth = 35;
    $scope.gridOptionsremedir.enableFiltering = true;
    $scope.cantidadlotes = 0;

    $scope.gridOptionsremedir.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selected = row;
            $scope.cantidadlotes = $scope.gridApi.selection.getSelectedCount();

            $scope.gridApi.grid.getColumn("Fkarticulos").filters[0] = { term: row.entity.Fkarticulos };
        });

        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.cantidadlotes = $scope.gridApi.selection.getSelectedCount();
        });

        $scope.gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
            $scope.gridApi.selection.selectRow(newRowCol.row.entity);
        });
    };

    $scope.gridOptionsremedir.rowTemplate = '<div role=\"gridcell\" ng-dblclick="grid.appScope.onDblClick(row)" ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ui-grid-cell></div>';
    $scope.gridOptionsremedir.appScopeProvider = {
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

    eventAggregator.RegisterEvent("cargarpaso3", function (message) {
        $scope.CargarPaso3(message);
    });

    $scope.CargarPaso3 = function (data) {
        var model = {};
        var rows = $scope.gridApi.selection.getSelectedRows();
        model.id = rows[0]["Fkarticulos"];
        $scope.Fkarticulos = model.id;
        $scope.Largooriginal = rows[0]["SLargo"];
        $scope.Anchooriginal = rows[0]["SAncho"];
        $scope.Gruesooriginal = rows[0]["SGrueso"];
        $http({
            url: data.Url,
            method: "GET",
            params: model
        })
            .success(function (response) {
                $scope.Permiteeditarlargo = response.Editarlargo;
                $scope.Permiteeditarancho = response.Editarancho;
                $scope.Permiteeditargrueso = response.Editargrueso;
                $scope.Pesopiezaarticulo = response.Kilosud;
            }).error(function (data, status, headers, config) {
                alert("Error");
            });
    }

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
        $scope.gridApi.cellNav.scrollToFocus(row, $scope.gridOptionsremedir.columnDefs[colIndex]);
    };

    $scope.loading = true;
    $scope.load = function (campoIdentificador, IdComponenteasociado, IdFormulariomodal, Url, Titulo) {
        $scope.loading = true;
        $scope.cantidadlotes = 0;
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

               $scope.gridOptionsremedir.columnDefs = response.columns;
               $scope.gridOptionsremedir.data = response.values;
               $scope.gridApi.grid.enableHorizontalScrollbar = 2;
               $scope.gridOptionsremedir.minWidth = 150;
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
               $scope.gridOptionsremedir.data = [];
               $scope.loading = false;
           });
        }
        else {
            $http.get($scope.Url)
           .success(function (response) {
               $scope.selected = null;

               $scope.gridApi.grid.clearAllFilters();
               $scope.loading = false;
               $scope.gridOptionsremedir.columnDefs = response.columns;
               $scope.gridOptionsremedir.data = response.values;
               $scope.gridApi.grid.enableHorizontalScrollbar = 2;
               $scope.gridOptionsremedir.minWidth = 150;
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
               $scope.gridOptionsremedir.data = [];
               $scope.loading = false;
           });
        }


    }

    //api facturar

    $scope.remedir = function (urlretorno) {
        var rows = $scope.gridApi.selection.getSelectedRows();

        var largo = $scope.Nuevolargo;
        if ($scope.Nuevolargo) {
            if ($scope.Sumarlargo) {
                if (Globalize.parseFloat(largo) + (Globalize.parseFloat($scope.Largooriginal)*100) < 0) {
                    $("#errornuevolargo").html("El Largo resultado total debe ser positivo");
                    return;
                }
            } else if ($scope.Nuevolargo < 0) {
                $("#errornuevolargo").html("El Largo resultado total debe ser positivo");
                return;
            }

        }


        var ancho = $scope.Nuevoancho;
        if ($scope.Nuevoancho) {
            if ($scope.Sumarancho) {
                if (Globalize.parseFloat(ancho) + (Globalize.parseFloat($scope.Anchooriginal)*100) < 0) {
                    $("#errornuevoancho").html("El Ancho resultado total debe ser positivo");
                    return;
                }
            }
            else if ($scope.Nuevoancho < 0) {
                $("#errornuevoancho").html("El Ancho resultado total debe ser positivo");
                return;
            }

        }

        var grueso = $scope.Nuevogrueso;
        if ($scope.Nuevogrueso) {
            if ($scope.Sumargrueso) {
                if (Globalize.parseFloat(grueso) + (Globalize.parseFloat($scope.Gruesooriginal)*100) < 0) {
                    $("#errornuevogrueso").html("El Grueso resultado total debe ser positivo");
                    return;
                }
            }
            else if ($scope.Nuevogrueso < 0) {
                if (grueso + $scope.Gruesooriginal < 0) {
                    $("#errornuevogrueso").html("El Grueso resultado total debe ser positivo");
                    return;
                }
            }

        }

        var lotesId = $.map(rows, function (v) {
            return String.format("{0}|{1}|{2}|{3}", v.Fkalmacenes, v.Fkarticulos, v.Lote, v.Loteid);
        }).join(';');

        $("#remedirform input[name='Lotesreferencia']").val(lotesId);
        $("#remedirform input[name='Nuevolargo']").val($scope.Nuevolargo);
        $("#remedirform input[name='Nuevoancho']").val($scope.Nuevoancho);
        $("#remedirform input[name='Nuevogrueso']").val($scope.Nuevogrueso);
        $("#remedirform input[name='Sumarlargo']").val($scope.Sumarlargo);
        $("#remedirform input[name='Sumarancho']").val($scope.Sumarancho);
        $("#remedirform input[name='Sumargrueso']").val($scope.Sumargrueso);
        $("#remedirform input[name='Loteproveedor']").val($("#step3-form [name='Loteproveedor']").val());
        $("#remedirform input[name='Fkincidenciasmaterial']").val($("#Fkincidenciasmaterial").val());
        $("#remedirform input[name='Zona']").val($("#step3-form [name='Zona']").val());
        $("#remedirform input[name='Fkcalificacioncomercial']").val($("#step3-form [name='Fkcalificacioncomercial']").val());
        $("#remedirform input[name='Fktipograno']").val($("#step3-form [name='Fktipograno']").val());
        $("#remedirform input[name='Fktonomaterial']").val($("#step3-form [name='Fktonomaterial']").val());
        $("#remedirform input[name='Fkvariedades']").val($("#step3-form [name='Fkvariedades']").val());
        $("#remedirform input[name='Pesopieza']").val($("#step3-form [name='Pesopieza']").val());
        $("#remedirform").submit();
    }

    //end api facturar
}]);


