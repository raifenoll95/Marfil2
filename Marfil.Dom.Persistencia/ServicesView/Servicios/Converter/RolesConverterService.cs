using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class RolesConverterService : BaseConverterModel<RolesModel, Roles>
    {
        private readonly ISerializer<PermisosModel> _serializerPermisos;

        public RolesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            _serializerPermisos=new Serializer<PermisosModel>();
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Roles>().Include(f=>f.Usuarios).ToList().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Roles>().Where(f => f.id == new Guid(id)).Include(f => f.Usuarios).Single();
            return GetModelView(obj);
        }

        public override Roles CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as RolesModel;
            var result = _db.Set<Roles>().Create();
            result.id = viewmodel.Id;
            result.role = viewmodel.Role;
            result.permisos = _serializerPermisos.GetXml(viewmodel.Permisos);
            foreach (var item in viewmodel.Usuarios.usuarios)
                result.Usuarios.Add(_db.Usuarios.Single(f=>f.id == item.id));

            return result;
        }

        public override Roles EditPersitance(IModelView obj)
        {
            var viewmodel = obj as RolesModel;
            var result = _db.Roles.Where(f => f.id == viewmodel.Id).Include(b => b.Usuarios).ToList().Single();
            result.id = viewmodel.Id;
            result.role = viewmodel.Role;
            result.permisos = _serializerPermisos.GetXml(viewmodel.Permisos);
            result.Usuarios.Clear();
            foreach (var item in viewmodel.Usuarios.usuarios)
                result.Usuarios.Add(_db.Usuarios.Find(item.id));

            

            return result;
        }

        public override IModelView GetModelView(Roles obj)
        {
            var result = new RolesModel
            {
                Context= Context,
                Role = obj.role,
                Id = obj.id,
                Permisos = _serializerPermisos.SetXml(obj.permisos),
                Usuarios =
                {
                    usuarios =
                        obj.Usuarios.Select(
                            item => new UsuariosRelacionRolesModel() {id = item.id, usuario = item.usuario}).ToList()
                }
            };

            return result;
        }

       
    }
}
