app.controller('PedidosComprasCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.bancoscliente = [];
    $scope.Fkproveedores = "";
    $scope.urlApiBancos = "";
    $scope.cliente = "";
    $scope.fkobras = "";
    $scope.urlimportarlineas = "";
    $scope.urlDescargaDefecto = "";
    $scope.init = function (firstEdit, cliente, agente, comercial, bancomandato, fkobras, isnuevo, urlrefrescar, urlApiBancos, urlImportarlineas, urlDescargaDefecto)
    {
        $scope.firstEditSeries = firstEdit && cliente !== "";
        $scope.firstEditClientes = firstEdit && cliente!=="";
        $scope.firstEditAgentes = firstEdit && agente !== "";
        $scope.firstEditComerciales = firstEdit && comercial !== "";
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.Fkproveedores = cliente;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlimportarlineas = urlImportarlineas;
        $scope.fkobras = fkobras;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $("#PresupuestosDesde").prop("readonly", true);
        $("#btnbuscarPresupuestosDesde").prop("disabled", true);

        $("#PresupuestosHasta").prop("readonly", true);
        $("#btnbuscarPresupuestosHasta").prop("disabled", true);
    }

    eventAggregator.RegisterEvent("PresupuestosDesde", function (msg) {
        if ($("#PresupuestosHasta").val()==="") {
            eventAggregator.Publish("PresupuestosHasta-Buscar", msg);
        }
    });
   

    $("[name='Porcentajedescuentoprontopago']").bind('change', function (event, previousText) {
        if (!$scope.firstEditComerciales) {
            $scope.refrescharGrids();
        }
    });

    $("[name='Porcentajedescuentocomercial']").bind('change', function (event, previousText) {
        if (!$scope.firstEditComerciales) {
            $scope.refrescharGrids();
        }
    });
    eventAggregator.RegisterEvent("Fkseries-cv", function (message) {

        var fkmonedaactual = $("#Fkmonedas").val();
        if (!fkmonedaactual) {
            eventAggregator.Publish("Fkmonedas-Buscar", message.Fkmonedas);
        }
        
        });
    eventAggregator.RegisterEvent("Fkproveedores", function (message) {
        $scope.Fkproveedores = message;
        if (message == null || message === "") {
            $scope.cliente = "";
        }
        eventAggregator.Publish("PresupuestosDesde-Buscar", "");
        eventAggregator.Publish("PresupuestosHasta-Buscar", "");

        $("#PresupuestosDesde").prop("readonly", message == null || message === "");
        $("#btnbuscarPresupuestosDesde").prop("disabled", message == null || message === "");

        $("#PresupuestosHasta").prop("readonly", message == null || message === "");
        $("#btnbuscarPresupuestosHasta").prop("disabled", message == null || message === "");
        
    });
    eventAggregator.RegisterEvent("Fkproveedores-cv", function (message) {
        $scope.cliente = message.Fkcuentas + "-" + message.Descripcion;
            if (!$scope.firstEditClientes) {
                var fkmonedaactual = $("#Fkmonedas").val();
                if (fkmonedaactual && fkmonedaactual!=="" &&  message.Fkmonedas != fkmonedaactual) {
                    eventAggregator.Publish("Fkproveedores-Buscar", "");
                    bootbox.alert("La moneda de la seria y la del cliente no coinciden");
                    return;
                }
                if (!fkmonedaactual) {
                    
                    eventAggregator.Publish("Fkmonedas-Buscar", message.Fkmonedas);

                    if ($scope.isnuevo) {
                        $("#btnbuscarFkmonedas").removeAttr('disabled');
                        $("#Fkmonedas").removeAttr('readonly');
                    }
                }
                $scope.Fkproveedores = message.Fkcuentas;
                $("[name='Nombrecliente']").val(message.Descripcion);
                $("[name='Clientedireccion']").val(message.Direccion);
                $("[name='Clientepoblacion']").val(message.Poblacion);
                $("[name='Clientecp']").val(message.Cp);
                $("[name='Clientepais']").val(message.Pais);
                $("[name='Clienteprovincia']").val(message.Provincia);
                $("[name='Clientetelefono']").val(message.Telefono);
                $("[name='Clientefax']").val(message.Fax);
                $("[name='Clienteemail']").val(message.Email);
                $("[name='Clientenif']").val(message.Nif);
                $("[name='Porcentajedescuentocomercial']").val(message.Descuentocomercial);
                $("[name='Porcentajedescuentoprontopago']").val(message.Descuentoprontopago);
                $("[name='Fktransportista']").val(message.Fktransportistahabitual);
                $("[name='Fkregimeniva']").val(message.Fkregimeniva);
                $("[name='Nombrecliente']").val(message.Descripcion);
                $("[name='Incoterm']").val(message.Fkincoterm);
                $("[name='Unidadnegocio']").val(message.Fkunidadnegocio);
                $("#FkpuertosFkpaises").val(message.Fkpuertos.Fkpaises);
                $("#puertoscontrolid").val(message.Fkpuertos.Id);
                $("#idhidden").val(message.Fkpuertos.Id);
                
                eventAggregator.Publish("Fkcuentastesoreria-Buscar", message.Cuentatesoreria ? message.Cuentatesoreria : "");
                eventAggregator.Publish("Fkformaspago-Buscar", message.Fkformaspago ? message.Fkformaspago : "");
                eventAggregator.Publish("Fktransportista-Buscar", message.Fktransportistahabitual ? message.Fktransportistahabitual : "");
                eventAggregator.Publish("Fkagentes-Buscar", message.Fkcuentasagente ? message.Fkcuentasagente: "");
                eventAggregator.Publish("Fkcomerciales-Buscar", message.Fkcuentascomercial ? message.Fkcuentascomercial : "");
               
            }
            if ($scope.firstEditClientes)
                eventAggregator.Publish("Fkobras-Buscar", $scope.fkobras ? $scope.fkobras : "");
            $scope.calcularBancoscliente();
            $scope.firstEditClientes = false;
        });

        eventAggregator.RegisterEvent("Fkagentes-cv", function (message) {
            if (!$scope.firstEditAgentes && message) {
                $("[name='Comisionagente']").val(message.Porcentajecomision);
            }
            $scope.firstEditAgentes = false;
        });

        eventAggregator.RegisterEvent("Fkcomerciales-cv", function (message) {
            if (!$scope.firstEditComerciales && message) {
                $("[name='Comisioncomercial']").val(message.Porcentajecomision);
            }
            $scope.firstEditComerciales = false;
        });

        eventAggregator.RegisterEvent("Fkmonedas-cv", function (message) {
            if (!$scope.firstEditClientes && message) {
                $("[name='Tipocambio']").val(message.CambioMonedaBase);
            }
        });

        eventAggregator.RegisterEvent("Fkregimeniva-cv", function (message) {
            if (!$scope.firstEditComerciales ) {
                $scope.refrescharGrids();
            }
        });
        eventAggregator.RegisterEvent("Precio-Buscarcancelar", function (message) {
            SPrecio.Focus();
        });
        eventAggregator.RegisterEvent("Fkarticulos-Buscarcancelar", function (message) {
            Fkarticulos.Focus();
        });
        $scope.calcularBancoscliente = function () {
            if (!$scope.Fkproveedores) {
                $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
                return;
            }
            $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkproveedores)
                .success(function (response) {
                    if (response.values.length > 0) {
                        $scope.bancoscliente = response.values;
                        $scope.bancoscliente.splice(0, 0, { Id: "", Descripcion: "", Defecto:false });
                        if (!$scope.fkbancosmandatos) {
                            var result = $.grep($scope.bancoscliente, function (e) { return e.Defecto == true; });
                            if (result.length)
                                $scope.fkbancosmandatos = result[0].Id;
                        }
                    } else {
                        $scope.fkbancosmandatos = "";
                        $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
                    }
                    
                }).error(function (data, status, headers, config) {
                    //alert(status);
                });
        }

        $scope.refrescharGrids=function() {
            $.get($scope.urlreferescarlineas, { porcentajedescuentopp: $("[name='Porcentajedescuentoprontopago']").val(), porcentajedescuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

                GridViewLineas.Refresh();
                GridViewTotales.Refresh();

            }).error(function (jqXHR, textStatus, errorThrown) {
                

            });


        }


    //region start importar lineas

    $scope.ImportarLineas = function(url) {
        var obj = {
            campoIdentificador: "Id",
            IdComponenteasociado: "Importarlineas",
            IdFormulariomodal: "BusquedaGlobal",
            Url:url,
            Titulo: "Buscar líneas de prespuestos",
            Params: "{ \"cliente\":\"" + $("[name='Fkproveedores']").val() + "\" ,\"PresupuestosDesde\":\"" + $("[name='PresupuestosDesde']").val() + "\", \"PresupuestosHasta\":\"" + $("[name='PresupuestosHasta']").val() + "\"}"
            
        };

        eventAggregator.Publish("_lanzarbusquedaimportarlineas", obj);
    };

    

    eventAggregator.RegisterEvent("Importarlineas-Buscar", function(obj) {

        if (GridViewLineas.GetVisibleRowsOnPage() === 0 && obj && obj.length>0)
            SetCabeceraValues(obj[0].Cabecera);

        $.post($scope.urlimportarlineas, {
            lineas: JSON.stringify(obj), descuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), descuentopp:$("[name='Porcentajedescuentoprontopago']").val()
    }).success(function (result) {

            GridViewLineas.Refresh();
            GridViewTotales.Refresh();
            messagesService.show(TipoMensaje.Informacion, "Bien", "Lineas importadas correctamente");
            $('.nav-tabs a[href="#desglose"]').tab('show');
            $('.nav-tabs a[href="#importar"]').hide();
            

        }).error(function (jqXHR, textStatus, errorThrown) {

            messagesService.show(TipoMensaje.Error, "Ups!", "Parece que ocurrión un error al importar las líneas");
        });
        

    });

    function SetCabeceraValues(cabecera) {

        $scope.firstEditAgentes = true;
        $scope.firstEditComerciales = true;
        $("[name='Nombrecliente']").val(cabecera.Nombrecliente);
        $("[name='Clientedireccion']").val(cabecera.Clientedireccion);
        $("[name='Clientepoblacion']").val(cabecera.Clientepoblacion);
        $("[name='Clientecp']").val(cabecera.Clientecp);
        $("[name='Clientepais']").val(cabecera.Clientepais);
        $("[name='Clienteprovincia']").val(cabecera.Clienteprovincia);
        $("[name='Clientetelefono']").val(cabecera.Clientetelefono);
        $("[name='Clientefax']").val(cabecera.Clientefax);
        $("[name='Clienteemail']").val(cabecera.Clienteemail);
        $("[name='Clientenif']").val(cabecera.Clientenif);
        $("[name='Fkalmacen']").val(cabecera.Fkalmacen);
        $("[name='Comisionagente']").val(cabecera.Comisionagente);
        $("[name='Comisioncomercial']").val(cabecera.Comisioncomercial);
        $("[name='Porcentajedescuentocomercial']").val(cabecera.Porcentajedescuentocomercial);
        $("[name='Porcentajedescuentoprontopago']").val(cabecera.Porcentajedescuentoprontopago);
        $("[name='Fktransportista']").val(cabecera.Fktransportista);
        $("[name='Fkregimeniva']").val(cabecera.Fkregimeniva);
        $("[name='Unidadnegocio']").val(cabecera.Unidadnegocio);
        $("[name='Tipocambio']").val(cabecera.Tipocambio);
        //otros
        $("[name='Comisionagente']").val(cabecera.Comisionagente);
        $("[name='Comisioncomercial']").val(cabecera.Comisioncomercial);
        $("[name='Peso']").val(cabecera.Peso);
        $("[name='Fkobras']").val(cabecera.Fkobras);
        $("[name='Incoterm']").val(cabecera.Incoterm);
        $("[name='Costemateriales']").val(cabecera.Costemateriales);
        $("[name='Tiempooficinatecnica']").val(cabecera.Tiempooficinatecnica);
        $("[name='Fkalmacen']").val(cabecera.Fkalmacen);
        $("[name='Referenciadocumento']").val(cabecera.Referenciadocumento);
        $("[name='Cartacredito']").val(cabecera.Cartacredito);
        $("[name='Vencimientocartacredito']").val(cabecera.Vencimientocartacreditocadena);
        $("[name='Contenedores']").val(cabecera.Contenedores);
        $("[name='Notas']").val(cabecera.Notas);
        $("#FkpuertosFkpaises").val(cabecera.Fkpuertos.Fkpaises);
        $("#puertoscontrolid").val(cabecera.Fkpuertos.Id);
        $("#idhidden").val(cabecera.Fkpuertos.Id);

        eventAggregator.Publish("Fkcuentastesoreria-Buscar", cabecera.Fkcuentastesoreria ? cabecera.Fkcuentastesoreria : "");
        eventAggregator.Publish("Fkformaspago-Buscar", cabecera.Fkformaspago ? cabecera.Fkformaspago : "");
        eventAggregator.Publish("Fktransportista-Buscar", cabecera.Fktransportista ? cabecera.Fktransportista : "");
        eventAggregator.Publish("Fkagentes-Buscar", cabecera.Fkagentes ? cabecera.Fkagentes : "");
        eventAggregator.Publish("Fkcomerciales-Buscar", cabecera.Fkcomerciales ? cabecera.Fkcomerciales : "");

    }
    //endregion end importar lineas

    eventAggregator.RegisterEvent("Enviarpedido", function (message) {
            var model = new EmailModel();
            var referenciaDocumento = $("[name='Referencia']").val();
            model.Tituloformulario = "PedidosCompras ";
            model.Fkcuenta = $("[name='Fkproveedores']").val();
            model.Destinatario = $("[name='Clienteemail']").val();
            model.Asunto = String.format("Pedido: {0}", referenciaDocumento);
            model.Contenido = "";
            var ficheroDefecto = new FicherosEmailModel();
            ficheroDefecto.Nombre = String.format("{0}.pdf",referenciaDocumento);
            ficheroDefecto.Tipo = TipoFicherosEmail.Url;
            ficheroDefecto.Url = $scope.urlDescargaDefecto;
            model.Ficheros = [];
            model.Ficheros.push(ficheroDefecto);
            model.PertmiteCc = true;
            model.PertmiteBcc = true;
            eventAggregator.Publish("Mostraremail", model);
        });

}]);

