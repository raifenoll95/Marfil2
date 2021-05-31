app.controller('PresupuestosCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.fkobras = "";
    $scope.bancoscliente = [];
    $scope.fkclientes = "";
    $scope.urlApiBancos = "";
    $scope.urlDescargaDefecto = "";

    $scope.regimenIVA;

    $scope.init = function (regimenIVA, firstEdit, cliente, agente, comercial, bancomandato, fkobras, isnuevo, urlrefrescar, urlApiBancos, urlDescargaDefecto) {
        $scope.regimenIVA = regimenIVA;
        $scope.firstEditClientes = firstEdit && cliente !== "";
        $scope.firstEditAgentes = firstEdit && agente !== "";
        $scope.firstEditComerciales = firstEdit && comercial !== "";
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.fkobras = fkobras;
        $scope.fkclientes = cliente;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
    }

    $("[name='Porcentajedescuentoprontopago']").bind('change', function (event, previousText) {
        if (!$scope.firstEditComerciales) {
            var descuento = $("[name='Porcentajedescuentoprontopago']").val();
            if (descuento.includes(",")) {
                $("[name='Porcentajedescuentoprontopago']").val(descuento.replace(",", "."));
            }
            $scope.refrescharGrids();
        }
    });

    $("[name='Porcentajedescuentocomercial']").bind('change', function (event, previousText) {
        if (!$scope.firstEditComerciales) {
            var descuento = $("[name='Porcentajedescuentocomercial']").val();
            if (descuento.includes(",")) {
                $("[name='Porcentajedescuentocomercial']").val(descuento.replace(",", "."));
            }
            $scope.refrescharGrids();
        }
    });
    eventAggregator.RegisterEvent("Fkseries-cv", function (message) {
        eventAggregator.Publish("Fkmonedas-Buscar", message.Fkmonedas);
    });
    eventAggregator.RegisterEvent("Fkclientes", function (message) {
        if (message === "") $("#convertirprospectocliente").addClass("hide");
    });
    eventAggregator.RegisterEvent("Fkclientes-cv", function (message) {

        if (message && message.EsProspecto)
            $("#convertirprospectocliente").removeClass("hide");

        else $("#convertirprospectocliente").addClass("hide");


        if (!$scope.firstEditClientes) {
            var fkmonedaactual = $("#Fkmonedas").val();
            if (fkmonedaactual && fkmonedaactual !== "" && message.Fkmonedas != fkmonedaactual) {
                eventAggregator.Publish("Fkclientes-Buscar", "");
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
            $scope.fkclientes = message.Fkcuentas;
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
            eventAggregator.Publish("Fkagentes-Buscar", message.Fkcuentasagente ? message.Fkcuentasagente : "");
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
        if (message.Id != $scope.regimenIVA) {
            $scope.regimenIVA = message.Id;
            if (!$scope.firstEditComerciales) {
                $scope.refrescharGrids();
            }
        }
    });

    $scope.calcularBancoscliente = function () {
        if (!$scope.fkclientes) {
            $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
            return;
        }
        $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.fkclientes)
            .success(function (response) {
                if (response.values.length > 0) {
                    $scope.bancoscliente = response.values;
                    $scope.bancoscliente.splice(0, 0, { Id: "", Descripcion: "", Defecto: false });
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

    $scope.refrescharGrids = function () {
        $.get($scope.urlreferescarlineas, { porcentajedescuentopp: $("[name='Porcentajedescuentoprontopago']").val(), porcentajedescuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

            GridViewLineas.Refresh();
            GridViewTotales.Refresh();

        }).error(function (jqXHR, textStatus, errorThrown) {


        });
    }

    eventAggregator.RegisterEvent("Precio-Buscarcancelar", function (message) {
        SPrecio.Focus();
    });
    eventAggregator.RegisterEvent("Fkarticulos-Buscarcancelar", function (message) {
        Fkarticulos.Focus();
    });

    eventAggregator.RegisterEvent("Enviarpresupuesto", function (message) {
        var model = new EmailModel();
        var referenciaDocumento = $("[name='Referencia']").val();
        model.Fkcuenta = $("[name='Fkclientes']").val();
        model.Tituloformulario = "Presupuestos ";
        model.Destinatario = $("[name='Clienteemail']").val();
        model.Asunto = String.format("Presupuesto: {0}", referenciaDocumento);
        model.Contenido = "";
        var ficheroDefecto = new FicherosEmailModel();
        ficheroDefecto.Nombre = String.format("{0}.pdf", referenciaDocumento);
        ficheroDefecto.Tipo = TipoFicherosEmail.Url;
        ficheroDefecto.Url = $scope.urlDescargaDefecto;
        model.Ficheros = [];
        model.Ficheros.push(ficheroDefecto);
        model.PertmiteCc = true;
        model.PertmiteBcc = true;
        eventAggregator.Publish("Mostraremail", model);
    });

    $scope.ConvertirACliente = function () {
        var model = new ConvertirProspectoClienteModel();
        model.ProspectoId = $("[name='Fkclientes']").val();
        model.ProspectoDescripcion = $("#cv-Fkclientes-descripcion").html();
        eventAggregator.Publish("Mostrarconvertirprospectocliente", model);
    }



}]);

var articuloActual = null;
var cargardDireccion = function (urldireccion, id, entidad) {
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

var MostrarCalculadora = function () {
    if (articuloActual.Formulas === 0) {
        var model = {};
        var cLargo = GridViewLineas.GetEditor("SLargo");
        var cAncho = GridViewLineas.GetEditor("SAncho");
        var cGrueso = GridViewLineas.GetEditor("SGrueso");
        var cMetros = GridViewLineas.GetEditor("SMetros");
        model.FormulaSuperficie = articuloActual.Formulas;
        model.Cajas = articuloActual.Piezascaja;
        model.ControlCantidad = GridViewLineas.GetEditor("Cantidad");
        model.Largo = cLargo.GetValue();
        model.Ancho = cAncho.GetValue();
        model.Grueso = cGrueso.GetValue();
        model.Metros = cMetros.GetValue();
        model.Decimales = articuloActual.Decimalestotales;
        eventAggregator.Publish("calculadora", model);
    }
}

var pedirArticulo = function (codigoarticulo, urlarticulos, lineas, setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkclientes']").val(), fkmonedas: $("[name='Fkmonedas']").val(), fkregimeniva: $("[name='Fkregimeniva']").val(), flujo: '0' }).success(function (result) {

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
        var cFktiposiva = lineas.GetEditor("Fktiposiva");
        var cIva = lineas.GetEditor("Porcentajeiva");
        var cMetros = lineas.GetEditor("SMetros");
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
    var cPorcentajedescuento = lineas.GetEditor("Porcentajedescuento");
    var cIva = lineas.GetEditor("Porcentajeiva");
    var cFktipoiva = lineas.GetEditor("Fktiposiva");
    var cMetros = lineas.GetEditor("SMetros");
    if (articuloActual.Articulocomentario) {
        cCantidad.SetValue(0);
    }

    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo, articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho, articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso, articuloActual.Decimalestotales));
    cPrecio.SetValue(articuloActual.Precio);
    cPorcentajedescuento.SetValue(articuloActual.Descuento)
    cIva.SetValue(articuloActual.Porcentajeiva);
    cFktipoiva.SetValue(articuloActual.Fktiposiva);
    cDescripcion.SetValue(articuloActual.Descripcion);

    cMetros.SetValue(FFormulasService.CreateFormula(articuloActual.Formulas).calculate(cCantidad.GetValue(), cLargo.GetValue(), cAncho.GetValue(), cGrueso.GetValue(), cMetros.GetValue(), articuloActual.Decimalestotales));

    calculoImporte(lineas);
    cDescripcion.Focus();
}

var calculoImporte = function (lineas) {
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



