
app.controller('BancosMandatosCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.controlId = "";
    $scope.empresa = "";
    $scope.fkcuentas = "";
    $scope.bancosmandatos = [];
    $scope.cargosempresas = [];

    var counter = -1;
    //nuevo item
    $scope.errores =
    {
        
        Empresa: "",
        Fkcuentas: "",
        Id: "",
        Descripcion: "",
        Fkpaises: "",
        Iban: "",
        Bic: "",
        Sufijoacreedor: "",
        Contratoconfirmig: "",
        Contadorconfirming: "",
        Direccion: "",
        Cpostal: "",
        Ciudad: "",
        Fkprovincias: "",
        Telefonobanco: "",
        Personacontacto: "",
        Idmandato: "",
        Idacreedor: "",
        Tiposecuenciasepa: "",
        Tipoadeudo: "",
        Importemandato: "",
        Recibosmandato: "",
        Importelimiterecibo: "",
        Fechafirma: "",
        Fechaexpiracion: "",
        Fechaultimaremesa: "",
        Importeremesados: "",
        Recibosremesados: "",
        Devolvera: "",
        Notas: "",
        Defecto: "",
        Finalizado: "",
        Bloqueado: "",
        Esquema: "",
        Riesgonacional: "",
        Riesgoextranjero: ""
    }
    $scope.nuevo =
    {
        
        Descripcion: "",
        Fkpaises: "",
        Iban: "",
        Bic: "",
        Sufijoacreedor: "",
        Contratoconfirmig: "",
        Contadorconfirming: "",
        Direccion: "",
        Cpostal: "",
        Ciudad: "",
        Fkprovincias: "",
        Telefonobanco: "",
        Personacontacto: "",
        Idmandato: "",
        Idacreedor: "",
        Tiposecuenciasepa: 0,
        Tipoadeudo: 0,
        Importemandato: 0,
        Recibosmandato: 0,
        Importelimiterecibo: 0,
        Fechafirma: null,
        Fechaexpiración: null,
        Fechaultimaremesa: null,
        Importeremesados: 0,
        Recibosremesados: 0,
        Devolvera: "",
        Notas: "",
        Defecto: false,
        Finalizado: false,
        Bloqueado: false,
        Esquema: "",
        Riesgonacional: "",
        Riesgoextranjero: ""
    };
    $scope.clearError=function() {
        $scope.errores =
        {
            Empresa: "",
            Fkcuentas: "",
            Id: "",
            Descripcion: "",
            Fkpaises: "",
            Iban: "",
            Bic: "",
            Sufijoacreedor: "",
            Contratoconfirmig: "",
            Contadorconfirming: "",
            Direccion: "",
            Cpostal: "",
            Ciudad: "",
            Fkprovincias: "",
            Telefonobanco: "",
            Personacontacto: "",
            Idmandato: "",
            Idacreedor: "",
            Tiposecuenciasepa: "",
            Tipoadeudo: "",
            Importemandato: "",
            Recibosmandato: "",
            Importelimiterecibo: "",
            Fechafirma: "",
            Fechaexpiracion: "",
            Fechaultimaremesa: "",
            Importeremesados: "",
            Recibosremesados: "",
            Devolvera: "",
            Notas: "",
            Defecto: "",
            Finalizado: "",
            Bloqueado: "",
            Esquema: "",
            Riesgonacional: "",
            Riesgoextranjero: ""
        };
    }
    $scope.clear = function () {
        $scope.nuevo =
    {
       
        Descripcion: "",
        Fkpaises: "",
        Iban: "",
        Bic: "",
        Sufijoacreedor: "000",
        Contratoconfirmig: "",
        Contadorconfirming: "",
        Direccion: "",
        Cpostal: "",
        Ciudad: "",
        Fkprovincias: "",
        Telefonobanco: "",
        Personacontacto: "",
        Idmandato: "",
        Idacreedor: "",
        Tiposecuenciasepa: 0,
        Tipoadeudo: 0,
        Importemandato: 0,
        Recibosmandato: 0,
        Importelimiterecibo: 0,
        Fechafirma: null,
        Fechaexpiración: null,
        Fechaultimaremesa: null,
        Importeremesados: 0,
        Recibosremesados: 0,
        Devolvera: "",
        Notas: "",
        Defecto: false,
        Finalizado: false,
        Bloqueado: false,
        Esquema: "",
        Riesgonacional: "",
        Riesgoextranjero: ""
    };
        $scope.nuevo.Empresa = $scope.empresa;
        $scope.nuevo.Fkcuentas= $scope.fkcuentas;
        $scope.nuevo.Id = --counter;
        $scope.nuevo.Fkpaises = $scope.fkpaisdefecto;
        $scope.nuevo.Sufijoacreedor = "000";
        $scope.errores =
     {
         Empresa: "",
         Fkcuentas: "",
         Id: "",
         Descripcion: "",
         Fkpaises: "",
         Iban: "",
         Bic: "",
         Sufijoacreedor: "",
         Contratoconfirmig: "",
         Contadorconfirming: "",
         Direccion: "",
         Cpostal: "",
         Ciudad: "",
         Fkprovincias: "",
         Telefonobanco: "",
         Personacontacto: "",
         Idmandato: "",
         Idacreedor: "",
         Tiposecuenciasepa: "",
         Tipoadeudo: "",
         Importemandato: "",
         Recibosmandato: "",
         Importelimiterecibo: "",
         Fechafirma: "",
         Fechaexpiracion: "",
         Fechaultimaremesa: "",
         Importeremesados: "",
         Recibosremesados: "",
         Devolvera: "",
         Notas: "",
         Defecto: "",
         Finalizado: "",
         Bloqueado: "",
         Esquema: "",
         Riesgonacional: "",
         Riesgoextranjero:""

     }
    }

    $scope.paises = [];

    $scope.init = function (controlId, empresa, tipotercero, contactos,  urlprovincias,urlbic, fkpaisdefecto,paises,nifempresa) {
        $scope.controlId = controlId;
        $scope.empresa = empresa;
        $scope.tipotercero = tipotercero;
        $scope.bancosmandatos = contactos;
        $scope.urlprovincias = urlprovincias;
        $scope.fkpaisdefecto = fkpaisdefecto;
        $scope.urlbic = urlbic;
        $scope.paises = paises;
        $scope.nif = nifempresa;//forzado siempre a este valor
        counter = contactos ? (contactos.length + 1) * -1 : -1;
    }

    //IBAN
    $scope.findPais=function(codigo) {
        for (var i = 0; i < $scope.paises.length; i++) {
            if ($scope.paises[i].Valor === codigo)
                return $scope.paises[i];
        }
        return null;
    }

    $scope.RellenaCeros = function() {
        $scope.nuevo.Idmandato = Funciones.RellenaCeros($scope.nuevo.Idmandato, 35,1);
    }

    $scope.operaIban = function() {

        $scope.nuevo.Iban = $scope.nuevo.Iban.toUpperCase();
        var regex = /^[A-Z][A-Z].*$/;
        var regexescuenta = /^[0-9]{20}$/;
        if (!regex.test($scope.nuevo.Iban) && regexescuenta.test($scope.nuevo.Iban)) {//españa
            $scope.calculaDigitoControlIban();
        }
        if (($scope.nuevo.Bic === "" || !$scope.nuevo.Bic) && $scope.nuevo.Fkpaises === "070")
            $scope.calculaBic();
    }

    $scope.calculaBic = function () {

        var codigobanco = $scope.nuevo.Iban.substring(4, 8);
        $http({
            url: $scope.urlbic +"/"+ codigobanco,
            method: "GET"
        })
         .success(function (response) {
             $scope.nuevo.Bic = response.Bic;
         }).error(function (data, status, headers, config) {
                $scope.nuevo.Bic = "";
            });
    }

    $scope.calculaDigitoControlIban = function () {
        var iban = IBAN.formatear($scope.nuevo.Iban);
        $scope.nuevo.Iban = IBAN.calcular(iban, $scope.findPais($scope.nuevo.Fkpaises).CodigoIsoAlfa2);
    }

    //END IBAN

    $scope.nuevoRegistro = function () {
        $scope.clear();
        if ($scope.tipotercero == "Mandato") {
            var pais = $scope.findPais($scope.nuevo.Fkpaises);
            if (pais) {
                var paisIso = pais.CodigoIsoAlfa2;
                $scope.nuevo.Idacreedor = SEPA.checksumCreditorID(paisIso + $scope.nuevo.Sufijoacreedor + "00" + $scope.nif);
            }
            
        }
        
        eventAggregator.Publish("cuentatesoreria", $scope.nuevo);
        $("#" + $scope.controlId).modal();
    }
    $scope.setDefecto = function (item) {
        //establecemos un defecto
        if (item.Defecto) {
            for (var i = 0; i < $scope.bancosmandatos.length; i++) {
                if (!($scope.bancosmandatos[i].Empresa == item.Empresa &&
                    $scope.bancosmandatos[i].Fkcuentas == item.Fkcuentas &&
                    $scope.bancosmandatos[i].Id == item.Id)) {
                    return $scope.bancosmandatos[i].Defecto = false;
                }
            }
        } else {
            var existeDefecto = false;
            for (var i = 0; i < $scope.bancosmandatos.length; i++) {
                if ($scope.bancosmandatos[i].Defecto) {
                    existeDefecto = true;
                    break;
                }
            }

            if (!existeDefecto && $scope.bancosmandatos.length > 0)
                return $scope.bancosmandatos[0].Defecto = true;
            else if ($scope.bancosmandatos.length == 0) {
                item.Defecto = true;
            }
        }
    }

    $scope.validateItem = function (item) {
        $scope.clearError();
        $scope.setDefecto(item);

        //aqui pondremos las reglas necesarias
        if (!Funciones.IsRequired(item.Descripcion)) {
            $scope.errores.Descripcion = "El campo Descripción es obligatorio";
            return false;
        }

        //aqui pondremos las reglas necesarias
        if (!Funciones.IsRequired(item.Iban)) {
            $scope.errores.Iban = "El campo IBAN es obligatorio";
            return false;
        }

        if (!SEPA.validateIBAN(item.Iban)) {
            $scope.errores.Iban = "El formato del IBAN no es correcto";
            return false;
        }

        //aqui pondremos las reglas necesarias
        if (!Funciones.IsRequired(item.Bic)) {
            $scope.errores.Bic = "El campo BIC es obligatorio";
            return false;
        }
       
        if (!Funciones.IsRequired(item.Fkpaises)) {
            $scope.errores.Fkpaises = "El campo País es obligatorio";
            return false;
        }

        var bicregex = /^([a-zA-Z]){4}([a-zA-Z]){2}([0-9a-zA-Z]){2}([0-9a-zA-Z]{3})?$/;
        var ibanregex=/^[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}$/;

        if (!ibanregex.test(item.Iban)) {
            $scope.errores.Iban = "El formato del campo IBAN no es correcto";
            return false;
        }

        if (!bicregex.test(item.Bic)) {
            $scope.errores.Bic = "El formato del campo BIC no es correcto";
            return false;
        }

        if ($scope.tipotercero == "Mandato") {

            //aqui pondremos las reglas necesarias
            if (!Funciones.IsRequired(item.Idmandato)) {
                $scope.errores.Idmandato = "El campo Id. Mandato es obligatorio";
                return false;
            }
            if (item.Idmandato.length!==35) {
                $scope.errores.Idmandato = "La longitud del mandato tiene que ser de 35 caracteres";
                return false;
            }
            //verificar si es mandato
            if (item.Idacreedor !== "" && item.Idacreedor.length <=7 && !SEPA.validateCreditorID(item.Idacreedor)) {
                $scope.errores.Idacreedor = "El formato del acreedor no es válido";
                return false;
            }

            if (item.Importemandato !== "" && !Funciones.IsNumeric(item.Importemandato)) {
                $scope.errores.Importemandato = "El campo debe ser numérico";
                return false;
            }

            if (item.Recibosmandato !== "" && !Funciones.IsNumeric(item.Recibosmandato)) {
                $scope.errores.Recibosmandato = "El campo debe ser numérico";
                return false;
            }

            if (item.Importelimiterecibo !== "" && !Funciones.IsNumeric(item.Importelimiterecibo)) {
                $scope.errores.Importelimiterecibo = "El campo debe ser numérico";
                return false;
            }

            if (item.Importeremesados !== "" && !Funciones.IsNumeric(item.Importeremesados)) {
                $scope.errores.Importeremesados = "El campo debe ser numérico";
                return false;
            }

            if (item.Recibosremesados !== "" && !Funciones.IsNumeric(item.Recibosremesados)) {
                $scope.errores.Recibosremesados = "El campo debe ser numérico";
                return false;
            }
        }


        return true;
    }

    $scope.saveItem = function () {
        //todo validar
        if ($scope.validateItem($scope.nuevo)) {
            var item = $scope.searchItem($scope.nuevo.Empresa, $scope.nuevo.Fkcuentas, $scope.nuevo.Id);
            if (item == undefined)
                $scope.bancosmandatos.push($scope.nuevo);
            else {
                var index = $scope.bancosmandatos.indexOf(item);
                if (index >= 0)
                    $scope.bancosmandatos[index] = $scope.nuevo;
            }
            $("#" + $scope.controlId).modal('hide');
            $scope.clear();
        }

    }

    $scope.edit = function (empresa, fkcuentas, id) {
        $scope.clear();
        var item = $scope.searchItem(empresa, fkcuentas, id);
        $scope.nuevo = $.parseJSON(JSON.stringify(item));;
        $("#" + $scope.controlId).modal();
    }
    $scope.delete = function (empresa, fkcuentas, id) {

        bootbox.confirm(Messages.EliminarRegistro, function (result) {
            if (result) {
                var item = $scope.searchItem(empresa, fkcuentas, id);
                var index = $scope.bancosmandatos.indexOf(item);
                if (index > -1) {
                    $scope.$apply(function () {
                        var eraDefecto = item.Defecto;
                        $scope.bancosmandatos.splice(index, 1);
                        if (eraDefecto && $scope.bancosmandatos.length > 0) {
                            $scope.bancosmandatos[0].Defecto = true;
                        }

                    });
                }
            }
        });
    }

    $scope.searchItem = function (empresa, fkcuentas, id) {
        for (var i = 0; i < $scope.bancosmandatos.length; i++) {
            if ($scope.bancosmandatos[i].Empresa == empresa &&
                $scope.bancosmandatos[i].Fkcuentas == fkcuentas &&
                $scope.bancosmandatos[i].Id == id) {
                return $scope.bancosmandatos[i];
            }
        }
    };

    $scope.$watch('nuevo.Fkpaises', function () {
        $http({
            url: $scope.urlprovincias,
            method: "GET",
            params: { codigopais: $scope.nuevo.Fkpaises }
        })
          .success(function (response) {
              $scope.provincias = response.values;
          }).error(function (data, status, headers, config) {
              $scope.Fkprovincia = "";
              $scope.provincias = [];
          });
    });

    $scope.$watch('nuevo.Sufijoacreedor', function () {
        if ($scope.tipotercero === "Mandato") {
            var nif = $scope.nif;

            if (nif.length > 2) {
                var reg = /^[a-zA-Z]$/;
                if (reg.test(nif.charAt(0)) && reg.test(nif.charAt(1))) {
                    nif = nif.substring(2);
                }
            }
            var pais = $scope.findPais($scope.nuevo.Fkpaises);
            if (pais) {
                var paisIso = $scope.findPais($scope.nuevo.Fkpaises).CodigoIsoAlfa2;
                $scope.nuevo.Idacreedor = SEPA.checksumCreditorID(paisIso + "00" + $scope.nuevo.Sufijoacreedor + nif);
            }
            
        }
    });

   


    //IBAN
    var IBAN = (function () {
        var paisOmision = "es";

        //============================================
        // PUBLISHED

        /*
          El parámetro numero puede ser un CCC o un IBAN
          Si es un CCC retorna el IBAN correspondiente
          Si es un IBAN lo formatea
          Avisa si es un CCC incorrecto o un IBAN incorrecto
    
          Ejemplo1: IBAN.convertir("12345") --> "Error: No es IBAN ni CCC"
          Ejemplo2: IBAN.convertir("ES0012345678061234567890") --> "Error: IBAN incorrecto"
          Ejemplo3: IBAN.convertir("ES5212345678001234567890") --> "Error: CCC incorrecto"
          Ejemplo4: IBAN.convertir("ES6812345678061234567890") --> "ES68 1234 5678 0612 3456 7890"
          Ejemplo5: IBAN.convertir("1234-5678-06-1234567890") --> "ES68 1234 5678 0612 3456 7890"
        */
        function convertir(numero, pais) {
            numero = limpiar(numero);
            var iban = numero.substr(numero.length - 24, 24);
            var ccc = numero.substr(numero.length - 20, 20);
            if (!esIBAN(numero) && !esCCC(numero)) return "Error: No es IBAN ni CCC";
            else if (esIBAN(numero) && !validarIBAN(iban)) return "Error: IBAN incorrecto";
            else if (!validarCCC(ccc)) return "Error: CCC incorrecto";
            else if (esIBAN(numero)) return formatearIBAN(iban);
            else return formatearIBAN(calcularIBAN(ccc, pais));
        }

        // Ejemplo: IBAN.calcular("1234-5678-??-1234567890") --> "ES6812345678061234567890" (68 y 06)
        function calcular(numero, pais) {
            numero = limpiar(numero);
            if (esCCC(numero)) {
                var dc = numero.substr(8, 2);
                if (!sonDigitos(dc)) numero = calcularCCC(numero);
                return calcularIBAN(numero, pais);
            }
            else return numero;
        }

        // Ejemplo1: IBAN.validar("ES68 1234 5678 0612 3456 7890") --> true (68)
        // Ejemplo2: IBAN.validar("1234-5678-06-1234567890") --> true (06)
        function validar(numero) {
            numero = limpiar(numero);
            if (esIBAN(numero)) return validarIBAN(numero);
            else if (esCCC(numero)) return validarCCC(numero);
            else return false;
        }

        // Ejemplo: IBAN.formatear("12345678061234567890") --> "1234-5678-06-1234567890"
        // Ejemplo: IBAN.formatear("ES6812345678061234567890") --> "ES68 1234 5678 0612 3456 7890"
        function formatear(numero, separador) {
            numero = limpiar(numero);
            if (esIBAN(numero)) return formatearIBAN(numero, separador);
            else if (esCCC(numero)) return formatearCCC(numero, separador);
            else return "";
        }

        //============================================
        // HIGH LEVEL

        /*
          Cómo se calcula los dígitos de control del IBAN
          a) Se añade al final de la BBAN, el código del país
             según la norma ISO 3166-1 y dos ceros.
          b) Si en el BBAN hay letras, convierte estas letras en números del 10 al 35,
             siguiendo el orden del abecedario; A=10 y Z=35.
          c) Divide el número por 97, y quédate con el resto.
          d) Réstale a 98 el resto que te quede
          e) Ya tenemos los dígitos de control, si la diferencia es menor a 10,
             añade un 0 a la izquierda.
        */

        // Ejemplo: calcularIBAN("1234-5678-06-1234567890", "es") --> "ES6812345678061234567890"
        function calcularIBAN(ccc, pais) {
            ccc = limpiar(ccc);
            pais = (pais == undefined ? paisOmision : pais).toUpperCase();
            var cifras = ccc + valorCifras(pais) + "00";
            var resto = modulo(cifras, 97);
            return pais + cerosIzquierda(98 - resto, 2) + ccc;
        }

        // Ejemplo1: validarIBAN("ES00 1234 5678 0612 3456 7890") --> false
        // Ejemplo2: validarIBAN("ES68 1234 5678 0612 3456 7890") --> true
        function validarIBAN(iban) {
            iban = limpiar(iban);
            var pais = iban.substr(0, 2);
            var dc = iban.substr(2, 2);
            var cifras = iban.substr(4, 20) + valorCifras(pais) + dc;
            resto = modulo(cifras, 97);
            return resto == 1;
        }

        // Ejemplo1: validarCCC("1234-5678-00-1234567890") --> false
        // Ejemplo2: validarCCC("1234-5678-06-1234567890") --> true
        function validarCCC(ccc) {
            ccc = limpiar(ccc);
            var items = formatearCCC(ccc, " ").split(" ");
            var dc = modulo11(items[0] + items[1]) + "" + modulo11(items[3]);
            return dc == items[2];
        }


        // Ejemplo: calcularCCC("1234-5678-??-1234567890") --> "12345678061234567890"
        function calcularCCC(ccc) {
            ccc = limpiar(ccc);
            return ccc.substr(0, 8) + calcularDC(ccc) + ccc.substr(10, 10);
        }

        // Ejemplo: calcularDC("1234-5678-??-1234567890") --> "06"
        function calcularDC(ccc) {
            ccc = limpiar(ccc);
            var items = formatearCCC(ccc, " ").split(" ");
            return modulo11(items[0] + items[1]) + "" + modulo11(items[3]);
        }

        // Ejemplo: formatearCCC("12345678061234567890") --> "1234-5678-06-1234567890"
        function formatearCCC(ccc, separador) {
            ccc = limpiar(ccc);
            if (separador == undefined) separador = "-";
            return ccc.substr(0, 4) + separador + ccc.substr(4, 4) + separador +
                    ccc.substr(8, 2) + separador + ccc.substr(10, 10);
        }

        // Ejemplo: formatearIBAN("ES6812345678061234567890") --> "ES68 1234 5678 0612 3456 7890"
        function formatearIBAN(iban, separador) {
            iban = limpiar(iban);
            if (separador == undefined) separador = " ";
            var items = [];
            for (var i = 0; i < 6; i++) { items.push(iban.substr(i * 4, 4)); }
            return items.join(separador);
        }

        //============================================
        // LOW LEVEL

        function esCCC(cifras) {
            ////return /^(\d{20})$/i.test(cifras);
            return cifras.length == 20;
        }

        function esIBAN(cifras) {
            ////return /^(\d{24})$/i.test(cifras);
            return cifras.length == 24;
        }

        // Ejemplo1: sonDigitos("1234") --> true
        // Ejemplo2: sonDigitos("12e4") --> false
        function sonDigitos(cifras) {
            var er = new RegExp("^(\\d{" + cifras.length + "})$", "i");
            return er.test(cifras);
        }

        // Ejemplo: limpiar("IBAN1234 5678-90") --> "1234567890"
        function limpiar(numero) {
            return numero
              .replace(/IBAN/g, "")
              .replace(/ /g, "")
              .replace(/-/g, "");
        }

        // Ejemplo: modulo("12345678061234567890142800", 97) --> 30
        function modulo(cifras, divisor) {
            /*
              El entero más grande en Javascript es 9.007.199.254.740.990 (2^53)
              que tiene 16 cifras, de las cuales las 15 últimas pueden tomar cualquier valor.
              El divisor y el resto tendrán 2 cifras. Por lo tanto CUENTA como tope
              puede ser de 13 cifras (15-2) y como mínimo de 1 cifra.
            */
            var CUENTA = 10;
            var largo = cifras.length;
            var resto = 0;
            for (var i = 0; i < largo; i += CUENTA) {
                var dividendo = resto + "" + cifras.substr(i, CUENTA);
                resto = dividendo % divisor;
            }
            return resto;
        }

        // Ejemplo1: modulo11("12345678") --> "0"
        // Ejemplo2: modulo11("1234567890") --> "6"
        function modulo11(cifras) {
            var modulos = [1, 2, 4, 8, 5, 10, 9, 7, 3, 6]; //2^index % 11
            var suma = 0;
            var cifras = cerosIzquierda(cifras, 10);
            for (var i = 0; i < cifras.length; i++) {
                suma += parseInt(cifras[i]) * modulos[i];
            }
            var control = suma % 11;
            return control < 2 ? control : 11 - control;
        }

        // Ejemplo: cerosIzquierda("7", 3) --> "007"
        function cerosIzquierda(cifras, largo) {
            cifras += '';
            while (cifras.length < largo) { cifras = '0' + cifras; }
            return cifras;
        }

        // Ejemplo: valorCifras("es") --> "1428"
        function valorCifras(cifras) {
            var LETRAS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // A=10, B=11, ... Z=35
            var items = [];
            for (var i = 0; i < cifras.length; i++) {
                var posicion = LETRAS.indexOf(cifras[i]);
                items.push(posicion < 0 ? "-" : posicion);
            }
            return items.join("");
        }

        //--------------------------------------------

        return {
            convertir: convertir,
            calcular: calcular,
            validar: validar,
            formatear: formatear,
        };

    })();

}]);
