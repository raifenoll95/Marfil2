using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using System.IO;
using System.Data;
using System.Text;
using DevExpress.DataAccess.Sql;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class SeccionesAnaliticasController : GenericController<SeccionesanaliticasModel>
    {
        #region Seguridad controlador
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "seccionesanaliticas";
            var permisos = appService.GetPermisosMenu("seccionesanaliticas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }
        #endregion

        #region CTR
        public SeccionesAnaliticasController(IContextService context):base(context)
        {
        }
        #endregion

        #region Asistente Importación

        public ActionResult AsistenteSeccionesAnaliticas()
        {
            return View(new AsistenteSeccionesAnaliticasModel()
            {

            });
        }

        [HttpPost]                            
        public ActionResult AsistenteSeccionesAnaliticas(AsistenteSeccionesAnaliticasModel model)
        {
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var service = FService.Instance.GetService(typeof(SeccionesanaliticasModel), ContextService) as SeccionesanaliticasService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("CodSec");
                        dt.Columns.Add("Nombre");                 

                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString();
                                        }
                                        catch(Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("AsistenteSeccionesAnaliticas", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                j++;
                            }
                        }
                        try
                        {
                            service.Importar(dt, ContextService);
                            sr.Close();
                        }
                        catch (Exception ex)
                        {                           
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }

                        TempData["Success"] = "Importado correctamente!";
                        return RedirectToAction("AsistenteSeccionesAnaliticas", "SeccionesAnaliticas");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);                        
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("AsistenteSeccionesAnaliticas", model);
        }

        #endregion

    }
}