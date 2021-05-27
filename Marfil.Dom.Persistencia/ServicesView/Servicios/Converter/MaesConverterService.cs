using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class MaesConverterService : BaseConverterModel<MaesModel, Maes>
    {
        public string Ejercicio { get; set; }

        public MaesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
                var ejercicio = Funciones.Qint(Ejercicio);
                var result = _db.Set<Maes>().Where(f => f.empresa == Empresa && f.fkejercicio == ejercicio).ToList().Select(f => GetModelView(f) as MaesModel).ToList();
            return result;
            
        }


        public override bool Exists(string id)
        {
            var ejercicio = Funciones.Qint(Ejercicio);
            return _db.Maes.Any(f => f.empresa == Context.Empresa && f.fkcuentas == id && f.fkejercicio == ejercicio);
        }

        public override IModelView CreateView(string id)
        {
           
            var ejercicio = Funciones.Qint(Ejercicio);
            var result = new MaesModel();
            
            var obj = _db.Set<Maes>().SingleOrDefault(f => f.empresa == Empresa && f.fkcuentas == id && f.fkejercicio == ejercicio);
            if (obj !=null)
            {
                result = GetModelView(obj) as MaesModel;
            }
            else
            {
                //inicializacion si no hay datos
                result.Fkcuentas = id;
                result.Debe = 0;
                result.Haber = 0;
                result.Saldo = 0;
            }

            // descripcion cuenta            
            var fservice = FService.Instance;
            var cuentaService = fservice.GetService(typeof(CuentasModel), Context, _db);
            var cuentaModel = cuentaService.get(id) as CuentasModel;
            result.Descripcion = cuentaModel.Descripcion;

            return result;
        }

        public override IModelView GetModelView(Maes obj)
        {
            var result = new MaesModel();
            if (obj != null)
            {
                result.Fkejercicio = obj.fkejercicio;
                result.Fkcuentas = obj.fkcuentas;
                result.Empresa = obj.empresa;
                result.Debe = obj.debe;
                result.Haber = obj.haber;
                result.Saldo = obj.saldo;
            }
            else
            {
                result.Fkejercicio = obj.fkejercicio;
                result.Fkcuentas = obj.fkcuentas;
                result.Empresa = obj.empresa;
                result.Debe = 0;
                result.Haber = 0;
                result.Saldo = 0;
            };
            return result;
        }
    }
}
