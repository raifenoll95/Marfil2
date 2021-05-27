using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class TiposcuentasConverterService : BaseConverterModel<TiposCuentasModel, Tiposcuentas>
    {
        
        public TiposcuentasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Tiposcuentas>().Include(f => f.TiposcuentasLin).Where(f=>f.empresa==Empresa).ToList().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            
            var intId = int.Parse(id);
            var obj = _db.Tiposcuentas.Include(f=>f.TiposcuentasLin).Single(f => f.tipos == intId && f.empresa == Empresa);
            return GetModelView(obj);
        }

        public override bool Exists(string id)
        {
            var intId = int.Parse(id);
            return _db.Set<Tiposcuentas>().Any(f => f.tipos == intId && f.empresa == Empresa);
        }

        public override Tiposcuentas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TiposCuentasModel;
            var result = _db.Set<Tiposcuentas>().Create();
            result.empresa = viewmodel.Empresa;
            result.tipos = (int)viewmodel.Tipos;
            result.cuenta = viewmodel.Cuenta.ToUpper();
            result.descripcion = viewmodel.Descripcion;
            result.nifobligatorio = viewmodel.Nifobligatorio;
            result.categoria = (int)viewmodel.Categoria;
            result.TiposcuentasLin.Clear();
            if(viewmodel.Categoria==CategoriasCuentas.Contables)
            foreach (var item in viewmodel.Lineas)
            {
                result.TiposcuentasLin.Add(new TiposcuentasLin()
                {
                    empresa = item.Empresa,
                    fkTiposcuentas = (int)item.Tipo,
                    cuenta = item.Cuenta,
                    descripcion = item.Descripcion
                });
            }
            return result;
        }

        public override Tiposcuentas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TiposCuentasModel;
            var result = _db.Tiposcuentas.Include(f=>f.TiposcuentasLin).Single(f => f.tipos == (int)viewmodel.Tipos && f.empresa == viewmodel.Empresa);
            result.empresa = viewmodel.Empresa;
            result.tipos = (int)viewmodel.Tipos;
            result.cuenta = viewmodel.Cuenta.ToUpper();
            result.descripcion = viewmodel.Descripcion;
            result.nifobligatorio = viewmodel.Nifobligatorio;
            result.TiposcuentasLin.Clear();
            if (viewmodel.Categoria == CategoriasCuentas.Contables)
            {
                foreach (var item in viewmodel.Lineas)
                {
                    result.TiposcuentasLin.Add(new TiposcuentasLin()
                    {
                        empresa = item.Empresa,
                        fkTiposcuentas = (int)item.Tipo,
                        cuenta = item.Cuenta,
                        descripcion = item.Descripcion
                    });
                }
            }

            return result;
        }

        public override IModelView GetModelView(Tiposcuentas obj)
        {
            return new TiposCuentasModel
            {
                Empresa=obj.empresa,
                Tipos = (TiposCuentas)obj.tipos,
                Cuenta = obj.cuenta,
                Descripcion = obj.descripcion,
                Nifobligatorio = obj.nifobligatorio ?? false,
                Categoria = (CategoriasCuentas)obj.categoria,
                Lineas = obj.TiposcuentasLin.Select(f=>new TiposCuentasLinModel() {Empresa = f.empresa, Tipo = (TiposCuentas)f.fkTiposcuentas,Cuenta = f.cuenta,Descripcion = f.descripcion}).ToList()
            };
        }
    }
}
