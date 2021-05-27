
app.controller('RecepcionStockCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.Urlapi = "";
    $scope.Urlapilotes = "";
    $scope.Urlultimoprecio = "";
    $scope.Editarmetros = false;
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
    $scope.Lotetransformacion = "";
    $scope.Materialsalida = "";
    //errores
    $scope.Fkarticuloserrores = "";
    $scope.Cantidaderrores = "";
    $scope.Largoerrores = "";
    $scope.Anchoerrores = "";
    $scope.Gruesoerrores = "";
    $scope.Precioerrores = "";
    $scope.Generalerrores = "";
    $scope.Carga = false;
    $scope.init = function(url,urllotes,urlultimoprecio,tipodocumento) {
        $scope.Urlapi = url;
        $scope.Urlapilotes = urllotes;
        $scope.Urlultimoprecio = urlultimoprecio;
        $scope.Tipodocumento = tipodocumento;
        $scope.Lotesautomaticos = [{ IdLote: "", Texto: "<Lote automático>" }];
        eventAggregator.RegisterEvent("Precioarticulo-Buscar", function (valor) {
            $scope.Precio = valor;
        });
    }

    $scope.ClearFormulario = function () {
        $scope.Bundle = "";
        $scope.Fkarticuloserrores = "";
        $scope.Cantidaderrores = "";
        $scope.Largoerrores = "";
        $scope.Anchoerrores = "";
        $scope.Gruesoerrores = "";
        $scope.Precioerrores = "";
        $scope.Generalerrores = "";
        if ($scope.Tipofamilia == 2) {
            $scope.GetLotesAutomaticos($scope.Fkarticulos);
        }
    }

    $('#_entradastock').on('show.bs.modal', function () {
        $scope.Carga = true;
        $scope.ClearFormulario();
        
    });
    $('#_entradastock').on('hidden.bs.modal', function () {
        $scope.GridVista().Focus();
    });
    $scope.GridVista = function () {
        return typeof GridViewLineas === 'undefined' || GridViewLineas === null ? GridViewLineasEntrada : GridViewLineas;
    }
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
            $scope.Editarmetros = ms.Permitemodificarmetros;
            $scope.Grueso = Funciones.RedondearGlobalize(ms.Grueso, $scope.Decimalesmedidas);
            $scope.Precio = Funciones.RedondearGlobalize(ms.Precio, $scope.Decimalesmonedas);
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
            

            if (ms.Tipofamilia == 1) {
                $scope.Cantidad = 1;
            }

            if (ms.Tipofamilia == 2 && $scope.Lotetransformacion) {
                $scope.Loteautomatico = false;
            }
            if (ms.Articulocomentario) {
                $scope.Cantidad = "0";
                $scope.Precio = "0";
                $scope.Descuentoarticulo = "0";
            }
            $scope.RecalculaMetrosSubtotal();
            if ($scope.Tipofamilia == 2) {
                $scope.GetLotesAutomaticos(ms.Id);
            } else {
                $scope.Loteautomaticoid = "";
                $scope.Lotesautomaticos = [];
                $scope.Lotesautomaticos.splice(0, 0, { IdLote: "", Texto: "<Lote automático>" });
            }

          

            
        }
        
    });

    $scope.GetLotesAutomaticos = function (fkarticulo) {
        if (fkarticulo && fkarticulo != "") {
            var req = {
                method: 'POST',
                url: $scope.Urlapilotes,
                data: JSON.stringify({ fkarticulo: fkarticulo })
            }
            $http(req)
                    .success(function (response) {

                        $scope.Lotesautomaticos = response;

                        $scope.Lotesautomaticos.splice(0, 0, { IdLote: "", Texto: "<Lote automático>" });
                        //$scope.Loteautomaticoid = "";
                    }).error(function (data, status, headers, config) {
                        $scope.Lotesautomaticos = [];
                        $scope.Lotesautomaticos.splice(0, 0, { IdLote: "", Texto: "<Lote automático>" });
                        //$scope.Loteautomaticoid = "";
                    });
        }
        
    }

    eventAggregator.RegisterEvent("setloteBloqueSalida",function(msg) {
        $scope.Lotetransformacion = msg;
    });

    eventAggregator.RegisterEvent("setMaterialSalida", function (msg) {
        $("#Materialsalida").val(msg);
        
        
    });

    $scope.$watch("Loteautomatico", function (value, old) {
        $scope.Lote = "";
        if ($scope.Lotetransformacion && $scope.Tipofamilia == "2")
            $scope.Lote = $scope.Lotetransformacion;

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

    $scope.SearchPrecios = function(){
       
        if ($scope.Fkarticulos && $scope.Fkarticulos!="") {
            var obj = {
                campoIdentificador: "Precio",
                IdComponenteasociado: "Precioarticulo",
                IdFormulariomodal: "ultimoprecio",
                Url: $scope.Urlultimoprecio,
                Titulo: "Últimos precios",
                Params: "{\"fkcuenta\":\"" + $("[name='Fkproveedores']").val() + "\",\"articulo\":\"" + $scope.Fkarticulos + "\",\"tipodocumento\":\"" + $scope.Tipodocumento + "\",\"cliente\":\"" + $("#cv-Fkproveedores-descripcion").html() + "\",\"descripcion\":\"" + $scope.Descripcion + "\"}"
            };

            eventAggregator.Publish("_lanzarbusquedaultimoprecio", obj);
        }
        
    }

    eventAggregator.RegisterEvent("Precioarticulo-Buscar",function(ms) {
        $scope.Precio = ms.toLocaleString();
        $scope.RecalculaMetrosSubtotal();
    });

    $scope.ValidarDatos = function () {

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
        if (($scope.Precio=="")) {
            resultado = false;
            $scope.Precioerrores = campoobligatorio;
        }

        if ($("[name='Fkalmacen']").val() == "") {
            resultado = false;
            $scope.Generalerrores = "No ha indicado ningún almacén para la entrada";
        }

        if ($scope.Tipogestionlotes == 2 && !$scope.Loteautomatico && (!$scope.Lote|| $scope.Lote=="")) {
            resultado = false;
            $scope.Generalerrores = "Si no tiene marcado un lote automatico, debe indicar uno.";
        }

        if ($scope.Tipofamilia != "2") {
            $scope.Bundle = "";
        }

        if ($scope.Bundle.length > 2) {
            resultado = false;
            $scope.Generalerrores = "La longitud máxima del Bundle es de dos.";
        }
        return resultado;
    }

    $scope.Aceptar = function() {
        if ($scope.ValidarDatos()) {
            $scope.EnviarDatos();
        }
    }

    $scope.AceptarComponente = function () {
        var a = 3;
    }

    $scope.EnviarDatos = function () {
        var Model = {};
        $scope.Fkarticulos = $("[name='Fkarticulosentrada']").val();
        Model.Fkarticulos = $("[name='Fkarticulosentrada']").val();
        Model.Fkalmacen = $("[name='Fkalmacen']").val();
        Model.Fkzonas = $("[name='Fkzonas']").val();
        Model.Loteautomatico = $scope.Loteautomatico;
        Model.Lote = $scope.Loteautomatico ? $scope.Loteautomaticoid: $scope.Lote;
        Model.Cantidad = $scope.Cantidad;
        Model.Largo = $scope.Largo;
        Model.Ancho = $scope.Ancho;
        Model.Grueso = $scope.Grueso;
        Model.Metros = $scope.Metros;
        Model.Precio = $scope.Precio;
        Model.Subtotal = $scope.Subtotal;
        Model.Decimalesmonedas = $scope.Decimalesmonedas;
        Model.Decimalesmedidas = $scope.Decimalesmedidas;
        Model.Fkunidades = $scope.Fkunidades;
        Model.Formulas = $scope.Formulas;
        Model.Fktiposiva = $scope.Fktiposiva;
        Model.Tipofamilia = $scope.Tipofamilia;
        Model.Descuento = $scope.Descuentoarticulo;
        Model.Descuentoprontopago = $("[name='Porcentajedescuentoprontopago']").val();
        Model.Descuentocomercial = $("[name='Porcentajedescuentocomercial']").val();
        Model.Portes = $("[name='Costeportes']").val();
        Model.Bundle = $scope.Bundle;
        var req = {
            method: 'POST',
            url: $scope.Urlapi,
            data: JSON.stringify(Model),
            contentType: 'application/json',
            dataType: 'json'
        }
        $http(req)
            .success(function (response) {
                if (response.error) {
                    $scope.Generalerrores = response.error;
                } else {
                    $scope.GridVista().Refresh();
                    if(typeof GridViewTotales !=='undefined' && GridViewTotales!==null)
                        GridViewTotales.Refresh();
                    $('#_entradastock').modal('hide');
                }



            }).error(function(data, status, headers, config,statusText ) {
                
            });
    }

}]);

