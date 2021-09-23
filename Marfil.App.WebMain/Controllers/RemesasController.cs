using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Resources;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Web.Mvc;
using System.Threading.Tasks;
using static Marfil.Dom.ControlsUI.Descarga.FileExtension;
using Marfil.Dom.Persistencia.Helpers;
using System.Diagnostics;
using System.Web.UI;
using Marfil.App.WebMain.Misc;

namespace Marfil.App.WebMain.Controllers
{
    public class RemesasController : GenericController<RemesasModel>
    {

        #region CTR
        public RemesasController(IContextService context) : base(context)
        {
        }
        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }


        protected override void CargarParametros()
        {
            MenuName = "Remesas";
            var permisos = appService.GetPermisosMenu("Remesas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = false;
            CanEliminar = false;
        }

        public FileResult ImprimirCuaderno(string valorCuaderno, string referencia)
        {
            return Escribir(valorCuaderno, referencia);

            //return "El cuaderno es " + valorCuaderno;
        }

        public FileResult Descargar(string filepath)
        {
            return File(filepath, "application/force- download", Path.GetFileName(filepath));
        }

        public FileResult Escribir(string valorCuaderno, string referencia)
        {
            List<CuadernosBancariosLin> cabecera = new List<CuadernosBancariosLin>();
            List<CuadernosBancariosLin> detalle = new List<CuadernosBancariosLin>();
            List<CuadernosBancariosLin> total = new List<CuadernosBancariosLin>();
            var tipoFormato = "";
            string filepath = "";

            using (var service = new RemesasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
            {
                var idCuaderno = service.GetCuadernoId(valorCuaderno);
                var formato = service.GetFormato(valorCuaderno);
                var remesa = service.GetRemesa(referencia);
                cabecera = service.GetCuadernoCabecera(idCuaderno);
                detalle = service.GetCuadernoDetalle(idCuaderno);
                total = service.GetCuadernoTotal(idCuaderno);
                if (formato == 0)
                {
                    tipoFormato = "txt";
                }
                else
                {
                    tipoFormato = "xml";
                }

                //filepath = @"C:\inetpub\wwwroot\MarfilBetaEstable\Remesas\" + valorCuaderno + "." + tipoFormato;
                /*LOCAL*/
                //filepath = Server.MapPath(valorCuaderno + "." + tipoFormato);
                filepath = System.Web.HttpContext.Current.Server.MapPath(valorCuaderno + "." + tipoFormato);
                filepath = filepath.Replace("Remesas","FicherosRemesas");

                var ordenAnterior = -1;
                int numero = 0;

                if (tipoFormato == "txt")//Si es txt
                {
                    using (TextWriter sw = new StreamWriter(filepath, false))
                    {
                        try
                        {
                            foreach (var item in cabecera)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    if (item.campo == "Blanco")
                                    {
                                        sw.Write(" ");
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.Write(item.descripcionLin);
                                    }
                                    else
                                    {
                                        sw.Write(service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa));
                                    }
                                }
                                else
                                {
                                    sw.Write(item.campo);
                                }
                            }

                            sw.WriteLine();//Cambio de tipo

                            foreach (var item in detalle)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    //Cambia el orden, cambia la línea
                                    if (item.orden != ordenAnterior && ordenAnterior != -1)
                                    {
                                        sw.WriteLine();
                                    }

                                    if (item.campo == "Blanco")
                                    {
                                        sw.Write(" ");
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.Write(item.descripcionLin);
                                    }
                                    else
                                    {
                                        sw.Write(service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa));
                                    }
                                    ordenAnterior = (int)item.orden;
                                }
                                else
                                {
                                    sw.Write(item.campo);
                                }
                            }

                            sw.WriteLine();//Cambio de tipo

                            foreach (var item in total)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    if (item.campo == "Blanco")
                                    {
                                        sw.Write(" ");
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.Write(item.descripcionLin);
                                    }
                                    else
                                    {
                                        sw.Write(service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa));
                                    }
                                }
                                else
                                {
                                    sw.Write(item.campo);
                                }
                            }

                            sw.Close();
                        }
                        catch (Exception ex)
                        {
                            //TempData["errors"] = ex.Message;
                            sw.WriteLine("**Error en el mapeo de campos del cuaderno bancario " + valorCuaderno + "**");

                            return File(filepath, "application/force- download", "Error - " + valorCuaderno + ".txt");
                        }
                        finally
                        {
                            sw.Close();
                        }
                    }
                }
                else//si es xml
                {
                    using (TextWriter sw = new StreamWriter(filepath, false))
                    {
                        try
                        {
                            foreach (var item in cabecera)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    if (item.campo == "Blanco")
                                    {
                                        sw.WriteLine(item.etiquetaIni + " " + item.etiquetaFin);
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.WriteLine(item.descripcionLin);
                                    }
                                    else
                                    {
                                        sw.WriteLine(item.etiquetaIni + service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa) + item.etiquetaFin);
                                    }
                                    
                                }
                                else
                                {
                                    sw.WriteLine(item.etiquetaIni + item.campo + item.etiquetaFin);
                                }
                            }

                            sw.WriteLine();//Cambio de tipo

                            foreach (var item in detalle)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    //Cambia el orden, cambia la línea
                                    if (item.orden != ordenAnterior && ordenAnterior != -1)
                                    {
                                        sw.WriteLine();
                                    }

                                    if (item.campo == "Blanco")
                                    {
                                        sw.WriteLine(item.etiquetaIni + " " + item.etiquetaFin);
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.WriteLine(item.descripcionLin);
                                    }
                                    else
                                    {
                                        sw.WriteLine(item.etiquetaIni + service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa) + item.etiquetaFin);
                                    }

                                    ordenAnterior = (int)item.orden;
                                }
                                else
                                {
                                    sw.WriteLine(item.etiquetaIni + item.campo + item.etiquetaFin);
                                }
                            }

                            sw.WriteLine();//Cambio de tipo

                            foreach (var item in total)
                            {
                                //sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                                //Comprobar si el campo es un número fijo
                                if (!int.TryParse(item.campo, out numero))
                                {
                                    if (item.campo == "Blanco")
                                    {
                                        sw.WriteLine(item.etiquetaIni + " " + item.etiquetaFin);
                                    }
                                    else if (item.campo == "Valor Fijo")
                                    {
                                        sw.WriteLine(item.etiquetaIni + item.descripcionLin + item.etiquetaFin);
                                    }
                                    else
                                    {
                                        sw.WriteLine(item.etiquetaIni + service.GetMapeo(item.campo, valorCuaderno, item.tipoCampo, remesa) + item.etiquetaFin);
                                    }
                                }
                                else
                                {
                                    sw.WriteLine(item.etiquetaIni + item.campo + item.etiquetaFin);
                                }
                            }

                            sw.Close();
                        }
                        catch (Exception ex)
                        {
                            TempData["errors"] = ex.Message;
                            sw.WriteLine("**Error en el mapeo de campos del cuaderno bancario - " + valorCuaderno + "**");

                            return File(filepath, "application/force- download", "Error - " + valorCuaderno + ".xml");
                        }
                        finally
                        {
                            sw.Close();
                        }
                    }
                }
            }

            return Descargar(filepath);
            /*System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            FileInfo file = new FileInfo(filepath);

            response.ClearContent();
            response.Clear();
            response.ContentType = ReturnFiletype("." + tipoFormato);
            response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name + ";");
            //response.AddHeader("Content-Length", file.Length.ToString());
            response.TransmitFile(filepath);
            //response.Flush();
            response.End();
            //Descargar(valorCuaderno, tipoFormato, filepath);*/


        }
        public static string ReturnFiletype(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".htm":
                case ".html":
                case ".log":
                    return "text/HTML";
                case ".txt":
                    return "text/plain";
                case ".doc":
                    return "application/ms-word";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".asf":
                    return "video/x-ms-asf";
                case ".avi":
                    return "video/avi";
                case ".zip":
                    return "application/zip";
                case ".xls":
                case ".csv":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case "jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".wav":
                    return "audio/wav";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mpg":
                case "mpeg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".asp":
                    return "text/asp";
                case ".pdf":
                    return "application/pdf";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".ppt":
                    return "application/mspowerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                case ".msg":
                    return "application/msoutlook";
                case ".xml":
                case ".sdxl":
                    return "application/xml";
                case ".xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }


        #region imprimir

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = "Remesa";
            return result;
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            RemesasModel objModel = model as RemesasModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            RemesasModel objModel = model as RemesasModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(RemesasModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Remesa, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Remesa, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }
        #endregion

    }
}