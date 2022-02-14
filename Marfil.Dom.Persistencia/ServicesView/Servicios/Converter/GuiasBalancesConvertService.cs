using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class GuiasBalancesConvertService : BaseConverterModel<GuiasBalancesModel, GuiasBalances>
    {
        public GuiasBalancesConvertService(IContextService context, MarfilEntities db) : base(context, db)
        {
                
        }
        public override IModelView CreateView(string id)
        {
            var idparse = int.Parse(id);
            var obj = _db.GuiasBalances.Include("GuiasBalancesLineas").Single(f => f.id == idparse);
            var result = GetModelView(obj) as GuiasBalancesModel;
            result.TextoGrupo = obj.textogrupo;
            result.RegDig = obj.regdig;
            result.Lineas =
               obj.GuiasBalancesLineas.ToList().Select(
                   item =>
                       new GuiasBalancesLineasModel()
                       {
                           Id = item.id,
                           GuiaId = (GuiasBalancesLineasModel.TipoGuiaE)item.guiaId,
                           Orden = item.orden,
                           InformeId = (GuiasBalancesLineasModel.TipoInformeE)item.informeId,
                           GuiasBalancesId = (int)item.guiasBalancesId,
                           Cuenta = item.cuenta,
                           Signo = item.signo,
                           Signoea = item.signoea,
                           Empresa = item.empresa
                       }).ToList();

            return result;
        }

        public override GuiasBalances CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as GuiasBalancesModel;
            var result = _db.GuiasBalances.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "guiasbalanceslineas")
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            /*result.guiaId = (int)viewmodel.GuiaId;
            result.informeId = (int)viewmodel.InformeId;*/
            result.textogrupo = viewmodel.TextoGrupo;
            result.regdig = viewmodel.RegDig;

            result.GuiasBalancesLineas.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.GuiasBalancesLineas.Create();
                newItem.empresa = item.Empresa;
                newItem.cuenta = item.Cuenta;
                newItem.guiaId = (int)result.guiaId;//misma que de la cabecera
                newItem.guiasBalancesId = (int)result.id;//misma que de la cabecera
                newItem.id = item.Id;
                newItem.informeId = (int)result.informeId;//misma que de la cabecera
                newItem.orden = result.orden;//misma que de la cabecera
                newItem.signo = item.Signo;
                newItem.signoea = item.Signoea;
                result.GuiasBalancesLineas.Add(newItem);
            }
            return result;
        }
        public override GuiasBalances EditPersitance(IModelView obj)
        {
            var viewmodel = obj as GuiasBalancesModel;
            var result = _db.Set<GuiasBalances>().Single(f => f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "guiasbalanceslineas")
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            /*result.guiaId = (int)viewmodel.GuiaId;
            result.informeId = (int)viewmodel.InformeId;*/
            result.textogrupo = viewmodel.TextoGrupo;
            result.regdig = viewmodel.RegDig;

            result.GuiasBalancesLineas.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.GuiasBalancesLineas.Create();
                newItem.empresa = item.Empresa;
                newItem.cuenta = item.Cuenta;
                newItem.guiaId = (int)result.guiaId;
                newItem.guiasBalancesId = (int)result.id;
                newItem.id = item.Id;
                newItem.informeId = (int)result.informeId;
                newItem.orden = result.orden;
                newItem.signo = item.Signo;
                newItem.signoea = item.Signoea;
                result.GuiasBalancesLineas.Add(newItem);
            }
            return result;
        }

    }
}
