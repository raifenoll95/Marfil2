using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IUsuariosService
    {

    }

    public class UsuariosService : GestionService<UsuariosModel, Usuarios>, IUsuariosService
    {
        #region CTR

        public UsuariosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Usuario" };
            var propiedades = Helpers.Helper.getProperties<UsuariosModel>();
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select  id, usuario from usuarios where id!='{0}'",Guid.Empty);
        }

        #endregion

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.edit(obj);

                var usuarioModel = obj as UsuariosModel;
                if (usuarioModel.Id == Guid.Empty)
                {
                    var administrador = _db.Administrador.Single();
                    if (administrador.password != usuarioModel.Password)
                    {
                        _db.Administrador.Remove(administrador);
                        var nuevo = _db.Administrador.Create();
                        nuevo.password = usuarioModel.Password;
                        _db.Administrador.AddOrUpdate(nuevo);
                        _db.SaveChanges();
                    }
                }
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            var modelview = obj as UsuariosModel;
            if(modelview.Id==Guid.Empty)
                throw new ValidationException("No se puede eliminar el usuario administrador");

            base.delete(obj);
        }

        public UsuariosModel Get(string username)
        {
            return _converterModel.GetModelView(_db.Usuarios.Single(f => f.usuario == username)) as UsuariosModel;
        }

        public override string FirstRegister()
        {
            var obj = _db.Set<Usuarios>().FirstOrDefault(f => f.id != Guid.Empty);
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        public override string LastRegister()
        {
            var keyNames = GetprimarykeyColumns();
            var enumerable = keyNames as string[] ?? keyNames.ToArray();
            var query = _db.Set<Usuarios>();

            var flagFirst = true;
            IOrderedQueryable<Usuarios> orderedQuery = null;
            foreach (var item in enumerable)
            {

                orderedQuery = flagFirst ? query.OrderByDescending(item) : orderedQuery.ThenByDescending(item);
                flagFirst = false;

            }
            var obj = orderedQuery.FirstOrDefault(f => f.id != Guid.Empty);
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        public override string PreviousRegister(string id)
        {

            var result = base.PreviousRegister(id);
            return !string.IsNullOrEmpty(result) && result != Guid.Empty.ToString() ? result : string.Empty;
        }

        public override string NextRegister(string id)
        {
            var result = base.NextRegister(id);
            return !string.IsNullOrEmpty(result) && result != Guid.Empty.ToString() ? result : string.Empty;
        }

       
    }
}
