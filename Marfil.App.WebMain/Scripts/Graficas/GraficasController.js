app.controller('GraficasCtrl', ['$scope', '$rootScope', '$http', '$interval',  '$timeout', function($scope, $rootScope, $http, $interval, $timeout) {

    $scope._url = "";
    $scope._id = "";
    $scope._divid = "";
    $scope._tipografica = "";
    $scope._titulo = "";
    $scope._titulox = "";
    $scope.Loading = false;
    $scope._data = [];
    $scope.ImageUrl = "";
    var GenerarGrafica = function (divid, tipo) {
      
        switch(tipo) {
            case "0":
                return new google.visualization.AreaChart(document.getElementById(divid));
            case "1":
                return new google.visualization.BarChart(document.getElementById(divid));
            case "2":
                return new google.visualization.ColumnChart(document.getElementById(divid));
            case "3":
                return new google.visualization.LineChart(document.getElementById(divid));
            case "4":
                return new google.visualization.ScatterChart(document.getElementById(divid));
            case "5":
                return new google.visualization.Table(document.getElementById(divid));
            case "6":
                return new google.visualization.PieChart(document.getElementById(divid));

        }

        return new google.visualization.LineChart(document.getElementById(divid));
    }

    $scope.init = function (url,id,divid,tipografica,titulo,titulox,tituloy) {
        $scope._url = url;
        $scope._id = id;
        $scope._divid = divid;
        $scope._tipografica = tipografica;
        $scope._titulo = titulo;
        $scope._titulox = titulox;
        $scope._tituloy = tituloy;

        // Load the Visualization API and the piechart package.
        google.charts.load('current', { 'packages': ['corechart','table'] });

        // Set a callback to run when the Google Visualization API is loaded.
        google.charts.setOnLoadCallback($scope.Cargar);

        $(window).resize(function () {
            $scope.CargarGrafica();
        });

        eventAggregator.RegisterEvent("Imprimirgrafica", function(msg) {
            if (msg == $scope._id) {
                $scope.Imprimir();
            }
        });

    }

    $scope.Cargar = function () {
        $scope.Loading = true;
        $http({
            url: $scope._url, method: "POST", params: { id: $scope._id }
            })
          .success(function (response) {
              $scope.Loading = false;
              var json = JSON.parse(response);
              $scope._data = new google.visualization.DataTable();
              $scope._data.addColumn('string', $scope._titulox);
              $scope._data.addColumn('number', $scope._tituloy);
                json.forEach(function(item) {
                    $scope._data.addRows([[item[$scope._titulox], item[$scope._tituloy]]]);
                });


              $scope.CargarGrafica();
              
             
          }).error(function (data, status, headers, config) {
              
              $scope.Loading = false;
          });
    }

    $scope.CargarGrafica = function () {

        switch($scope._tipografica) {
            
        }
        var chart = GenerarGrafica($scope._divid, $scope._tipografica);
        if ($scope._tipografica != "5") {//Tipo tabla
            google.visualization.events.addListener(chart, 'ready', function () {
                $scope.ImageUrl = chart.getImageURI();
            });
        }
        

        var  options = {
            title: $scope._titulo,
            hAxis: {title: $scope._titulox},
            animation:{
            duration: 1000,
            easing: 'out',
            startup: true}
        };

        chart.draw($scope._data, options);
    }


    
    $scope.Imprimir = function () {
        if ($scope.ImageUrl != "") {
            popup = window.open('', 'popup', 'toolbar=no,menubar=no');
            popup.document.open();
            popup.document.write("<html><head></head><body onload='print()'>");
            popup.document.write('<img src="' + $scope.ImageUrl + '"/>');
            popup.document.write("</body></html>");
            popup.document.close();
        }
        
    }

   

}]);