var articuloActual = null;
var cargardDireccion=function(urldireccion,id,entidad) {
    $.get(urldireccion + "/" + id, { fkentidad: entidad }).success(function (message) {
       
        $("[name='Clientedireccion']").val(message.Direccion);
        $("[name='Clientepoblacion']").val(message.Poblacion);
        $("[name='Clientecp']").val(message.Cp);
        $("[name='Clientepais']").val(message.Pais);
        $("[name='Clienteprovincia']").val(message.Provincia);
        $("[name='Clientetelefono']").val((message.Telefono ? message.Telefono + ' - ' : '') + (message.Telefonomovil ? message.Telefonomovil : ''));
        $("[name='Clientefax']").val(message.Fax);
        $("[name='Clienteemail']").val(message.Email);
    }).error(function (jqXHR, textStatus, errorThrown) {
        
    });

}
var pedirArticulo=function(codigoarticulo,urlarticulos,lineas,setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkproveedores']").val(), fkmonedas: $("[name='Fkmonedas']").val(), fkregimeniva: $("[name='Fkregimeniva']").val(), flujo: '1' }).success(function (result) {

        articuloActual = result;

        var escomentario = result.Articulocomentario;
        var cCantidad = lineas.GetEditor("Cantidad");
        var cPorcentajedescuento = lineas.GetEditor("Porcentajedescuento");
        var cLote = lineas.GetEditor("Lote");
        var cLargo = lineas.GetEditor("SLargo");
        var cAncho = lineas.GetEditor("SAncho");
        var cGrueso = lineas.GetEditor("SGrueso");
        var cPrecio = lineas.GetEditor("SPrecio");
        var cIva = lineas.GetEditor("Porcentajeiva");
        var cMetros = lineas.GetEditor("SMetros");
        var cFktiposiva = lineas.GetEditor("Fktiposiva");
        lineas.SetEnabled(true);
        cCantidad.SetEnabled(!escomentario);
        cPorcentajedescuento.SetEnabled(!escomentario);
        cLote.SetEnabled(!escomentario);
        cLargo.SetEnabled(result.Permitemodificarlargo && !escomentario);
        cAncho.SetEnabled(result.Permitemodificarancho && !escomentario);
        cGrueso.SetEnabled(result.Permitemodificargrueso && !escomentario);
        cMetros.SetEnabled(result.Permitemodificarmetros && !escomentario);
        cPrecio.SetEnabled(!escomentario);
        cFktiposiva.SetEnabled(result.Tipoivavariable);
        if (setvaloresdefecto)
            establecerValoresDefecto(lineas);

        GridViewLineas.FocusEditor("Descripcion");

    }).error(function (jqXHR, textStatus, errorThrown) {
        lineas.SetEnabled(true);
        Fkarticulos.SetValue("");
        Fkarticulos.Focus();

    });
}

