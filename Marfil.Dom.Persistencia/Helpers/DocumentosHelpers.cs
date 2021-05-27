using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.Helpers
{
    internal class DocumentosHelpers
    {
        public static void GenerarCarpetaAsociada(IModelView model,TipoDocumentos tipo,IContextService context, MarfilEntities db)
        {
            var carpetasService = FService.Instance.GetService(typeof(CarpetasModel), context, db) as CarpetasService;
            var ejercicioService =FService.Instance.GetService(typeof (EjerciciosModel), context, db) as EjerciciosService;

            if (tipo != TipoDocumentos.Articulos && tipo != TipoDocumentos.Materiales && tipo != TipoDocumentos.GrupoMateriales && tipo != TipoDocumentos.Almacenes && tipo != TipoDocumentos.Familias && tipo !=TipoDocumentos.TransformacionesAcabados)
            {
                var ejercicioModel = ejercicioService.get(model.get("Fkejercicio").ToString()) as EjerciciosModel;

                if (!carpetasService.ExisteCarpeta(Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
                Funciones.GetEnumByStringValueAttribute(tipo), ejercicioModel.Descripcioncorta, model.get("Referencia").ToString())))
                {
                    var carpeta = carpetasService.GenerarCarpetaAsociadaDocumento(tipo, ejercicioModel.Descripcioncorta, model.get("Referencia").ToString());
                    model.set("Fkcarpetas", carpeta.Id);
                }
                else
                {
                    var ruta = carpetasService.GenerateRutaCarpeta(tipo, ejercicioModel.Descripcioncorta, model.get("Referencia").ToString());
                    var carpeta = carpetasService.GetCarpeta(ruta);
                    model.set("Fkcarpetas", carpeta.Id);
                }
            }
            else
            {
                if (!carpetasService.ExisteCarpeta(Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
                    tipo.ToString(), "Imagenes", model.GetPrimaryKey())))
                {
                    var carpeta = carpetasService.GenerarCarpetaAsociada(tipo.ToString(), "Imagenes", model.GetPrimaryKey());
                    model.set("Fkcarpetas", carpeta.Id);
                }
                else
                {
                    var ruta = carpetasService.GenerateRutaCarpeta(tipo.ToString(), "Imagenes", model.GetPrimaryKey());
                    var carpeta = carpetasService.GetCarpeta(ruta);
                    model.set("Fkcarpetas", carpeta.Id);
                }
            }

        }

        public static void GenerarCarpetaAsociadaYGuardarBD(IModelView model, TipoDocumentos tipo, IContextService context, MarfilEntities db)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            using (var servModel = FService.Instance.GetService(model.GetType(), context, db))
            {
                GenerarCarpetaAsociada(model, tipo, context, db);
                if(servModel.exists(model.GetPrimaryKey()))
                {
                    servModel.edit(model);
                }
            }
        }
    }
}
