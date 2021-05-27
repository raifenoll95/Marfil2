using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Resources;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public abstract class GestionService<TView, TPersistencia> : IGestionService where TPersistencia : class where TView : class, IModelView
    {
        #region Members

        protected readonly MarfilEntities _db;
        internal IConverterModelService<TView, TPersistencia> _converterModel;
        internal IValidationService<TPersistencia> _validationService;
        public IContextService _context;
        protected ApplicationHelper _appService;
        #endregion

        #region Properties

        protected char SeparatorPk = '-';

        private string _empresa;
        public string Empresa
        {
            get { return _empresa; }
            set
            {
                _empresa = value;
                _converterModel.Empresa = value;
            }
        }

        public Guid Usuarioid { get; set; }


        public List<string> WarningList
        {
            get { return _validationService.WarningList; }
        }

        #endregion

        #region CTR

        protected GestionService(IContextService context, MarfilEntities db = null)
        {
            _context = context;
            _empresa = _context.Empresa;
            Usuarioid = _context.Id;
            _db = db ?? MarfilEntities.ConnectToSqlServer(_context.BaseDatos);
            _appService = new ApplicationHelper(context);
            _converterModel = FConverterModel.Instance.CreateConverterModelService<TView, TPersistencia>(context, _db, Empresa);
            _validationService = FValidationService.Instance.CreateConverterModelService<TPersistencia>(context, _db);
        }

        #endregion

        #region Select busqueda principal

        public virtual IEnumerable<T> GetAll<T>() where T : IModelView
        {
            return _db.Database.SqlQuery<T>(GetSelectPrincipal()).ToList();
        }

        public virtual string GetSelectPrincipal()
        {
            var sb = new StringBuilder();

            var tienempresa = typeof(TPersistencia).GetProperty("empresa") != null;
            sb.AppendFormat("select * from {0} {1}", typeof(TPersistencia).Name,
                tienempresa ? string.Format(" where empresa='{0}'", Empresa) : string.Empty);

            return sb.ToString();
        }



        #endregion

        public IEnumerable<IModelView> getAll()
        {
            return _converterModel.GetAll();
        }

        public virtual IModelView get(string id)
        {
            return _converterModel.CreateView(id);
        }

        public virtual bool exists(string id)
        {
            return _converterModel.Exists(id);
        }

        protected Expression<Func<T, bool>> CreateWhereClause<T>(
    string propertyName, object propertyValue)
        {
            var parameter = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(parameter, propertyName),
                    Expression.Constant(propertyValue)),
                parameter);
        }

        private Expression<Func<T, bool>> CreateWhereClauseGreater<T>(
   string propertyName, object propertyValue)
        {
            var parameter = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(
                    Expression.Property(parameter, propertyName),
                    Expression.Constant(propertyValue)),
                parameter);
        }

        public virtual string FirstRegister()
        {
            var obj = GetprimarykeyColumns().All(f => f != "empresa") ? _db.Set<TPersistencia>().FirstOrDefault() : _db.Set<TPersistencia>().FirstOrDefault(CreateWhereClause<TPersistencia>("empresa", Empresa));
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();

        }

        public virtual string LastRegister()
        {

            var keyNames = GetprimarykeyColumns();
            var enumerable = keyNames as string[] ?? keyNames.ToArray();
            var query = enumerable.All(f => f != "empresa") ? _db.Set<TPersistencia>() : _db.Set<TPersistencia>().Where(CreateWhereClause<TPersistencia>("empresa", Empresa));

            var flagFirst = true;
            IOrderedQueryable<TPersistencia> orderedQuery = null;
            foreach (var item in enumerable)
            {

                orderedQuery = flagFirst ? query.OrderByDescending(item) : orderedQuery.ThenByDescending(item);
                flagFirst = false;

            }
            var obj = orderedQuery.FirstOrDefault();
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        public virtual string NextRegister(string id)
        {
            return GetRegister(id, TipoSelect.Next);
        }

        public virtual string PreviousRegister(string id)
        {
            return GetRegister(id, TipoSelect.Previous);
        }

        protected virtual string GetRegister(string id, TipoSelect tipo)
        {
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                var keyNames = GetprimarykeyColumns().ToArray();
                using (var cmd = new SqlCommand(GetSelect(keyNames, tipo), con))
                {
                    if (keyNames.Contains("empresa"))
                    {
                        cmd.Parameters.AddWithValue("empresa", Empresa);
                    }
                    var pkColumns = keyNames.Count(c => c != "empresa");
                    var pkvector = pkColumns > 1 ? id.Split(SeparatorPk) : new[] { id };
                    var j = 0;
                    foreach (var item in keyNames.Where(item => item != "empresa"))
                    {
                        cmd.Parameters.AddWithValue(item, pkvector[j++]);
                    }
                    var tabla = new DataTable();
                    using (var ad = new SqlDataAdapter(cmd))
                    {

                        ad.Fill(tabla);
                        if (tabla.Rows.Count > 0)
                        {
                            var vector = GetKeys(keyNames, tabla.Rows[0]);
                            var obj = _db.Set<TPersistencia>().Find(vector);
                            return _converterModel.GetModelView(obj).GetPrimaryKey();
                        }
                    }
                }

            }

            return string.Empty;
        }

        protected enum TipoSelect
        {
            Next,
            Previous
        }

        protected virtual string GetSelect(string[] keyNames, TipoSelect tipo)
        {

            var sb = new StringBuilder();

            sb.AppendFormat("select top 1 * from {0} Where ", typeof(TPersistencia).Name);
            var flagFirst = true;
            for (var i = 0; i < keyNames.Count(); i++)
            {
                if (!flagFirst)
                    sb.Append(" AND ");

                if (i == keyNames.Count() - 1)
                    sb.AppendFormat("{0}{1}@{0}", keyNames[i], tipo == TipoSelect.Next ? ">" : "<");
                else
                    sb.AppendFormat("{0}=@{0}", keyNames[i]);

                flagFirst = false;
            }

            if (tipo == TipoSelect.Previous)
                sb.AppendFormat(" order by {0} desc", string.Join(",", keyNames));

            return sb.ToString();

        }

        protected object[] GetKeys(IEnumerable<string> keyNames, DataRow dataRow)
        {

            return keyNames.Select(item => dataRow[item]).ToArray();
        }

        public virtual void create(IModelView obj)
        {
            var newItem = _converterModel.CreatePersitance(obj);
            if (_validationService.ValidarGrabar(newItem))
            {
                _db.Set<TPersistencia>().Add(newItem);
                try
                {
                    _db.SaveChanges();
                    _converterModel.AsignaId(newItem, ref obj);
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null
                        && ex.InnerException.InnerException != null)
                    {
                        var inner = ex.InnerException.InnerException as SqlException;
                        if (inner != null)
                        {
                            if (inner.Number == 2627 || inner.Number == 2601)
                            {
                                throw new ValidationException(General.ErrorRegistroExistente);
                            }
                        }
                    }


                    throw ex;
                }

            }

        }

        public virtual void edit(IModelView obj)
        {
            var objPersistance = _converterModel.EditPersitance(obj);
            if (_validationService.ValidarGrabar(objPersistance))
            {
                _db.Entry(objPersistance).State = EntityState.Modified;
                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null
                        && ex.InnerException.InnerException != null)
                    {
                        var inner = ex.InnerException.InnerException as SqlException;
                        if (inner != null)
                        {
                            if (inner.Number == 2627 || inner.Number == 2601)
                            {
                                throw new ValidationException(General.ErrorRegistroExistente);
                            }
                        }
                    }

                    throw;
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
        }

        public virtual void delete(IModelView obj)
        {
            var objPersistance = _converterModel.EditPersitance(obj);
            if (_validationService.ValidarBorrar(objPersistance))
            {
                _db.Set<TPersistencia>().Remove(objPersistance);
                _db.SaveChanges();
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        public virtual ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var fmodel = new FModel();
            var obj = fmodel.GetModel<TView>(_context);
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;

            var a = new ListIndexModel();
            a.Entidad = display.DisplayName;
            a.List = GetAll<TView>();
            a.PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList();
            a.VarSessionName = "__" + t.Name;
            a.Properties = instance.getProperties();
            a.Controller = controller;
            a.PermiteEliminar = canEliminar;
            a.PermiteModificar = canModificar;
            a.ExcludedColumns = new[] { "Toolbar" } ;

            //var a = new ListIndexModel()
            //{
            //    Entidad = display.DisplayName,
            //    List = GetAll<TView>(),
            //    PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
            //    VarSessionName = "__" + t.Name,
            //    Properties = instance.getProperties(),
            //    Controller = controller,
            //    PermiteEliminar = canEliminar,
            //    PermiteModificar = canModificar,
            //    ExcludedColumns = new[] { "Toolbar" }
            //};

            return a;
        }

        protected IEnumerable<string> GetprimarykeyColumns()
        {
            var adapter = (IObjectContextAdapter)_db;
            var objectContext = adapter.ObjectContext;
            var objectSet = objectContext.CreateObjectSet<TPersistencia>();
            var entitySet = objectSet.EntitySet;
            return entitySet.ElementType.KeyMembers.ToList().Select(f => f.Name);
        }

        public EmpresaModel get(object empresa)
        {
            throw new NotImplementedException();
        }
    }
}
