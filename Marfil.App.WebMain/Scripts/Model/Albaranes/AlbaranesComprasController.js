app.controller('AlbaranesCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditProveedor = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.firstEditDireccionfacturacion = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.bancosproveedor = [];
    $scope.Fkproveedores = "";
    $scope.urlApiBancos = "";
    $scope.proveedor = "";
    $scope.urlimportarlineas = "";
    $scope.fkobras = "";
    $scope.tipodeportes = "";
    $scope.urlDescargaDefecto = "";

    $scope.urlApiEmpresas = "";
    $scope.empresa = "";
   
    $scope.urlApiCuentasTerceros = "";

    $scope.init = function (firstEdit, proveedor, agente, comercial, direccionfacturacion, bancomandato, fkobras, isnuevo, tipodeportes, urlrefrescar, urlApiBancos, urlImportarlineas, urlDescargaDefecto, urlApiEmpresas, empresa, urlApiCuentasTerceros)
    {
        $scope.firstEditSeries = firstEdit && proveedor !== "";
        $scope.firstEditProveedor = firstEdit && proveedor !== "";
        $scope.firstEditAgentes = firstEdit && agente !== "";
        $scope.firstEditComerciales = firstEdit && comercial !== "";
        $scope.firstEditDireccionfacturacion = firstEdit && direccionfacturacion!=="";
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.Fkproveedores = proveedor;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlimportarlineas = urlImportarlineas;
        $scope.fkobras = fkobras;
        $scope.tipodeportes = tipodeportes;
        $scope.urlDescargaDefecto = urlDescargaDefecto;

        $scope.urlApiEmpresas = urlApiEmpresas;
        $scope.empresa = empresa;

        $scope.urlApiCuentasTerceros = urlApiCuentasTerceros;

        $("#PedidosDesde").prop("readonly", true);
        $("#btnbuscarPedidosDesde").prop("disabled", true);

        $("#PedidosHasta").prop("readonly", true);
        $("#btnbuscarPedidosHasta").prop("disabled", true);
   
    }

  

    $scope.Nuevalinea = function () {

        if ($("[name='Fkproveedores']").val() != '') {
            $('#_entradastock').on('shown.bs.modal', function () {
                $("#Fkarticulosentrada").focus();
            });

            $("#_entradastock").modal('show');
        }

        else {
            bootbox.alert("Seleccione primero un proveedor");
        }     
    }

    $scope.cambioTipoportes = function(valor) {
        if(!(valor && valor==0)){$("[name='Costeportes']").val("");}
    }

    eventAggregator.RegisterEvent("PedidosDesde",function(msg) {
        
        if ($("#PedidosHasta").val()==="") {
            eventAggregator.Publish("PedidosHasta-Buscar",msg);
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

        if (message.Entradasvarias) {
            console.log($scope.urlApiEmpresas)
            $http.get($scope.urlApiEmpresas + "?id=" + $scope.empresa)
                .success(function (response) {
                    var json = response;
                    var cuenta = json.FkCuentaEntradasVariasAlmacen;
                    if (cuenta) {
                            eventAggregator.Publish("Fkproveedores-Buscar", cuenta);
                    }
                }).error(function (data, status, headers, config) {
                    //  alert(status);
                });
        }
    });

    eventAggregator.RegisterEvent("Fkproveedores", function (message) {
        $scope.Fkproveedores = message;
        if (message == null || message === "") {
            $scope.proveedor = "";
        }
        eventAggregator.Publish("PedidosDesde-Buscar", "");
        eventAggregator.Publish("PedidosHasta-Buscar", "");

        $("#PedidosDesde").prop("readonly", message == null || message === "");
        $("#btnbuscarPedidosDesde").prop("disabled", message == null || message === "");

        $("#PedidosHasta").prop("readonly", message == null || message === "");
        $("#btnbuscarPedidosHasta").prop("disabled", message == null || message === "");
        
    });
    eventAggregator.RegisterEvent("Fkproveedores-cv", function (message) {
        $scope.proveedor = message.Fkcuentas + "-" + message.Descripcion;
            if (!$scope.firstEditProveedor) {
                var fkmonedaactual = $("#Fkmonedas").val();
                if (fkmonedaactual && fkmonedaactual!=="" &&  message.Fkmonedas != fkmonedaactual) {
                    eventAggregator.Publish("Fkproveedores-Buscar", "");
                    bootbox.alert("La moneda de la seria y la del proveedor no coinciden");
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
                $("[name='Nombreproveedor']").val(message.Descripcion);
                $("[name='Proveedordireccion']").val(message.Direccion);
                $("[name='Proveedorpoblacion']").val(message.Poblacion);
                $("[name='Proveedorcp']").val(message.Cp);
                $("[name='Proveedorpais']").val(message.Pais);
                $("[name='Proveedorprovincia']").val(message.Provincia);
                $("[name='Proveedortelefono']").val(message.Telefono);
                $("[name='Proveedorfax']").val(message.Fax);
                $("[name='Proveedoremail']").val(message.Email);
                $("[name='Proveedornif']").val(message.Nif);
                $("[name='Porcentajedescuentocomercial']").val(message.Descuentocomercial);
                $("[name='Porcentajedescuentoprontopago']").val(message.Descuentoprontopago);
                $("[name='Fktransportista']").val(message.Fktransportistahabitual);
                $("[name='Fkregimeniva']").val(message.Fkregimeniva);
                $("[name='Nombreproveedor']").val(message.Descripcion);
                $("[name='Incoterm']").val(message.Fkincoterm);
                $("[name='Unidadnegocio']").val(message.Fkunidadnegocio);
                $scope.tipodeportes= message.Tipodeportes;
                $("#FkpuertosFkpaises").val(message.Fkpuertos.Fkpaises);
                $("#puertoscontrolid").val(message.Fkpuertos.Id);
                $("#idhidden").val(message.Fkpuertos.Id);
                if (message.Fkcriteriosagrupacion && message.Fkcriteriosagrupacion !== "")
                    $("[name='Fkcriteriosagrupacion']").val(message.Fkcriteriosagrupacion);

                eventAggregator.Publish("Fkcuentastesoreria-Buscar", message.Cuentatesoreria ? message.Cuentatesoreria : "");
                eventAggregator.Publish("Fkformaspago-Buscar", message.Fkformaspago ? message.Fkformaspago : "");
                eventAggregator.Publish("Fktransportista-Buscar", message.Fktransportistahabitual ? message.Fktransportistahabitual : "");
                eventAggregator.Publish("Fkagentes-Buscar", message.Fkcuentasagente ? message.Fkcuentasagente: "");
                eventAggregator.Publish("Fkcomerciales-Buscar", message.Fkcuentascomercial ? message.Fkcuentascomercial : "");
                eventAggregator.Publish("Fkdireccionfacturacion-Buscar", message.DireccionId ? message.DireccionId : "");
               
            }

            if ($scope.fkobras !== "") {
               
                eventAggregator.Publish("Fkobras-Buscar", $scope.fkobras ? $scope.fkobras : "");
                $scope.fkobras = "";
            }
                
            $scope.calcularBancosproveedor();
            $scope.firstEditProveedor = false;
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
            if (!$scope.firstEditProveedor) {
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
        $scope.calcularBancosproveedor = function () {
            if (!$scope.Fkproveedores) {
                $scope.bancosproveedor = [{ Id: "", Descripcion: "" }];
                return;
            }
            $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkproveedores)
                .success(function (response) {
                    if (response.values.length > 0) {
                        $scope.bancosproveedor = response.values;
                        $scope.bancosproveedor.splice(0, 0, { Id: "", Descripcion: "", Defecto:false });
                        if (!$scope.fkbancosmandatos) {
                            var result = $.grep($scope.bancosproveedor, function (e) { return e.Defecto == true; });
                            if (result.length)
                                $scope.fkbancosmandatos = result[0].Id;
                        }
                    } else {
                        $scope.fkbancosmandatos = "";
                        $scope.bancosproveedor = [{ Id: "", Descripcion: "" }];
                    }
                    
                }).error(function (data, status, headers, config) {
                    //alert(status);
                });
        }

        $scope.refrescharGrids=function() {
            $.get($scope.urlreferescarlineas, { porcentajedescuentopp: $("[name='Porcentajedescuentoprontopago']").val(), porcentajedescuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

                GridViewLineas.Refresh();
                GridViewTotales.Refresh();
                GridViewCosteAdicional.Refresh();
            }).error(function (jqXHR, textStatus, errorThrown) {
                

            });
        }

        $scope.Guardarcostes= function() {
            $("#forzarcostesadicionales").submit();
        }

    //region start importar lineas

    $scope.ImportarLineas = function(url) {
        var obj = {
            campoIdentificador: "Id",
            IdComponenteasociado: "Importarlineas",
            IdFormulariomodal: "BusquedaGlobal",
            Url:url,
            Titulo: "Buscar líneas de pedidos",
            Params: "{ \"proveedor\":\"" + $("[name='Fkproveedores']").val() + "\" ,\"PedidosDesde\":\"" + $("[name='PedidosComprasDesde']").val() + "\", \"PedidosHasta\":\"" + $("[name='PedidosComprasHasta']").val() + "\"}"
            
        };

        eventAggregator.Publish("_lanzarbusquedaimportarlineas", obj);
    };

    $scope.Nuevalineacoste = function () {
        GridViewCosteAdicional.AddNewRow();
    }
    eventAggregator.RegisterEvent("nuevalineacoste", function (msg) {
        GridViewCosteAdicional.AddNewRow();
    });

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
        $scope.firstEditDireccionfacturacion = true;
        $("[name='NombreCliente']").val(cabecera.Nombrecliente);
        $("[name='Clientedireccion']").val(cabecera.Clientedireccion);
        $("[name='Clientepoblacion']").val(cabecera.Clientepoblacion);
        $("[name='Clientecp']").val(cabecera.Clientecp);
        $("[name='Clientepais']").val(cabecera.Clientepais);
        $("[name='Clienteprovincia']").val(cabecera.Clienteprovincia);
        $("[name='Clientetelefono']").val(cabecera.Clientetelefono);
        $("[name='Clientefax']").val(cabecera.Clientefax);
        $("[name='Clienteemail']").val(cabecera.Clienteemail);
        $("[name='Clientenif']").val(cabecera.Clientenif);
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
        model.Fkcuenta = $("[name='Fkproveedores']").val();
        var referenciaDocumento = $("[name='Referencia']").val();
        model.Tituloformulario = "Albaranes ";
        model.Destinatario = $("[name='Proveedoremail']").val();
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
var cargardDireccion=function(urldireccion,id,entidad) {
    $.get(urldireccion + "/" + id, { fkentidad: entidad }).success(function (message) {
       
        $("[name='Proveedordireccion']").val(message.Direccion);
        $("[name='Proveedorpoblacion']").val(message.Poblacion);
        $("[name='Proveedorcp']").val(message.Cp);
        $("[name='Proveedorpais']").val(message.Pais);
        $("[name='Proveedorprovincia']").val(message.Provincia);
        $("[name='Proveedortelefono']").val((message.Telefono ? message.Telefono + ' - ' : '') + (message.Telefonomovil ? message.Telefonomovil : ''));
        $("[name='Proveedorfax']").val(message.Fax);
        $("[name='Proveedoremail']").val(message.Email);
    }).error(function (jqXHR, textStatus, errorThrown) {
        
    });

}

var cargarDireccionFacturacion = function (message) {
    $("#direccionfacturaciondescripcion").html(message ? "<div class=\"well\">"+message.Direccion + "<br/>" + message.Poblacion + ", " + message.Provincia + "<br/>" + message.Cp + "<br/>" + message.Pais+ "</div>" : "");
   
}
var pedirArticulo=function(codigoarticulo,urlarticulos,lineas,setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkproveedores']").val(), flujo: '1', fkmonedas: $("[name='Fkmonedas']").val(), fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

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
    cLargo.SetValue(Funciones.Redondear(articuloActual.Largo,articuloActual.Decimalestotales));
    cAncho.SetValue(Funciones.Redondear(articuloActual.Ancho,articuloActual.Decimalestotales));
    cGrueso.SetValue(Funciones.Redondear(articuloActual.Grueso,articuloActual.Decimalestotales));
    cPrecio.SetValue(articuloActual.Precio);
    cIva.SetValue(articuloActual.Porcentajeiva);
    cDescripcion.SetValue(articuloActual.Descripcion);
    cFktiposiva.SetValue(articuloActual.Fktiposiva);
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

    var importe = Globalize.format(Funciones.Redondear(neto, articuloActual.Decimalesmonedas ));

    cImporte.SetValue(importe);
}

var GenerarDevolucion = function(lineas, columnas, url,id) {
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

eventAggregator.RegisterEvent("Fktransportista-cv", function (message) {
    if(message)
        $("[name='Nombretransportista']").val(message.Descripcion);
    else
        $("[name='Nombretransportista']").val("");
});

function CallFacturarAlbaran() {
    $("#facturarform").submit();
}

function CallSaldarPedido() {
    eventAggregator.Publish("SaldarPedido", null);
}

