app.controller('PresupuestosComprasCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.fkobras = "";
    $scope.bancoscliente = [];
    $scope.Fproveedores = "";
    $scope.urlApiBancos = "";
    $scope.urlDescargaDefecto = "";
    $scope.init = function (firstEdit, cliente, agente, comercial, bancomandato, fkobras, isnuevo, urlrefrescar, urlApiBancos, urlDescargaDefecto)
    {
        $scope.firstEditClientes = firstEdit && cliente!=="";
        $scope.firstEditAgentes = firstEdit && agente !== "";
        $scope.firstEditComerciales = firstEdit && comercial !== "";
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.fkobras = fkobras;
        $scope.Fkproveedores = cliente;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
    }

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
            eventAggregator.Publish("Fkmonedas-Buscar", message.Fkmonedas);
        });
        eventAggregator.RegisterEvent("Fkproveedores", function (message) {
            if (message==="") $("#convertirprospectocliente").addClass("hide");
        });
        eventAggregator.RegisterEvent("Fkproveedores-cv", function (message) {

            if (message && message.EsProspecto)
                $("#convertirprospectocliente").removeClass("hide");

            else $("#convertirprospectocliente").addClass("hide");


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
            if (!$scope.firstEditAgentes) {
                $("[name='Comisionagente']").val(message.Porcentajecomision);
            }
            $scope.firstEditAgentes = false;
        });

        eventAggregator.RegisterEvent("Fkcomerciales-cv", function (message) {
            if (!$scope.firstEditComerciales) {
                $("[name='Comisioncomercial']").val(message.Porcentajecomision);
            }
            $scope.firstEditComerciales = false;
        });

        eventAggregator.RegisterEvent("Fkmonedas-cv", function (message) {
            if (!$scope.firstEditClientes) {
                $("[name='Tipocambio']").val(message.CambioMonedaBase);
            }
        });

        eventAggregator.RegisterEvent("Fkregimeniva-cv", function (message) {
            if (!$scope.firstEditComerciales) {
                $scope.refrescharGrids();
            }
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

        eventAggregator.RegisterEvent("Precio-Buscarcancelar", function(message) {
            SPrecio.Focus();
        });
        eventAggregator.RegisterEvent("Fkarticulos-Buscarcancelar", function(message) {
            Fkarticulos.Focus();
        });

        eventAggregator.RegisterEvent("Enviarpresupuesto", function (message) {
            var model = new EmailModel();
            var referenciaDocumento = $("[name='Referencia']").val();
            model.Fkcuenta = $("[name='Fkproveedores']").val();
            model.Tituloformulario = "Presupuestos ";
            model.Destinatario = $("[name='Clienteemail']").val();
            model.Asunto = String.format("Presupuesto: {0}",referenciaDocumento);
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

        $scope.ConvertirACliente=function() {
            var model = new ConvertirProspectoClienteModel();
            model.ProspectoId = $("[name='Fkproveedores']").val();
            model.ProspectoDescripcion = $("#cv-fkproveedores-descripcion").html();
            eventAggregator.Publish("Mostrarconvertirprospectocliente", model);
        }

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
        alert("Error");
    });

}
var pedirArticulo=function(codigoarticulo,urlarticulos,lineas,setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkproveedores']").val(), fkmonedas: $("[name='Fkmonedas']").val(),fkregimeniva: $("[name='Fkregimeniva']").val(), flujo: '1' }).success(function (result) {

        articuloActual = result;

        var escomentario = result.Articulocomentario;
        var cCantidad = lineas.GetEditor("Cantidad");
        var cPorcentajedescuento = lineas.GetEditor("Porcentajedescuento");
        var cRevision = lineas.GetEditor("Revision");
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
        cRevision.SetEnabled(!escomentario);
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
    if (articuloActual.Articulocomentario) {
        cCantidad.SetValue(0);
    }
    cFktiposiva.SetValue(articuloActual.Fktiposiva);
    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo, articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho, articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso, articuloActual.Decimalestotales));
    cPrecio.SetValue(articuloActual.Precio);
    cIva.SetValue(articuloActual.Porcentajeiva);
    cDescripcion.SetValue(articuloActual.Descripcion);
    
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



