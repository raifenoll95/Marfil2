function ModalDocumentosAsociados(controller,cadenavector) {
    var vector = JSON.parse(cadenavector);
    //titulodocumentoasociado
    $("#titulodocumentoasociado").html( "Ver " + controller);
    //enlacesdocumentoasociado
    var listadoHtml = "<ul class=\"list-group\">";
    for(var i=0;i<vector.length;i++)
    {
        listadoHtml += "<li class=\"list-group-item\">Ver el documento <a href=\""+ vector[i].url+"\">"+ vector[i].referencia +"</a></li>";
    }
    listadoHtml += "</ul>";
    $("#enlacesdocumentoasociado").html(listadoHtml);
    $("#documentosasociados").modal();
}