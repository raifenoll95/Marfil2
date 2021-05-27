using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITiposivaService
    {

    }

    public class TiposivaService : GestionService<TiposIvaModel, TiposIva>, ITiposivaService
    {


        #region CTR

        public TiposivaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override string GetSelectPrincipal()
        {
            var sb=new StringBuilder();
            
          
            sb.AppendFormat("select *,porcentajerecargoequivalente as [PorcentajeRecargoEquivalencia] from {0} {1}", "TiposIva", string.Format(" where empresa='{0}'", Empresa)) ;

            return sb.ToString();

        }

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model =  base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var obj=new TiposIvaModel();
            var excluded =
                obj.getProperties()
                    .Where(f => f.property.Name != "Nombre" && f.property.Name != "PorcentajeIva" && f.property.Name!="Id")
                    .Select(f => f.property.Name).ToList();
            model.ExcludedColumns = excluded;

            return model;
        }

        public TiposIvaModel GetTipoIva(string fkgrupo, string  regimen)
        {
            var gruposivaService = FService.Instance.GetService(typeof (GruposIvaModel), _context, _db);
            var regimenService = FService.Instance.GetService(typeof(RegimenIvaModel), _context, _db);
            var grupomodel = gruposivaService.get(fkgrupo) as GruposIvaModel;
            var lineagrupoiva = grupomodel.Lineas.OrderByDescending(f => f.Desde).First(f => f.Desde <= DateTime.Now);
            var regimenivamodel = regimenService.get(regimen) as RegimenIvaModel;

            var codigoTipoIva = "";
            if (regimenivamodel.Normal)
            {
                codigoTipoIva = lineagrupoiva.FkTiposIvaSinRecargo;
            }
            else if (regimenivamodel.Recargo)
            {
                codigoTipoIva = lineagrupoiva.FkTiposIvaConRecargo;
            }
            else if (regimenivamodel.ExentoTasa)
            {
                codigoTipoIva = lineagrupoiva.FkTiposIvaExentoIva;
            }
            else
                throw new ValidationException("No hay IVA asociado al articulo-cliente");

            return get(codigoTipoIva) as TiposIvaModel;
        }
    }
}
