using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class CuadernosBancariosConverterServices : BaseConverterModel<CuadernosBancariosModel,CuadernosBancarios>
    {
        public CuadernosBancariosConverterServices(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override CuadernosBancarios CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CuadernosBancariosModel;
            var result = _db.Set<CuadernosBancarios>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "formato" && item.Name.ToLower() != "tipoCampo")
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
            result.formato = (int)viewmodel.Formato;
            result.tipoCampo = (int)viewmodel.TipoCampo;

            result.CuadernosBancariosLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.CuadernosBancariosLin.Create();
                newitem.empresa = result.empresa;
                newitem. idCab= result.id;
                newitem.id = item.Id;
                newitem.posicion = item.Posicion;
                newitem.orden = item.Orden;
                newitem.obligatorio = item.Obligatorio;
                newitem.campo = item.Campo;
                newitem.etiquetaFin = item.EtiquetaFin;
                newitem.etiquetaIni = item.EtiquetaIni;
                newitem.condicion = item.Condicion;
                newitem.descripcionLin = item.DescripcionLin;
                newitem.longitud = item.Longitud;
                newitem.registro = item.Registro;
                newitem.tipoCampo = (int)item.TipoCampo;
                result.CuadernosBancariosLin.Add(newitem);
            }

            return result;
        }
        public override IModelView CreateView(string id)
        {
            var idparse = int.Parse(id);
            var obj = _db.CuadernosBancarios.Include("CuadernosBancariosLin").Single(f => f.empresa == Context.Empresa && f.id == idparse);
            var result = GetModelView(obj) as CuadernosBancariosModel;
            result.Lineas =
               obj.CuadernosBancariosLin.ToList().Select(
                   item =>
                       new CuadernosBancariosLinModel()
                       {
                           Id = item.id,
                           Posicion = item.posicion ?? 0,
                           Orden = item.orden ?? 0,
                           Obligatorio = item.obligatorio ?? 0,
                           Campo = item.campo,
                           Condicion = item.condicion,
                           EtiquetaFin = item.etiquetaFin,
                           EtiquetaIni = item.etiquetaIni,
                           DescripcionLin = item.descripcionLin,
                           Longitud = item.longitud ?? 0,
                           Registro = item.registro,
                           TipoCampo = item.tipoCampo.HasValue ? (CuadernosBancariosLinModel.TipoCampos)item.tipoCampo.Value : CuadernosBancariosLinModel.TipoCampos.Alfanumerico
                       }).ToList();

            return result;
        }
        public override CuadernosBancarios EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CuadernosBancariosModel;
            var result = _db.Set<CuadernosBancarios>().Single(f => f.empresa == Context.Empresa && f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "formato" && item.Name.ToLower() != "tipoCampo")
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
            result.formato = (int)viewmodel.Formato;
            result.tipoCampo = (int)viewmodel.TipoCampo;

            result.CuadernosBancariosLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.CuadernosBancariosLin.Create();
                newitem.empresa = result.empresa;
                newitem.idCab = result.id;
                newitem.id = item.Id;
                newitem.posicion = item.Posicion;
                newitem.orden = item.Orden;
                newitem.obligatorio = item.Obligatorio;
                newitem.campo = item.Campo;
                newitem.etiquetaFin = item.EtiquetaFin;
                newitem.etiquetaIni = item.EtiquetaIni;
                newitem.condicion = item.Condicion;
                newitem.descripcionLin = item.DescripcionLin;
                newitem.longitud = item.Longitud;
                newitem.registro = item.Registro;
                newitem.tipoCampo = (int)item.TipoCampo;
                result.CuadernosBancariosLin.Add(newitem);
            }

            return result;
        }
        public override bool Exists(string id)
        {
            var idparse = int.Parse(id);
            return _db.CuadernosBancarios.Any(f => f.empresa == Context.Empresa && f.id == idparse);
        }

        public override IModelView GetModelView(CuadernosBancarios obj)
        {
            var result = base.GetModelView(obj) as CuadernosBancariosModel;

            result.Formato = obj.formato.HasValue ? (CuadernosBancariosModel.TipoFormatos)obj.formato : CuadernosBancariosModel.TipoFormatos.Fijo;
            result.TipoCampo = obj.tipoCampo.HasValue ? (CuadernosBancariosModel.TipoCampos)obj.tipoCampo : CuadernosBancariosModel.TipoCampos.Alfanumerico;

            return result;
        }

    }
}
