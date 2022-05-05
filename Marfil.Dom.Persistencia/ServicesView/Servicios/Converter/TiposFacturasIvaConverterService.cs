using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class TiposFacturasIvaConverterService : BaseConverterModel<TiposFacturasIvaModel, Persistencia.TiposFacturas>
    {
        public TiposFacturasIvaConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.TiposFacturas>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idnum = Int32.Parse(id);
            var obj = _db.Set<Persistencia.TiposFacturas>().Single(f => f.empresa == Empresa && f.id == idnum);
            var result = GetModelView(obj) as TiposFacturasIvaModel;
            return result;
        }

        public override Persistencia.TiposFacturas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as TiposFacturasIvaModel;
            var result = _db.Set<Persistencia.TiposFacturas>().Create();
            result.empresa = viewmodel.Empresa;
            result.tipocircuito = (int)viewmodel.Tipocircuito;
            result.descripcion = viewmodel.Descripcion;
            result.regimeniva = viewmodel.Regimeniva;          
            result.tipofacturadefecto = viewmodel.Tipofacturadefecto;
            result.ivadeducible = viewmodel.Ivadeducible;
            result.requiereirpf = viewmodel.Requiereirpf;
            result.operacionesexcluidas = viewmodel.Operacionesexcluidas;
            result.bieninversion = viewmodel.Bieninversion;
            result.cuentacargo1 = viewmodel.Cuentacargo1;
            result.cuentacargo2 = viewmodel.Cuentacargo2;
            result.cuentacargo3 = viewmodel.Cuentacargo3;
            result.cuentaabono1 = viewmodel.Cuentaabono1;
            result.cuentaabono2 = viewmodel.Cuentaabono2;
            result.cuentaabono3 = viewmodel.Cuentaabono3;
            result.importecuentacargo1 = (int)viewmodel.Importecuentacargo1;
            result.importecuentacargo2 = (int)viewmodel.Importecuentacargo2;
            result.importecuentacargo3 = (int)viewmodel.Importecuentacargo3;
            result.importecuentaabono1 = (int)viewmodel.Importecuentaabono1;
            result.importecuentaabono2 = (int)viewmodel.Importecuentaabono2;
            result.importecuentaabono3 = (int)viewmodel.Importecuentaabono3;
            result.desccuentacargo1 = viewmodel.Desccuentacargo1;
            result.desccuentacargo2 = viewmodel.Desccuentacargo2;
            result.desccuentacargo3 = viewmodel.Desccuentacargo3;
            result.desccuentaabono1 = viewmodel.Desccuentaabono1;
            result.desccuentaabono2 = viewmodel.Desccuentaabono2;
            result.desccuentaabono3 = viewmodel.Desccuentaabono3;
            return result;
        }

        public override Persistencia.TiposFacturas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as TiposFacturasIvaModel;
            var result = _db.TiposFacturas.Where(f => f.id == viewmodel.Id && f.empresa == Empresa).Single();
            result.empresa = viewmodel.Empresa;
            result.tipocircuito = (int)viewmodel.Tipocircuito;
            result.descripcion = viewmodel.Descripcion;
            result.regimeniva = viewmodel.Regimeniva;
            result.tipofacturadefecto = viewmodel.Tipofacturadefecto;
            result.ivadeducible = viewmodel.Ivadeducible;
            result.requiereirpf = viewmodel.Requiereirpf;
            result.operacionesexcluidas = viewmodel.Operacionesexcluidas;
            result.bieninversion = viewmodel.Bieninversion;
            result.cuentacargo1 = viewmodel.Cuentacargo1;
            result.cuentacargo2 = viewmodel.Cuentacargo2;
            result.cuentacargo3 = viewmodel.Cuentacargo3;
            result.cuentaabono1 = viewmodel.Cuentaabono1;
            result.cuentaabono2 = viewmodel.Cuentaabono2;
            result.cuentaabono3 = viewmodel.Cuentaabono3;
            result.importecuentacargo1 = (int)viewmodel.Importecuentacargo1;
            result.importecuentacargo2 = (int)viewmodel.Importecuentacargo2;
            result.importecuentacargo3 = (int)viewmodel.Importecuentacargo3;
            result.importecuentaabono1 = (int)viewmodel.Importecuentaabono1;
            result.importecuentaabono2 = (int)viewmodel.Importecuentaabono2;
            result.importecuentaabono3 = (int)viewmodel.Importecuentaabono3;
            result.desccuentacargo1 = viewmodel.Desccuentacargo1;
            result.desccuentacargo2 = viewmodel.Desccuentacargo2;
            result.desccuentacargo3 = viewmodel.Desccuentacargo3;
            result.desccuentaabono1 = viewmodel.Desccuentaabono1;
            result.desccuentaabono2 = viewmodel.Desccuentaabono2;
            result.desccuentaabono3 = viewmodel.Desccuentaabono3;
            return result;
        }

        public override IModelView GetModelView(Persistencia.TiposFacturas viewmodel)
        {
            var result = new TiposFacturasIvaModel
            {
                Id = viewmodel.id,
                Empresa = viewmodel.empresa,
                Descripcion = viewmodel.descripcion,
                Tipocircuito = (TipoFactura)viewmodel.tipocircuito,
                Regimeniva = viewmodel.regimeniva,              
                Tipofacturadefecto = viewmodel.tipofacturadefecto.Value,
                Ivadeducible = viewmodel.ivadeducible.Value,
                Requiereirpf = viewmodel.requiereirpf.Value,
                Operacionesexcluidas = viewmodel.operacionesexcluidas.Value,
                Bieninversion = viewmodel.bieninversion.Value,
                Cuentacargo1 = viewmodel.cuentacargo1,
                Cuentacargo2 = viewmodel.cuentacargo2,
                Cuentacargo3 = viewmodel.cuentacargo3,
                Cuentaabono1 = viewmodel.cuentaabono1,
                Cuentaabono2 = viewmodel.cuentaabono2,
                Cuentaabono3 = viewmodel.cuentaabono3,
                Importecuentacargo1 = (TipoImporte)viewmodel.importecuentacargo1,
                Importecuentacargo2 = (TipoImporte)viewmodel.importecuentacargo2,
                Importecuentacargo3 = (TipoImporte)viewmodel.importecuentacargo3,
                Importecuentaabono1 = (TipoImporte)viewmodel.importecuentaabono1,
                Importecuentaabono2 = (TipoImporte)viewmodel.importecuentaabono2,
                Importecuentaabono3 = (TipoImporte)viewmodel.importecuentaabono3,
                Desccuentacargo1 = viewmodel.desccuentacargo1,
                Desccuentacargo2 = viewmodel.desccuentacargo2,
                Desccuentacargo3 = viewmodel.desccuentacargo3,
                Desccuentaabono1 = viewmodel.desccuentaabono1,
                Desccuentaabono2 = viewmodel.desccuentaabono2,
                Desccuentaabono3 = viewmodel.desccuentaabono3
            };

            return result;
        }


    }
}
