app.controller('SaldarpedidoCtrl', ['$scope', '$rootScope', '$http', '$interval', function ($scope, $rootScope, $http, $interval) {

    $scope.Pedidosaldado = false;
    $scope.Carga = false;
    $scope.Urlarticulos = "";
    $scope.Urlsaldar = "";
    $scope.Articulos = [];
    $scope.ArticulosBackup = [];
    var getLineasSaldarPedidos = function () {
        $.get($scope.Urlarticulos +"/"+ $("#Fkpedidos").val()).success(function (message) {
            $scope.Articulos = message;
            $scope.ArticulosBackup =JSON.parse(JSON.stringify(message)) ;
            $scope.Carga = false;
            $scope.$apply();
        }).error(function (jqXHR, textStatus, errorThrown) {
            $scope.Articulos = [];
            $scope.Carga = false;
            $scope.$apply();
        });
    }

    $scope.CalcularPendientes =function(linea) {

        if (Globalize.parseFloat(linea.Metros) == 0) {
            linea.Cantidadpendiente = 0;
        } else {
            var cantidadalbaran = linea.Cantidadpedida * Globalize.parseFloat(linea.Metrosalbaran) / Globalize.parseFloat(linea.Metros);
            linea.Cantidadpendiente = Math.round((linea.Cantidadpedida - cantidadalbaran) * 100) / 100;
        }
        
    }

    $scope.init = function init(urlarticulos,urlsaldar) {
        $scope.Urlarticulos = urlarticulos;
        $scope.Urlsaldar = urlsaldar;
        $scope.Pedidosaldado = $("#Pedidosaldado").val().toLowerCase()=="true";
    }

    $scope.ClearFormulario=function() {
        getLineasSaldarPedidos();
    }

    //Saldar lineas

    var saldarPedido =function(linea) {
        linea.Cantidadpendiente = 0;
        linea.Metrosalbaran = linea.Metros;
    }

    var saldarAlbaran = function (linea) {
        var lineabackup = $.grep($scope.ArticulosBackup, function (e) { return e.Id == linea.Id; });
        
        linea.Metrosalbaran = lineabackup[0].Metrosalbaran;
        $scope.CalcularPendientes(linea);
    }

    var noSaldar = function (linea) {
       
        linea.Cantidadpendiente = linea.Cantidadpedida;
        linea.Metrosalbaran = 0;
    }

    $scope.SaldarLinea =function(operacion, linea) {
        switch(operacion) {
            case 0:
                saldarPedido(linea);
                break;
            case 1:
                saldarAlbaran(linea);
                break;
            case 2:
                noSaldar(linea);
                break;
        }
    }
    
    //saldar lineas

    $('#saldarpedidomodal').on('show.bs.modal', function () {
        $scope.Carga = true;
        $scope.ClearFormulario();
        $scope.$apply();
    });
  
      eventAggregator.RegisterEvent("SaldarPedido",function(msg) {
          $("#saldarpedidomodal").modal();
      });
    //Saldar pedido funcion

      var Validarenviar = function () {
          return true;
      }

      var Enviar = function () {
          
              var Model = {};
              Model.Fkpedidos = $("[name='Fkpedidos']").val();
              Model.Referenciaentrega = $("[name='Referencia']").val();
              Model.Lineas = $scope.Articulos;
              
              var req = {
                  method: 'POST',
                  url: $scope.Urlsaldar,
                  data: JSON.stringify(Model),
                  contentType: 'application/json',
                  dataType: 'json'
              }
              $http(req)
                  .success(function (response) {
                      if (response.Error) {
                          $scope.Generalerrores = response.Error;
                      } else {
                          $scope.Pedidosaldado = true;
                          messagesService.show(TipoMensaje.Exito, "Bien!", "Operacion realizada correctamente.")
                          $("#saldarpedidomodal").modal('hide');
                      }
                  }).error(function (data, status, headers, config, statusText) {
                      $scope.Generalerrores = data.error;
                  });
         
      }

      $scope.Aceptar = function() {
          if (Validarenviar()) {
              Enviar();
          }
      }
    //end saldar pedido funcion
     
}]);