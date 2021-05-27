using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ContactosConverterService : BaseConverterModel<ContactosLinModel, Contactos>
    {
        

        #region CTR

        public ContactosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
           
        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Set<Contactos>().Where(f => f.empresa == Empresa).ToList();

            var result = new List<ContactosLinModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as ContactosLinModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {

            var st = GetPrimaryKey(id);
            return _db.Set<Contactos>().Any(f => f.id == st.Id && f.empresa == st.Empresa  && f.fkentidad == st.Fkentidad);
        }

        public override IModelView CreateView(string id)
        {

            var service = new TablasVariasService(Context,_db);

            //var idiomasList = service.GetTablasVariasByCode(1100).Lineas.Select(f => (TablasVariasIdiomasAplicacion)f); //idiomas
            var cargosEmpresaList = service.GetTablasVariasByCode(2050).Lineas.Select(f => (TablasVariasCargosEmpresaModel)f); //idiomas
            var tiposContactoList = service.GetTablasVariasByCode(2040).Lineas.Select(f => (TablasVariasGeneralModel)f); //idiomas

            var st = GetPrimaryKey(id);
            var obj = _db.Set<Contactos>().Single(f => f.id == st.Id && f.empresa == st.Empresa && f.fkentidad == st.Fkentidad);

            var result = GetModelView(obj) as ContactosLinModel;
            //result.Idioma = idiomasList.SingleOrDefault(f => f.Valor == result.Fkidioma)?.Descripcion;
            result.CargoEmpresa = cargosEmpresaList.SingleOrDefault(f => f.Valor == result.Fkcargoempresa)?.Descripcion;
            result.TipoContacto = tiposContactoList.SingleOrDefault(f => f.Valor == result.Fktipocontacto)?.Descripcion;

            return result;
        }

        public override Contactos EditPersitance(IModelView obj)
        {
            var objext = obj as IModelViewExtension;
            var st = obj as ContactosLinModel;
            var result = _db.Set<Contactos>().Single(f => f.id == st.Id && f.empresa == st.Empresa && f.fkentidad == st.Fkentidad);

            foreach (var item in result.GetType().GetProperties())
            {
                var type = item.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    var value = obj.get(item.Name);
                    var t = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                    var safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                    item.SetValue(result, safeValue);
                    
                }
                else
                    item.SetValue(result, obj.get(item.Name));
            }

            return result;
        }

        public override IModelView GetModelView(Contactos obj)
        {
            return new ContactosLinModel()
            {
                Empresa = obj.empresa,
                Fkentidad = obj.fkentidad,
                Tipotercero = (TiposCuentas)obj.tipotercero,
                Id = obj.id,
                Nombre = obj.nombre,
                Fktipocontacto = obj.fktipocontacto,
                Fkcargoempresa = obj.fkcargoempresa,
                Fkidioma = obj.fkidioma,
                Telefono = obj.telefono,
                Telefonomovil = obj.telefonomovil,
                Fax = obj.fax,
                Email = obj.email??string.Empty,
                Nifcif = obj.nifcif,
                Observaciones = obj.observaciones,
                Fkid_direccion = obj.fkid_direccion
            };
        }

        #region Helpers

        private struct stPrimaryKey
        {
            public string Empresa { get; set; }
            public string Fkentidad { get; set; }
            public int Id { get; set; }
        }

        private stPrimaryKey GetPrimaryKey(string id)
        {
            var vector = id.Split(ContactosLinModel.SeparatorPk);
            return new stPrimaryKey()
            {
                Empresa = Empresa,
                Fkentidad = vector[1], //antes 0
                Id = int.Parse(vector[2]) // antes 1
            };
        }

        #endregion
    }
}
