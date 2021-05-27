using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using RAlbaranes=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    internal  class AlbaranesComprasGesionLotesService
    {
        public static void GestionarLote(ArticulosModel articulo,FamiliasproductosModel familia,IEnumerable<IDocumentosLinModel> lineas, IDocumentoLinVistaModel linea, out string lote, out string loteautomaticoid, out int lotenuevocontador,
            out int? tabla)
        {
            lote = "";
            loteautomaticoid = "";
            lotenuevocontador = 0;
            tabla = null;

            if (articulo.Tipogestionlotes == Tipogestionlotes.Loteobligatorio || (articulo.Tipogestionlotes==Tipogestionlotes.Loteopcional &&  (linea.Loteautomatico|| !string.IsNullOrEmpty(linea.Lote))))
            {
                var contadorLotes = 1;
                //Si Lote automatico
                if (linea.Loteautomatico && string.IsNullOrEmpty(linea.Lote))//Lote automatico y es un nuevo lote
                {
                    contadorLotes = lineas.Any(f => f.Lotenuevocontador > 0) ? lineas.Max(f => f.Lotenuevocontador) : 0;
                    contadorLotes++;

                }
                else if (linea.Loteautomatico && !string.IsNullOrEmpty(linea.Lote))//Si el lote es automatico y ya existe
                {
                    contadorLotes = lineas.First(f => (f.Loteautomaticoid?.ToLower()??string.Empty) == linea.Lote.ToLower()).Lotenuevocontador;
                }
                else if (!linea.Loteautomatico && !string.IsNullOrEmpty(linea.Lote))//Si el lote es manual
                {
                    contadorLotes = lineas.Any(f => f.Lote.ToLower() == linea.Lote.ToLower()) ? lineas.Max(f => f.Lotenuevocontador) : 0; //comprbamos que si el lote manual existe y asignamos el siguiente contador, sino usamos el  0 por defecto
                    contadorLotes++;
                }
                else
                {
                    throw new ValidationException(RAlbaranes.ErrorGestionLotes);
                }

                lote = linea.Loteautomatico ? RAlbaranes.Lote + (contadorLotes) : linea.Lote;
                loteautomaticoid = linea.Loteautomatico && string.IsNullOrEmpty(linea.Lote) ? Guid.NewGuid().ToString() : linea.Lote;

                var loteactual = linea.Loteautomatico ? loteautomaticoid : lote;

                //no se pueden duplicar lotes para elementos tipos tablas
                if (linea.Tipofamilia != TipoFamilia.Tabla && lineas.Any(f => f.Lote == loteactual || f.Loteautomaticoid == loteactual))
                {
                    throw new ValidationException(RAlbaranes.ErrorLoteRepetido);
                }

                if (!linea.Loteautomatico && lineas.Any(f => f.Lote == loteactual))
                {
                    //comprobar las condiciones
                    var oldlinea = lineas.First(f => f.Lote == loteactual);
                    var oldfamilia = ArticulosService.GetCodigoFamilia(oldlinea.Fkarticulos);
                    var oldmaterial = ArticulosService.GetCodigoMaterial(oldlinea.Fkarticulos);
                    var newfamilia = ArticulosService.GetCodigoFamilia(linea.Fkarticulos);
                    var newmaterial = ArticulosService.GetCodigoMaterial(linea.Fkarticulos);
                    if (oldfamilia != newfamilia || oldmaterial != newmaterial)
                    {
                        throw new ValidationException(string.Format("No se puede agregar el artículo al lote {0} porque las propiedades del lote no coinciden con las del artículo nuevo", loteactual));
                    }
                }

                loteautomaticoid = linea.Loteautomatico ? loteautomaticoid : string.Empty;
                lotenuevocontador = contadorLotes;
                if (familia.Tipofamilia == TipoFamilia.Tabla)
                {
                    tabla = linea.Loteautomatico
                        ? null
                        : (int?)
                            (lineas.Any() ? (lineas.Where(f => f.Lote == linea.Lote).Max(j => j.Tabla) + 1 ?? 1) : 1);
                }
                else if(!string.IsNullOrEmpty(lote))
                    tabla = linea.Loteautomatico ? null : (int?)0;


            }




        }

    }
}