var establecerValoresDefecto = function (lineas) {
    var cDescripcion = lineas.GetEditor("Descripcion");
    var cCantidad = lineas.GetEditor("Cantidad");
    var cLargo = lineas.GetEditor("SLargo");
    var cAncho = lineas.GetEditor("SAncho");
    var cGrueso = lineas.GetEditor("SGrueso");
    var cPrecio = lineas.GetEditor("SPrecio");
    var cIva = lineas.GetEditor("Porcentajeiva");
    var cMetros = lineas.GetEditor("SMetros");
    var cFktiposiva = lineas.GetEditor("Fktiposiva");
    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo, articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho, articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso, articuloActual.Decimalestotales));
    cPrecio.SetValue(articuloActual.Precio);
    cIva.SetValue(articuloActual.Porcentajeiva);
    cDescripcion.SetValue(articuloActual.Descripcion);
    cFktiposiva.SetValue(articuloActual.Fktiposiva);
    if (articuloActual.Articulocomentario) {
        cCantidad.SetValue(0);
    }
    cMetros.SetValue(FFormulasService.CreateFormula(articuloActual.Formulas).calculate(cCantidad.GetValue(), cLargo.GetValue(), cAncho.GetValue(), cGrueso.GetValue(), cMetros.GetValue(), articuloActual.Decimalestotales));

    calculoImporte(lineas);
    cDescripcion.Focus();
}

var calculoImporte = function(lineas)
{
    var cDescuento = lineas.GetEditor("Porcentajedescuento");
    var cMetros = lineas.GetEditor("SMetros");
    var cPrecio = lineas.GetEditor("SPrecio");
    var cImporte = lineas.GetEditor("SImporte");

 
    var porcentajedescuento = cDescuento.GetValue();

    var bruto = cMetros.GetValue() * cPrecio.GetValue();
    var neto = bruto - (bruto * porcentajedescuento / 100.0);

    var importe = Globalize.format(Funciones.Redondear(neto, articuloActual.Decimalesmonedas));

    cImporte.SetValue(importe);
}



