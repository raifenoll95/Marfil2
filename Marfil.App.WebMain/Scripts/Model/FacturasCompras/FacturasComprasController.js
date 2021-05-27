app.controller('FacturasComprasCtrl', ['$scope', '$http', '$location', '$window', '$timeout', function ($scope, $http, $location, $window, $timeout) {

    $scope.firstEditClientes = false;
    $scope.firstEditAgentes = false;
    $scope.firstEditComerciales = false;
    $scope.firstEditDireccionfacturacion = false;
    $scope.firstEditTiposretenciones = false;
    $scope.isnuevo = false;
    $scope.urlreferescarlineas = "";
    $scope.fkbancosmandatos = "";
    $scope.bancoscliente = [];
    $scope.Fkproveedores = "";
    $scope.urlApiBancos = "";
    $scope.cliente = "";
    $scope.urlimportarlineas = "";
    $scope.fkobras = "";
    $scope.UrlreferescarVencimientos = "";
    $scope.urlDescargaDefecto = "";
    $scope.fktiposretenciones = "";

    $scope.init = function (firstEdit, cliente, direccionfacturacion, bancomandato,fkobras,fktiposretenciones, isnuevo, urlrefrescar, urlApiBancos,urlImportarlineas,urlreferescarvencimientos,urlDescargaDefecto)
    {
        $scope.firstEditTiposretenciones = firstEdit && fktiposretenciones !== "";

        $scope.firstEditSeries = firstEdit && cliente !== "";
        $scope.firstEditClientes = firstEdit && cliente!=="";
        $scope.firstEditDireccionfacturacion = firstEdit && direccionfacturacion !== "";
        $scope.firstEditAgentes = firstEdit;
        $scope.firstEditComerciales = firstEdit;
        $scope.isnuevo = isnuevo;
        $scope.urlreferescarlineas = urlrefrescar;
        $scope.fkbancosmandatos = bancomandato;
        $scope.Fkproveedores = cliente;
        $scope.urlApiBancos = urlApiBancos;
        $scope.urlimportarlineas = urlImportarlineas;
        $scope.fkobras = fkobras;
        $scope.fktiposretenciones = fktiposretenciones;
        $scope.urlDescargaDefecto = urlDescargaDefecto;
        $scope.UrlreferescarVencimientos = urlreferescarvencimientos;
        $("#AlbaranesDesde").prop("readonly", true);
        $("#btnbuscarAlbaranesDesde").prop("disabled", true);

        $("#AlbaranesHasta").prop("readonly", true);
        $("#btnbuscarAlbaranesHasta").prop("disabled", true);

    }

    eventAggregator.RegisterEvent("AlbaranesDesde", function (msg) {
        
        if ($("#AlbaranesHasta").val() === "") {
            eventAggregator.Publish("AlbaranesHasta-Buscar", msg);
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
        eventAggregator.Publish("AlbaranesDesde-Buscar", "");
        eventAggregator.Publish("AlbaranesHasta-Buscar", "");

        $("#AlbaranesDesde").prop("readonly", message == null || message === "");
        $("#btnbuscarAlbaranesDesde").prop("disabled", message == null || message === "");

        $("#AlbaranesHasta").prop("readonly", message == null || message === "");
        $("#btnbuscarAlbaranesHasta").prop("disabled", message == null || message === "");
        
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
                eventAggregator.Publish("Fkdireccionfacturacion-Buscar", message.DireccionId ? message.DireccionId : "");

                eventAggregator.Publish("Fktiposretenciones-Buscar", message.Fktiposretenciones ? message.Fktiposretenciones : "");

            }

            if ($scope.fkobras !== "") {
               
                eventAggregator.Publish("Fkobras-Buscar", $scope.fkobras ? $scope.fkobras : "");
                $scope.fkobras = "";
            }
                
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

        eventAggregator.RegisterEvent("Fktiposretenciones-cv", function (message) {
            if (!$scope.firstEditTiposretenciones) {
                $("[name='Porcentajeretencion']").val(message.Porcentajeretencion);
            }
            $scope.firstEditTiposretenciones = false;
        });

        eventAggregator.RegisterEvent("Fkmonedas-cv", function (message) {
            if (!$scope.firstEditClientes) {
                $("[name='Tipocambio']").val(message.CambioMonedaBase);
            }
        });

        eventAggregator.RegisterEvent("Fkformaspago-cv", function (message) {
            if (!$scope.firstEditClientes && message!=null) {
                //Recalcular vencimientos
                $http.get($scope.UrlreferescarVencimientos +"?id=" + message.Id);
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
            if (!$scope.Fkproveedores) {
                $scope.bancoscliente = [{ Id: "", Descripcion: "" }];
                return;
            }
            $http.get($scope.urlApiBancos + "?fkcuenta=" + $scope.Fkproveedores)
                .success(function (response) {
                    if (response.values.length > 0) {
                        $scope.bancoscliente = response.values;
                        for (var i = 0; i < $scope.bancoscliente.length; i++) {
                            $scope.bancoscliente[i].Descripcion = String.format("{0},{1},{2},{3}", $scope.bancoscliente[i].Descripcion, $scope.bancoscliente[i].Idmandato, $scope.bancoscliente[i].Tipoadeudocadena, $scope.bancoscliente[i].Esquemacadena);
                        }
                        $scope.bancoscliente.splice(0, 0, { Id: "", Descripcion: "", Defecto:false });
                        if (!$scope.fkbancosmandatos && $scope.fkbancosmandatos!="") {
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
            Titulo: "Buscar albaranes",
            Params: "{ \"cliente\":\"" + $("[name='Fkproveedores']").val() + "\" ,\"fkalbaraninicio\":\"" + $("[name='fkalbaraninicio']").val() + "\", \"fkalbaranfin\":\"" + $("[name='fkalbaranfin']").val() + "\"}"
            
        };

        eventAggregator.Publish("_lanzarbusquedaimportarlineas", obj);
    };

    

    eventAggregator.RegisterEvent("Importarlineas-Buscar", function(obj) {
        $.post($scope.urlimportarlineas, {
            lineas: JSON.stringify(obj), decimalesmonedas: $("[name='Decimalesmonedas']").val(), descuentocomercial: $("[name='Porcentajedescuentocomercial']").val(), descuentopp: $("[name='Porcentajedescuentoprontopago']").val()
        }).success(function (result) {
            //$("[name='Importefacturaproveedor']").val(parseFloat($("[name='Importefacturaproveedor']").val()) + parseFloat(result));
            $("[name='Importefacturaproveedor']").val(result)
            GridViewLineas.Refresh();
            GridViewTotales.Refresh();
            messagesService.show(TipoMensaje.Informacion, "Bien", "Lineas importadas correctamente");
            $('.nav-tabs a[href="#desglose"]').tab('show');
            $('.nav-tabs a[href="#importar"]').hide();
            

        }).error(function (jqXHR, textStatus, errorThrown) {
            messagesService.show(TipoMensaje.Error, "Ups!", "Parece que ocurrión un error al importar las líneas");
            
        });
        

    });

    //endregion end importar lineas
    eventAggregator.RegisterEvent("Enviarfactura", function (message) {
        var model = new EmailModel();
        var referenciaDocumento = $("[name='Referencia']").val();
        model.Fkcuenta = $("[name='Fkproveedores']").val();
        model.Tituloformulario = "Facturas ";
        model.Destinatario = $("[name='Clienteemail']").val();
        model.Asunto = String.format("Factura: {0}", referenciaDocumento);
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

var cargarDireccionFacturacion = function (message) {
    $("#direccionfacturaciondescripcion").html(message ? "<div class=\"well\">"+message.Direccion + "<br/>" + message.Poblacion + ", " + message.Provincia + "<br/>" + message.Cp + "<br/>" + message.Pais+ "</div>" : "");
   
}
var pedirArticulo=function(codigoarticulo,urlarticulos,lineas,setvaloresdefecto) {
    $.get(urlarticulos + "/" + codigoarticulo, { fkcuentas: $("[name='Fkproveedores']").val(), fkmonedas: $("[name='Fkmonedas']").val(),fkregimeniva: $("[name='Fkregimeniva']").val() }).success(function (result) {

        articuloActual = result;

        var cCantidad = lineas.GetEditor("Cantidad");
        var cLargo = lineas.GetEditor("SLargo");
        var cAncho = lineas.GetEditor("SAncho");
        var cGrueso = lineas.GetEditor("SGrueso");
        var cPrecio = lineas.GetEditor("SPrecio");
        var cIva = lineas.GetEditor("Porcentajeiva");
        var cMetros = lineas.GetEditor("SMetros");
        lineas.SetEnabled(true);
        
        cLargo.SetEnabled(result.Permitemodificarlargo);
        cAncho.SetEnabled(result.Permitemodificarancho);
        cGrueso.SetEnabled(result.Permitemodificargrueso);
        cMetros.SetEnabled(result.Permitemodificarmetros);

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

    cLargo.SetValue(articuloActual.Largo.toFixed(articuloActual.Decimalestotales));
    cAncho.SetValue(articuloActual.Ancho.toFixed(articuloActual.Decimalestotales));
    cGrueso.SetValue(articuloActual.Grueso.toFixed(articuloActual.Decimalestotales));
    cPrecio.SetValue(articuloActual.Precio);
    cIva.SetValue(articuloActual.Porcentajeiva);
    cDescripcion.SetValue(articuloActual.Descripcion);
    
    cMetros.SetValue(FFormulasService.CreateFormula(articuloActual.Formulas).calculate(cCantidad.GetValue(), cLargo.GetValue(), cAncho.GetValue(), cGrueso.GetValue(), cMetros.GetValue()));

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

    var importe = Globalize.format(neto.toFixed(articuloActual.Decimalesmonedas));

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

eventAggregator.RegisterEvent("Fktransportista-cv", function(message) {
    $("[name='Nombretransportista']").val(message.Descripcion);
}
);
