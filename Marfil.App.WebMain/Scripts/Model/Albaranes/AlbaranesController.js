app.controller('AlbaranesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.firstEditDireccionfacturacion = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.bancoscliente = [];
    $scope.Fkclientes = "";
    $scope.urlApiBancos = "";
    $scope.cliente = "";
    $scope.urlimportarlineas = "";
    $scope.fkobras = "";
    $scope.tipodeportes = "";
    $scope.urlDescargaDefecto = "";

    $scope.urlTiposAlbaranes = "";
    $scope.urlApiEmpresas = "";
    $scope.empresa = "";

    $scope.init = function (firstEdit, cliente, agente, comercial, direccionfacturacion, bancomandato, fkobras, isnuevo, tipodeportes, urlrefrescar, urlApiBancos, urlImportarlineas, urlDescargaDefecto, urlApiEmpresas, empresa, urlTiposAlbaranes) {
        $scope.firstEditSeries = firstEdit && cliente !== "";
        $scope.firstEditClientes = firstEdit && cliente !== "";
        $scope.firstEditAgentes = firstEdit && agente !== "";
        $scope.firstEditComerciales = firstEdit && comercial !== "";
        $scope.firstEditDireccionfacturacion = firstEdit && direccionfacturacion !== "";
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.Fkclientes = cliente;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlimportarlineas = urlImportarlineas;
        $scope.fkobras = fkobras;
        $scope.tipodeportes = tipodeportes;
        $scope.urlDescargaDefecto = urlDescargaDefecto;

        $scope.urlTiposAlbaranes = urlTiposAlbaranes;
        $scope.urlApiEmpresas = urlApiEmpresas;
        $scope.empresa = empresa;

        $("#PedidosDesde").prop("readonly", true);
        $("#btnbuscarPedidosDesde").prop("disabled", true);

        $("#PedidosHasta").prop("readonly", true);
        $("#btnbuscarPedidosHasta").prop("disabled", true);
    }

    $scope.cambioTipoportes = function (valor) {
        if (!(valor && valor == 0)) { $("[name='Costeportes']").val(""); }
    }

    eventAggregator.RegisterEvent("PedidosDesde", function (msg) {

        if ($("#PedidosHasta").val() === "") {
            eventAggregator.Publish("PedidosHasta-Buscar", msg);
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

        $("[name='Tipodealmacenlote']").val(message.Tipodealmacenlote);

        var fkmonedaactual = $("#Fkmonedas").val();
        if (!fkmonedaactual) {
            eventAggregator.Publish("Fkmonedas-Buscar", message.Fkmonedas);
        }

        $http.get($scope.urlTiposAlbaranes + "/" + message.Id).success(function (response) {

            var tipoAlbaran = document.getElementById('Tipoalbaran').value
            var select = document.getElementById('Tipoalbaranenum')

            select.removeAttribute("disabled", "disabled")
            document.getElementById('Fkseries').removeAttribute("readonly", true)
            document.getElementById('btnbuscarFkseries').removeAttribute("disabled", "disabled")
            document.getElementById('Fkclientes').removeAttribute("readonly", true)
            document.getElementById('btnbuscarFkclientes').removeAttribute("disabled", "disabled")

            //Clear Dropdown
            while (select.firstChild) {
                select.removeChild(select.firstChild);
            }

            for (var i = 0; i < response.length; i++) {
                var option = document.createElement("option")
                option.value = response[i].EnumInterno
                option.text = response[i].Descripcion
                select.appendChild(option)
            }

            var childNodes = select.childNodes

            for (var i = 0; i < childNodes.length; i++) {
                if (childNodes[i].value == tipoAlbaran) {

                    childNodes[i].setAttribute("selected", "selected")
                    select.setAttribute("disabled", "disabled")

                    if (tipoAlbaran != 0) {
                        document.getElementById('Fkseries').setAttribute("readonly", true)
                        document.getElementById('btnbuscarFkseries').setAttribute("disabled", "disabled")
                        document.getElementById('Fkclientes').setAttribute("readonly", true)
                        document.getElementById('btnbuscarFkclientes').setAttribute("disabled", "disabled")
                    }
                    break;
                }
            }

        });


        if (message.Salidasvarias) {
            //console.log($scope.urlApiEmpresas)
            $http.get($scope.urlApiEmpresas + "?id=" + $scope.empresa)
                .success(function (response) {
                    var json = response;
                    var cuenta = json.FkCuentaSalidasVariasAlmacen;
                    if (cuenta) {
                        eventAggregator.Publish("Fkclientes-Buscar", cuenta);
                    }
                }).error(function (data, status, headers, config) {
                    //  alert(status);
                });
        }
    });


    eventAggregator.RegisterEvent("Fkclientes", function (message) {
        $scope.Fkclientes = message;
        if (message == null || message === "") {
            $scope.cliente = "";
        }
        eventAggregator.Publish("PedidosDesde-Buscar", "");
        eventAggregator.Publish("PedidosHasta-Buscar", "");

        $("#PedidosDesde").prop("readonly", message == null || message === "");
        $("#btnbuscarPedidosDesde").prop("disabled", message == null || message === "");

        $("#PedidosHasta").prop("readonly", message == null || message === "");
        $("#btnbuscarPedidosHasta").prop("disabled", message == null || message === "");

    });
    eventAggregator.RegisterEvent("Fkclientes-cv", function (message) {
        $scope.cliente = message.Fkcuentas + "-" + message.Descripcion;
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
            $scope.Fkclientes = message.Fkcuentas;
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
            $scope.tipodeportes = message.Tipodeportes;
            $("#FkpuertosFkpaises").val(message.Fkpuertos.Fkpaises);
            $("#puertoscontrolid").val(message.Fkpuertos.Id);
            $("#idhidden").val(message.Fkpuertos.Id);
            if (message.Fkcriteriosagrupacion && message.Fkcriteriosagrupacion !== "")
                $("[name='Fkcriteriosagrupacion']").val(message.Fkcriteriosagrupacion);

            eventAggregator.Publish("Fkcuentastesoreria-Buscar", message.Cuentatesoreria ? message.Cuentatesoreria : "");
            eventAggregator.Publish("Fkformaspago-Buscar", message.Fkformaspago ? message.Fkformaspago : "");
            eventAggregator.Publish("Fktransportista-Buscar", message.Fktransportistahabitual ? message.Fktransportistahabitual : "");
            eventAggregator.Publish("Fkagentes-Buscar", message.Fkcuentasagente ? message.Fkcuentasagente : "");
            eventAggregator.Publish("Fkcomerciales-Buscar", message.Fkcuentascomercial ? message.Fkcuentascomercial : "");
            eventAggregator.Publish("Fkdireccionfacturacion-Buscar", message.DireccionId ? message.DireccionId : "");

        }

        if ($scope.fkobras !== "") {

            eventAggregator.Publish("Fkobras-Buscar", $scope.fkobras ? $scope.fkobras : "");
            $scope.fkobras = "";
        }

        $scope.calcularBancoscliente();
        $scope.firstEditClientes = false;
    });

    eventAggregator.RegisterEvent("_nuevalinea", function (msg) {
        $scope.Nuevalinea();
    });

    $scope.Nuevalinea = function () {
        if ($("[name='Fkclientes']").val() && $("[name='Fkclientes']").val() !== "") {
            $('#_entregastock').on('shown.bs.modal', function () {
                $("#Fkarticulosentrada").focus();
            });

            $("#_entregastock").modal('show');
        } else {
            bootbox.alert("El campo cliente es obligatorio");
        }

    }
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
    eventAggregator.RegisterEvent("Precio-Buscarcancelar", function (message) {
        SPrecio.Focus();
    });
    eventAggregator.RegisterEvent("Fkarticulos-Buscarcancelar", function (message) {
        Fkarticulos.Focus();
    });
    $scope.calcularBancoscliente = function () {
        if (!$scope.Fkclientes) {
            $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
            return;
        }
        $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkclientes)
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






    //region start importar lineas

    $scope.ImportarLineas = function (url) {
        var obj = {
            campoIdentificador: "Id",
            IdComponenteasociado: "Importarlineas",
            IdFormulariomodal: "BusquedaGlobal",
            Url: url,
            Titulo: "Buscar líneas de pedidos",
            Params: "{ \"cliente\":\"" + $("[name='Fkclientes']").val() + "\" ,\"PedidosDesde\":\"" + $("[name='PedidosDesde']").val() + "\", \"PedidosHasta\":\"" + $("[name='PedidosHasta']").val() + "\"}"

        };

        eventAggregator.Publish("_lanzarbusquedaimportarlineas", obj);
    };



    eventAggregator.RegisterEvent("Importarlineas-Buscar", function (obj) {

        if (GridViewLineas.GetVisibleRowsOnPage() === 0 && obj && obj.length > 0)
            SetCabeceraValues(obj[0].Cabecera);

        $.post($scope.urlimportarlineas, {
            lineas: JSON.stringify(obj), descuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), descuentopp: $("[name='Porcentajedescuentoprontopago']").val()
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
        $scope.firstEditDireccionfacturacion = true;
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
        eventAggregator.Publish("Fkdireccionfacturacion-Buscar", cabecera.DireccionId ? cabecera.DireccionId : "");

    }

    //endregion end importar lineas

    eventAggregator.RegisterEvent("Enviaralbaran", function (message) {
        var model = new EmailModel();
        var referenciaDocumento = $("[name='Referencia']").val();
        model.Tituloformulario = "Albaranes ";
        model.Fkcuenta = $("[name='Fkclientes']").val();
        model.Destinatario = $("[name='Clienteemail']").val();
        model.Asunto = String.format("Albarán: {0}", referenciaDocumento);
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
var cargarDireccionFacturacion = function (message) {
    $("#direccionfacturaciondescripcion").html(message ? "<div class=\"well\">" + message.Direccion + "<br/>" + message.Poblacion + ", " + message.Provincia + "<br/>" + message.Cp + "<br/>" + message.Pais + "</div>" : "");

}
var pedirArticulo = function (codigoarticulo, urlarticulos, lineas, setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkclientes']").val(), flujo: '0', fkmonedas: $("[name='Fkmonedas']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

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
        var cFktiposiva = lineas.GetEditor("Fktiposiva");
        var cCaja = lineas.GetEditor("Caja");
        var cBundle = lineas.GetEditor("Bundle");
        var cMetros = lineas.GetEditor("SMetros");
        lineas.SetEnabled(true);
        cCantidad.SetEnabled(!escomentario);
        cPorcentajedescuento.SetEnabled(!escomentario);
        cLote.SetEnabled(!escomentario);
        cCaja.SetEnabled(!escomentario);
        cBundle.SetEnabled(!escomentario);
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
    var cMetros = lineas.GetEditor("SMetros");
    var cFktiposiva = lineas.GetEditor("Fktiposiva");
    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo, articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho, articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso, articuloActual.Decimalestotales));
    cFktiposiva.SetValue(articuloActual.Fktiposiva);
    cPrecio.SetValue(articuloActual.Precio);
    cPorcentajedescuento.SetValue(articuloActual.Descuento);
    cIva.SetValue(articuloActual.Porcentajeiva);
    cDescripcion.SetValue(articuloActual.Descripcion);
    if (articuloActual.Articulocomentario) {
        cCantidad.SetValue(0);
    }
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

var GenerarDevolucion = function (lineas, columnas, url, id) {
    var obj = {
        Values: lineas,
        Columns: columnas,
        Url: url,
        Idalbaran: id
    };

    eventAggregator.Publish("_lanzarbusquedadevolveralbaran", obj);
}

var GenerarReclamacion = function (lineas, columnas, url, id) {
    var obj = {
        Values: lineas,
        Columns: columnas,
        Url: url,
        Idalbaran: id
    };

    eventAggregator.Publish("_lanzarbusquedareclamaralbaran", obj);
}

var GenerarContenedores = function (lineas, columnas, url, id) {
    var obj = {
        Values: lineas,
        Columns: columnas,
        Url: url,
        Idalbaran: id
    };

    eventAggregator.Publish("_lanzarbusquedarasignarcontendores", obj);
}

eventAggregator.RegisterEvent("Fktransportista-cv", function (message) {
    $("[name='Nombretransportista']").val(message.Descripcion);
});

function CallFacturarAlbaran() {
    console.log("facturar");
    $("#facturarform").submit();
}

function CallSaldarPedido() {
    eventAggregator.Publish("SaldarPedido", null);
}



