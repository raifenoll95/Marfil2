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

        [HttpPost]
        public void ImprimirCuaderno(string valorCuaderno)
        {
            Escribir(valorCuaderno);

            //return "El cuaderno es " + valorCuaderno;
        }

        private void Descargar(string valorCuaderno, string tipoFormato, string filepath)
        {
            Response.Clear();
            Response.AddHeader("Content-disposition", "Attachment; Filename=" + valorCuaderno +"."+tipoFormato);
            Response.ContentType = "Text/Xml";
            Response.WriteFile(Server.MapPath(filepath));
            Response.End();
        }

        private void Escribir(string valorCuaderno)
        {
            List<CuadernosBancariosLin> cabecera = new List<CuadernosBancariosLin>();
            List<CuadernosBancariosLin> detalle = new List<CuadernosBancariosLin>();
            List<CuadernosBancariosLin> total = new List<CuadernosBancariosLin>();
            var tipoFormato = "";
            string filepath = "";

            using (var service = new RemesasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
            {
                var idCuaderno = service.GetCuaderno(valorCuaderno);
                var formato = service.GetFormato(valorCuaderno);
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
            }

            if (tipoFormato == "txt")//Si es txt
            {
                filepath = @"C:\Desarrolllo\Documentación\Marfil\" + valorCuaderno + ".txt";
                using (StreamWriter sw = new StreamWriter(filepath, false))
                {
                    foreach (var item in cabecera)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        if (item.campo == null)
                        {
                            sw.Write(" ");
                        }
                        else
                        {
                            sw.Write(item.campo);
                        }

                    }
                    sw.WriteLine();
                    foreach (var item in detalle)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        if (item.campo == null)
                        {
                            sw.Write(" ");
                        }
                        else
                        {
                            sw.Write(item.campo);
                        }
                    }
                    sw.WriteLine();
                    foreach (var item in total)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        if (item.campo == null)
                        {
                            sw.Write(" ");
                        }
                        else
                        {
                            sw.Write(item.campo);
                        }
                    }
                    sw.Close();
                }
            }
            else//si es xml
            {
                filepath = @"C:\Desarrolllo\Documentación\Marfil\" + valorCuaderno + ".xml";
                using (StreamWriter sw = new StreamWriter(filepath, false))
                {
                    foreach (var item in cabecera)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        sw.WriteLine(item.etiquetaIni + " " + item.campo + " " + item.etiquetaFin);
                    }
                    sw.WriteLine();
                    foreach (var item in detalle)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        sw.WriteLine(item.etiquetaIni + " " + item.campo + " " + item.etiquetaFin);
                    }
                    sw.WriteLine();
                    foreach (var item in total)
                    {
                        sw.BaseStream.Seek((long)item.posicion, SeekOrigin.Begin);
                        sw.WriteLine(item.etiquetaIni + " " + item.campo + " " + item.etiquetaFin);
                    }
                    sw.Close();
                }
            }

            //Descargar(valorCuaderno, tipoFormato, filepath);

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