using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using System.Transactions;
using Marfil.Inf.Genericos;
using System.Data.Entity;
using Marfil.Dom.Persistencia.Model;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RCuenta = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using System.Data;
using Marfil.Dom.ControlsUI.NifCif;
using System.Data.Common;
using System.Dynamic;
using System.Data.Entity.Migrations;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    
    public static class dbExtension
    {


        public static IEnumerable<dynamic> CollectionFromSql(this DbContext dbContext, string Sql, Dictionary<string, object> Parameters)
        {
            //using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            using (var cmd = dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                foreach (KeyValuePair<string, object> param in Parameters)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value;
                    cmd.Parameters.Add(dbParameter);
                }

                //var retObject = new List<dynamic>();
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dataRow = GetDataRow(dataReader);
                        yield return dataRow;
                    }
                }
            }
        }


        private static dynamic GetDataRow(DbDataReader dataReader)
        {
            var dataRow = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            return dataRow;
        }

    }


    public enum OperacionCuenta
    {
        Any,
        Crear,
        Editar
    }
    public interface ICuentasService
    {

    }

    public class CuentasService : GestionService<CuentasModel, Cuentas>, ICuentasService
    {

        public const string SelectCuentasTerceros =
            "select c.id as [Fkcuentas],c.descripcion as [Descripcion], c.descripcion2 as [Razonsocial], c.nif as [Nif], p.descripcion as [Pais], pr.nombre as [Provincia],d.poblacion as [Poblacion],c.bloqueada as [Bloqueado] from {2} as cli " +
            " inner join cuentas as c on c.id=cli.fkcuentas and c.empresa=cli.empresa " +
            " left join direcciones as d on d.empresa= c.empresa and d.tipotercero={0} and d.fkentidad=c.id and d.defecto=1 " +
            " left join paises as p on p.valor=d.fkpais " +
            " left join provincias as pr on pr.codigopais= d.fkpais  and pr.id =d.fkprovincia " +
            " where cli.empresa='{1}' ";

        public const string SelectCuentasTercerosPaginado =
            "select c.id as [Fkcuentas],c.descripcion as [Descripcion], c.descripcion as [Razonsocial], c.nif as [Nif], p.descripcion as [Pais], pr.nombre as [Provincia],d.poblacion as [Poblacion],c.bloqueada as [Bloqueado] from {2} as cli " +
            " inner join cuentas as c on c.id=cli.fkcuentas and c.empresa=cli.empresa " +
            " left join direcciones as d on d.empresa= c.empresa and d.tipotercero={0} and d.fkentidad=c.id and d.defecto=1 " +
            " left join paises as p on p.valor=d.fkpais " +
            " left join provincias as pr on pr.codigopais= d.fkpais  and pr.id =d.fkprovincia " +
            " where cli.empresa='{1}' and isnull(c.bloqueada,0)=0 {5}" +
            " order by c.id offset {3}*({4}-1) rows fetch next {3} rows only option (recompile)";

        public const string SelectCuentasTercerosTotalesPaginado =
           "select count(*) from {2} as cli " +
           " inner join cuentas as c on c.id=cli.fkcuentas and c.empresa=cli.empresa " +
           " left join direcciones as d on d.empresa= c.empresa and d.tipotercero={0} and d.fkentidad=c.id and d.defecto=1 " +
           " left join paises as p on p.valor=d.fkpais " +
           " left join provincias as pr on pr.codigopais= d.fkpais  and pr.id =d.fkprovincia " +
           " where cli.empresa='{1}' and isnull(c.bloqueada,0)=0 {5}";

        public const string SelectTodasLasCuentasTerceros =
            "SELECT t.*, c.descripcion"
            + " FROM("
            + " SELECT empresa, fkcuentas"
            + " FROM Clientes"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Proveedores"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Acreedores"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Cuentastesoreria"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Transportistas"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Agentes"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Aseguradoras"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Operarios"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Comerciales"
            + " UNION"
            + " SELECT empresa, fkcuentas"
            + " FROM Prospectos"
            + " )t"
            + " LEFT JOIN Cuentas AS c ON c.empresa = t.empresa AND c.id = t.fkcuentas"
            + " WHERE t.empresa={0}";
    
        public bool FlagDeleteFromThird { get; set; }

        #region CTR
        

        public CuentasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            
        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            try
            {
                using (TransactionScope tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
                {
                    var cuenta = obj as CuentasModel;
                    cuenta.Fechaalta = DateTime.Now;
                    base.create(cuenta);
                    obj = get(cuenta.Id);
                    CrearCuentasAsociadas(obj as CuentasModel);
                    tran.Complete();
                }
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

        }


        private void CrearCuentasAsociadas(CuentasModel model)
        {
            if (model.Nivel == 0 && model.Tiposcuentas.HasValue)
            {
                

                var codigocuentacliente = model.Id.Substring(_appService.NivelesCuentas(Empresa,_db));
                var result = _db.Tiposcuentas.SingleOrDefault(f => f.empresa == model.Empresa && f.tipos == (int)model.Tiposcuentas);
                if (result != null)
                {
                    if (result.TiposcuentasLin.Any())
                    {
                        foreach (var item in result.TiposcuentasLin)
                        {
                            var nuevaCuentaAsociadaId = string.Format("{0}{1}", item.cuenta, codigocuentacliente);

                            if (_db.Cuentas.Any(f => f.id == nuevaCuentaAsociadaId && f.nif!=model.Nif.Nif && f.empresa == Empresa))
                            {
                                throw new ValidationException(string.Format(RCuenta.ErrorCuentaExistente, nuevaCuentaAsociadaId));
                            }
                            else if (!_db.Cuentas.Any(f => f.id == nuevaCuentaAsociadaId && f.empresa==Empresa))
                            {
                                var nuevaCuenta = _db.Cuentas.Create();
                                nuevaCuenta.empresa = model.Empresa;
                                nuevaCuenta.descripcion = item.descripcion + " " + model.Descripcion;
                                nuevaCuenta.descripcion2 = item.descripcion + " " + model.Descripcion2;
                                nuevaCuenta.fechamodificacion = DateTime.Now;
                                nuevaCuenta.fkUsuarios = new Guid(model.UsuarioId);
                                nuevaCuenta.fkPais = model.FkPais;
                                nuevaCuenta.id = nuevaCuentaAsociadaId;
                                nuevaCuenta.nif = model.Nif.Nif;
                                nuevaCuenta.fktipoidentificacion_nif = model.Nif.TipoNif;
                                if (_validationService.ValidarGrabar(nuevaCuenta))
                                    _db.Cuentas.Add(nuevaCuenta);
                            }

                        }
                        _db.SaveChanges();
                    }


                }

            }
        }

        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var objPersistance = _converterModel.EditPersitance(obj);
                ((CuentasValidation)_validationService).FlagDeleteFromThird = FlagDeleteFromThird;
                
                if (_validationService.ValidarBorrar(objPersistance))
                {
                    BorrarCuentasAsociadas(obj as CuentasModel);
                    _db.Set<Cuentas>().Remove(objPersistance);
                    _db.SaveChanges();
                }
                tran.Complete();
            }
                

        }

        private void BorrarCuentasAsociadas(CuentasModel model)
        {
            if (model.Nivel == 0 && model.Tiposcuentas.HasValue)
            {


                var codigocuentacliente = model.Id.Substring(_appService.NivelesCuentas(Empresa, _db));
                var result = _db.Tiposcuentas.Include("TiposcuentasLin").SingleOrDefault(f => f.empresa == model.Empresa && f.tipos == (int)model.Tiposcuentas);
                if (result != null)
                {
                    if (result.TiposcuentasLin.Any())
                    {
                        foreach (var item in result.TiposcuentasLin)
                        {
                            var nuevaCuentaAsociadaId = string.Format("{0}{1}", item.cuenta, codigocuentacliente);
                            
                            if (_db.Cuentas.Any(f => f.id == nuevaCuentaAsociadaId && f.empresa==Empresa))
                            {
                                _db.Cuentas.Remove(_db.Cuentas.Single(f => f.id == nuevaCuentaAsociadaId && f.empresa == Empresa));
                            }

                        }


                    }


                }

            }
        }

        #endregion

        #region Api

        public IEnumerable<CuentasBusqueda> GetCuentas(ICuentasFiltros filtros,out int totales)
        {
            var tipo = (TiposCuentas)(Funciones.Qint(filtros.Tipocuenta) ?? 0);
            totales = _db.Database.SqlQuery<int>(string.Format(SelectCuentasTercerosTotalesPaginado, (int) tipo, Empresa, tipo, filtros.RegistrosPagina,filtros.Pagina, CrearCadenaFiltros(filtros.Filtros))).Single();
            return _db.Database.SqlQuery<CuentasBusqueda>(string.Format(SelectCuentasTercerosPaginado,(int)tipo, Empresa, tipo,filtros.RegistrosPagina,filtros.Pagina, CrearCadenaFiltros(filtros.Filtros))).ToList();
        }

        private string CrearCadenaFiltros(string filtros)
        {
            if (string.IsNullOrEmpty(filtros))
            {
                return string.Empty;
            }

            var sb=new StringBuilder();
            sb.AppendFormat(" AND (c.id like '%{0}%' OR c.descripcion like '%{0}%' OR c.descripcion2 like '%{0}%' OR c.nif like  '%{0}%' OR p.descripcion like '%{0}%' OR pr.nombre like '%{0}%' OR d.poblacion like '%{0}%')", filtros);
            return sb.ToString();
        }

        public IEnumerable<CuentasBusqueda> GetCuentas(TiposCuentas tipo)
        {
            if (tipo == TiposCuentas.Todas)
                return _db.Database.SqlQuery<CuentasBusqueda>(string.Format(SelectTodasLasCuentasTerceros, _context.Empresa)).ToList();
            else
                return _db.Database.SqlQuery<CuentasBusqueda>(string.Format(SelectCuentasTerceros, (int)tipo, Empresa, tipo)).ToList();
        }

        public static ListIndexModel GetTercerosIndexModel<T>(IContextService context,string controller,bool canEliminar,bool canModificar)
        {
            var instance = new CuentasBusqueda();
            var ctor = typeof(T).GetConstructor(new[] { typeof(IContextService) });
            var agentesObj = ctor.Invoke(new object[] { context }) as ICanDisplayName;
           

            var model = new ListIndexModel()
            {
                Entidad = agentesObj.DisplayName,
                
                PrimaryColumnns = new[] { "Fkcuentas" },
                VarSessionName = "__" + typeof(T).Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }



            };
            var propiedadesVisibles = new[] { "Fkcuentas", "Descripcion", "Razonsocial", "Nif", "Pais", "Provincia", "Poblacion" };
            var propiedades = Helpers.Helper.getProperties<CuentasBusqueda>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            model.BloqueoColumn = "Bloqueado";

            return model;
        }





        public IEnumerable<CuentasModel> GetCuentasContablesNivel(int nivel)
        {
            return _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == nivel && (!f.categoria.HasValue || (f.categoria.HasValue && f.categoria == (int)CategoriasCuentas.Contables))).ToList().Select(f => new CuentasModel() { Id = f.id, Descripcion = f.descripcion, Nivel = f.nivel ?? 0, BloqueoModel = new BloqueoEntidadModel() { Bloqueada = f.bloqueada ?? false, Fecha = f.fechamodificacionbloqueo ?? DateTime.MinValue, FkMotivobloqueo = f.fkMotivosbloqueo } });
        }


        public IEnumerable<CuentasModel> GetCuentasContables()
        {
            return _db.Cuentas.Where(f => f.empresa == Empresa && (!f.categoria.HasValue || (f.categoria.HasValue && f.categoria == (int)CategoriasCuentas.Contables))).ToList().Select(f => new CuentasModel() { Id=f.id,Descripcion = f.descripcion,Nivel = f.nivel??0,BloqueoModel = new BloqueoEntidadModel() {Bloqueada = f.bloqueada??false,Fecha=f.fechamodificacionbloqueo??DateTime.MinValue,FkMotivobloqueo = f.fkMotivosbloqueo} });
        }

        public IEnumerable<CuentasModel> GetCuentasTercerosArticulos()
        {
            return _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel==0 && (f.id.StartsWith("4300") || f.id.StartsWith("4000") || f.id.StartsWith("4100"))).Select(f=> new CuentasModel() { Id = f.id, Descripcion = f.descripcion, BloqueoModel = new BloqueoEntidadModel() { Bloqueada = f.bloqueada ?? false, Fecha = f.fechamodificacionbloqueo ?? DateTime.MinValue, FkMotivobloqueo = f.fkMotivosbloqueo } });
        }

        public IEnumerable<CuentasBusqueda> GetCuentasAsistenteAsignacionCobrosPagos(string tipoasignacion)
        {
            var result = new List<CuentasBusqueda>();
            var service = FService.Instance.GetService(typeof(CuentasModel), _context, _db) as CuentasService;

            if (tipoasignacion=="0")
            {
                result.AddRange(service.GetCuentas(TiposCuentas.Clientes).Where(f => !(f.Bloqueado ?? false)));
            }

            if (tipoasignacion=="1")
            {
                result.AddRange(service.GetCuentas(TiposCuentas.Proveedores).Where(f => !(f.Bloqueado ?? false)));
                result.AddRange(service.GetCuentas(TiposCuentas.Acreedores).Where(f => !(f.Bloqueado ?? false)));
            }

            return result;
        }

        public IEnumerable<CuentasModel> GetCuentasNivel0()
        {
            return _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0).Select(f => new CuentasModel() { Id = f.id, Descripcion = f.descripcion, BloqueoModel = new BloqueoEntidadModel() { Bloqueada = f.bloqueada ?? false, Fecha = f.fechamodificacionbloqueo ?? DateTime.MinValue, FkMotivobloqueo = f.fkMotivosbloqueo } });
        }

        public void ActualizarCuentas(string empresa)
        {
            var listcuentasclientes = _db.Cuentas.Where(f => f.nivel == 0 && f.empresa == Empresa).ToList().Select(_converterModel.GetModelView);
            foreach (var item in listcuentasclientes)
                edit(item);
        }

        public IEnumerable<CuentasModel> GetCuentasClientes()
        {
            return _db.Cuentas.Where(f => f.nivel == 0 && f.empresa == Empresa).ToList().Select(f => new CuentasModel() { Id = f.id, Descripcion2 = f.descripcion2, Descripcion = f.descripcion, Empresa = f.empresa, Nivel = f.nivel.Value });
        }

        public IEnumerable<CuentasModel> GetCuentasClientes(TiposCuentas tipo, bool primeracarga = false)
        {
            var fkcuentatercero = _db.Tiposcuentas.Single(f => f.empresa == Empresa && f.tipos == (int)tipo)?.cuenta;
            return _db.Cuentas.Where(f => f.nivel == 0 && f.empresa == Empresa && (primeracarga || f.bloqueada.HasValue == false || f.bloqueada.Value == false) && f.id.StartsWith(fkcuentatercero)).Select(f => new CuentasModel() { Id = f.id, Descripcion2 = f.descripcion2, Descripcion = f.descripcion, Empresa = f.empresa, Nivel = f.nivel.Value, Tiposcuentas = f.tipocuenta });
        }

        public string GetSuperCuenta(string cuentanivelcero)
        {
            var totalNivelesSupercuentas = _appService.NivelesCuentas(Empresa);
            return cuentanivelcero.Substring(0, totalNivelesSupercuentas);
        }

        public string GetDescripcionCuenta(string idcuenta)
        {
            var descripcionCuenta = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == idcuenta).Select(f => f.descripcion).SingleOrDefault();

            return descripcionCuenta;
        }

        public IEnumerable<CuentasModel> GetSuperCuentas()
        {
            var appService=new ApplicationHelper(_context);
            var nivelesCuentas = appService.NivelesCuentas(Empresa);
            return _db.Cuentas.Where(f => f.nivel == nivelesCuentas && f.empresa == Empresa).Select(f => new CuentasModel() { Id = f.id, Descripcion2 = f.descripcion2, Descripcion = f.descripcion, Empresa = f.empresa, Nivel = f.nivel.Value });
        }

        public IEnumerable<CuentasModel> GetSuperCuentas(string id)
        {
            var appService = new ApplicationHelper(_context);
            var list = new List<CuentasModel>();
            if (id.Length > 1)
            {
                var totalNivelesSupercuentas = appService.NivelesCuentas(Empresa);
                var totalLongitud = appService.DigitosCuentas(Empresa);

                var currentLevel = id.Length > totalNivelesSupercuentas ? totalNivelesSupercuentas + 1 : id.Length;
                for (var i = currentLevel; i > 1; i--, currentLevel--)
                {
                    var step = currentLevel > totalNivelesSupercuentas ? (totalLongitud - totalNivelesSupercuentas) : 1;//la logitud del identificador del nivel
                    var subCuenta = id.Substring(0, id.Length - step);
                    id = subCuenta;
                    var item = _db.Cuentas.SingleOrDefault(f => f.id == subCuenta && f.empresa == Empresa);
                    if (item != null)
                    {
                        list.Add(new CuentasModel() { Id = item.id, Descripcion2 = item.descripcion2, Descripcion = item.descripcion, Empresa = item.empresa, Nivel = item.nivel.Value });
                    }
                    else
                    {
                        list.Add(new CuentasModel() { Id = subCuenta, Descripcion2 = "", Descripcion = "", Empresa = Empresa, Nivel = i });
                    }
                }

            }

            return list;
        }

        public void CrearEditarCuenta(CuentasModel cuenta, IEnumerable<CuentasModel> supercuentas, OperacionCuenta operacion = OperacionCuenta.Any)
        {
            using (TransactionScope tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                foreach (var cuentasModel in supercuentas.OrderBy(f => f.Nivel))
                {
                    if (_db.Cuentas.Any(f => f.empresa == cuentasModel.Empresa && f.id == cuentasModel.Id))
                        edit(cuentasModel);
                    else
                        create(cuentasModel);
                }
                if (operacion == OperacionCuenta.Any)
                {
                    if (_db.Cuentas.Any(f => f.empresa == cuenta.Empresa && f.id == cuenta.Id))
                        edit(cuenta);
                    else
                        create(cuenta);
                }
                else if (operacion == OperacionCuenta.Crear)
                {
                    create(cuenta);
                }
                else
                {
                    edit(cuenta);
                }

                tran.Complete();
            }
        }

        public IEnumerable<CuentasModel> GetCuentasModel(string empresa, int nivel)
        {
            using (var cuentasService = FService.Instance.GetService(typeof(CuentasModel), _context))
            {
                return cuentasService.getAll().Where(f => ((CuentasModel)f).Nivel == nivel).Select(f => f as CuentasModel);
            }
        }

        public string GetNextCuenta(string empresa, string subcuenta)
        {
            var appService = new ApplicationHelper(_context);
            string result = string.Empty;
            var cuenta = _db.Cuentas.Where(f => f.id.StartsWith(subcuenta) && f.empresa == empresa).Max(f => f.id);
            if (cuenta != null)
            {
                var length = appService.DigitosCuentas(empresa);
                var id = string.IsNullOrEmpty(cuenta.Replace(subcuenta, "")) ? "0" : cuenta.Replace(subcuenta, "");
                var current = int.Parse(id);
                current++;
                result = subcuenta + Tools.RellenaCeros(current.ToString(), length - subcuenta.Length);

            }
            else
            {
                var length = appService.DigitosCuentas(empresa);
                result = subcuenta + Funciones.RellenaCod("1", length - subcuenta.Length);
            }

            return result;
        }

        public void Bloquear(string id, string motivo, string user, bool bloqueado)
        {
            var cuenta = _db.Cuentas.Single(f => f.empresa == Empresa && f.id == id);
            cuenta.fkMotivosbloqueo = motivo;
            cuenta.fkUsuariobloqueo = new Guid(user);
            cuenta.fechamodificacionbloqueo = DateTime.Now;
            cuenta.bloqueada = bloqueado;
            _db.Entry(cuenta).State = EntityState.Modified;
            _db.SaveChanges();
        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            
            var ctor = t.GetConstructor(new[] { typeof(IContextService) });
            var obj =ctor.Invoke(new object[] { _context });
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;
            return new ListIndexModel()
            {
                Entidad = display.DisplayName,
                List = GetCuentasContables(),
                PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar", "Fechaalta", "Bloqueado", "BloqueoModel", "Tiposcuentas", "Nifrequired", "Empresa", "Descripcion2", "FkPais", "Nif", "Contrapartida", "ContrapartidaDescripcion", "Bloqueada", "FkMotivoBloqueo", "MotivoBloqueo", "FechaModificacion", "Usuario", "UsuarioId", "DescripcionLarga", "FechaModificacionBloqueo", "UsuarioIdBloqueo", "UsuarioBloqueo", "DescripcionBloqueo" },
                FiltroColumnas = new Dictionary<string, FiltroColumnas>() { { "Id", FiltroColumnas.EmpiezaPor } },
                BloqueoColumn = "Bloqueado"
            };
        }

        #endregion


        #region Importar

        public void Importar(DataTable dt, int idPeticion, IContextService context)
        {
            string errores = "";
            string tipoIso = dt.Columns[5].ColumnName.ToString(); ;
            CuentasModel cuenta = new FModel().GetModel<CuentasModel>(context);                       

            foreach (DataRow row in dt.Rows)
            {
                // Existe la cuenta?
                var id = row["Cuenta"].ToString();
                var existecuenta = _db.Cuentas.Where(f => f.empresa == cuenta.Empresa && f.id == id).SingleOrDefault();

                if (existecuenta == null)
                {
                    cuenta.Id = id;
                    cuenta.UsuarioId = "00000000-0000-0000-0000-000000000000";
                    cuenta.Descripcion = row["Descripcion"].ToString();
                    cuenta.Descripcion2 = row["Razonsocial"].ToString();

                    // 1    nivel = 1
                    // 10   nivel = 2
                    // 101  nivel = 3
                    // 1010 nivel = 4                        
                    cuenta.Nivel = id.Length;

                    if (cuenta.Nivel > 4)
                    {
                        // 40000001 nivel = 0
                        cuenta.Nivel = 0;

                        // Nif
                        if (string.IsNullOrEmpty(row["Nif"].ToString()))
                        {
                            cuenta.Nif.Nif = null;
                            cuenta.Tiposcuentas = null;
                        }
                        else
                        {
                            cuenta.Nif.Nif = row["Nif"].ToString();
                            // Nifs mal introducidos?  
                            if (cuenta.Nif.Nif.Length > 15)
                            {
                                cuenta.Nif.Nif = row["Nif"].ToString().Substring(0, 14);
                            }
                            var digitoscuenta = cuenta.Id.Substring(0, 4);
                            // Revisar FirsOrDefault
                            // Exite un tipo de cuenta asociado?                                
                            var existeTipoCuenta = _db.Tiposcuentas.Where(f => f.empresa == cuenta.Empresa && f.cuenta == digitoscuenta).FirstOrDefault();
                            if (existeTipoCuenta != null)
                            {
                                var tipo = _db.Tiposcuentas.Where(f => f.empresa == cuenta.Empresa && f.cuenta == digitoscuenta).Select(f => f.tipos).FirstOrDefault();
                                cuenta.Tiposcuentas = tipo;
                            }
                            else
                            {
                                // Cuenta de tipo no contable pero sin un tipo de cuenta asociado                              
                                cuenta.Tiposcuentas = null;
                            }

                            var obligatorio = _db.Tiposcuentas.Where(f => f.empresa == cuenta.Empresa && f.cuenta == digitoscuenta).Select(f => f.nifobligatorio).FirstOrDefault();

                            if (obligatorio != null)
                            {
                                cuenta.Nif.Obligatorio = true;
                                //cuenta.Nif.Obligatorio = (bool)obligatorio;  
                                // TipoNif es una tabla varia, Valor NIF/CIF = 001                          
                                if (string.IsNullOrEmpty(row["TipoNif"].ToString()))
                                {
                                    cuenta.Nif.TipoNif = "001";
                                }
                                else
                                {
                                    cuenta.Nif.TipoNif = row["TipoNif"].ToString().PadLeft(3, '0');
                                }

                            }
                            else
                            {
                                cuenta.Nif.Obligatorio = false;
                            }
                        }
                    }


                    // País
                    string query = "SELECT Valor FROM Paises WHERE " + tipoIso + " = '" + row[5].ToString() + "'";

                    try
                    {
                        using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
                        {
                            con.Open();
                            using (SqlCommand command = new SqlCommand(query, con))
                            {
                                cuenta.FkPais = command.ExecuteScalar()?.ToString();
                            }
                            con.Close();
                        }
                    }
                    finally
                    {

                    }                   

                    try
                    {
                        create(cuenta);
                    }
                    catch (Exception ex)
                    {                                                                                               
                        errores += row[0] + ";" + row[1] + ";" + row[2] + ";" + row[3] + ";" + row[4] + ";" + row[5] + ";" + ex.Message + Environment.NewLine;
                    }

                }
            }

            var item = _db.PeticionesAsincronas.Where(f => f.empresa == context.Empresa && f.id == idPeticion).SingleOrDefault();

            item.estado = (int)EstadoPeticion.Finalizada;
            item.resultado = errores;

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            //throw new ValidationException(errores);            
        }

        public int CrearPeticionImportacion(IContextService context)
        {
            var item = _db.PeticionesAsincronas.Create();

            item.empresa = context.Empresa;
            item.id = _db.PeticionesAsincronas.Any() ? _db.PeticionesAsincronas.Max(f => f.id) + 1 : 1;
            item.usuario = context.Usuario;
            item.fecha = DateTime.Today;
            item.estado = (int)EstadoPeticion.EnCurso;
            item.tipo = (int)TipoPeticion.Importacion;
            item.configuracion = (((int)TipoImportacion.ImportarCuentas).ToString() + "-").ToString();

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            return item.id;
        }

        #endregion


        #region Exportar

        public void Exportar()
        {
            var sb = new StringBuilder();            
            string query =
                "SELECT c.id AS[Cuenta], '' AS[Desglose], c.descripcion AS[Descripcion], c.descripcion2 AS[RazonSocial]," +
                " d.direccion AS[Direccion], d.poblacion AS[Poblacion], d.cp AS[CodPostal]," +
                " p.nombre AS[Provincia], d.telefono AS[Telefono], d.fax AS[Fax]," +
                " c.nif AS[Nif], '' AS[TipoOperacion], '' AS[IRPF], b.iban AS[CodBancario]," +
                " c.fkPais AS [CodPais], b.iban AS[Iban], b.bic AS[Bic], '' AS[FormaDePago], d.email AS[Email]" +
                " FROM Cuentas as c" +
                " LEFT JOIN Direcciones AS d ON c.empresa = d.empresa AND c.id = d.fkentidad" +
                " LEFT JOIN BancosMandatos AS b ON c.empresa = b.empresa AND c.id = b.fkcuentas" +
                " LEFT JOIN Provincias AS p ON c.fkPais = p.codigopais AND d.fkprovincia = p.id" +
                " LEFT JOIN Prospectos AS prosp ON c.empresa = prosp.empresa AND c.id = prosp.fkcuentas" +
                " WHERE c.nivel = 0 AND prosp.fkcuentas IS NULL";
                //" WHERE d.defecto = 1 AND b.defecto = 1";

            List<dynamic> lista = _db.CollectionFromSql(query,
                new Dictionary<string, object> { }).ToList();

            // País
            string queryPaises = "SELECT Valor AS [Valor], CodigoIsoAlfa2 AS [ISO2] FROM Paises";

            List<dynamic> listaPaises = _db.CollectionFromSql(queryPaises,
                new Dictionary<string, object> { }).ToList();

            foreach (var item in lista)
            {
                string s = "";
                // Cuenta
                s += item.Cuenta.PadRight(12, ' ');

                // Desglose (1 = Cuenta de Mayor, 0 = Cuenta de Diario)                
                //s += item.Desglose.PadRight(1, ' ');
                s += "0";

                // Descripción                
                s += fixedLength(item.Descripcion.ToString(), 24);                

                // Razón social                
                s += fixedLength(item.RazonSocial.ToString(), 40);

                // Dirección                
                s += fixedLength(item.Direccion.ToString().Replace("\r\n", " ").Replace("\n", " "), 30);

                // Población                
                s += fixedLength(item.Poblacion.ToString(), 30);

                // Código postal                
                s += fixedLength(item.CodPostal.ToString(), 5);

                // Provincia
                s += (item.Provincia.ToString() ?? string.Empty).PadRight(20, ' ');

                // Teléfono
                s += (item.Telefono.ToString() ?? string.Empty).PadRight(20, ' ');

                // Fax   
                s += (item.Fax.ToString() ?? string.Empty).PadRight(15, ' ');

                // Nif
                s += (item.Nif.ToString() ?? string.Empty).PadRight(15, ' ');

                // Tipo de operación
                s += item.TipoOperacion.PadRight(1, ' ');

                // Retención IRPF
                s += item.IRPF.PadLeft(2, '0');

                // Código bancario iban - 4 primeros                
                if (!string.IsNullOrEmpty(item.Iban.ToString()))
                {
                    s += item.Iban.ToString().Substring(4, 20).PadRight(20, ' ');
                }
                else
                {
                    s += string.Empty.PadRight(20, ' ');
                }                           

                // País ISO2                            
                if(!string.IsNullOrEmpty(item.CodPais.ToString()))
                {
                    s += listaPaises.Where(f => f.Valor == item.CodPais).Select(f => f.ISO2).SingleOrDefault().ToString();
                }
                else
                {
                    s += string.Empty.PadRight(2, ' ');
                }              

                // Iban
                s += (item.Iban.ToString() ?? string.Empty).PadRight(34, ' ');

                // Bic
                //s += (item.Bic.ToString() ?? string.Empty).PadRight(11, ' '); en bd tamaño 15
                s += fixedLength(item.Bic.ToString(), 11);

                // Forma de pago
                s += (item.FormaDePago.ToString() ?? string.Empty).PadLeft(3, '0');

                // Email                       
                s += fixedLength(item.Email.ToString(), 50);

                sb.Append(s);
                sb.Append(Environment.NewLine);
            }
            //Get Current Response  
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            response.AddHeader("content-disposition", "attachment;filename=PLACO000.ASC");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }

        #endregion

        private string fixedLength(string input, int length)
        {
            if (input.Length > length)
                return input.Substring(0, length);
            else
                return input.PadRight(length, ' ');
        }

        public IEnumerable<CuentasRegularizacionModel> BuscarCuentasExistencias()
        {
            var ejericicio = int.Parse(_context.Ejercicio);
            var cuentaexistencias = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().cuentasexistencias;
            //las cuentas de existencia
            var listacuentas = _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0 && f.id.StartsWith(cuentaexistencias))
                                .Select(
                                        f => new CuentasRegularizacionModel()
                                        {
                                            Cuentaexistencias = f.id,
                                            Saldoexistenciasiniciales = 0,
                                            Cuentavariacion = "",
                                            Saldoexistenciasfinales = 0
                                        }).ToList();

            //obtenemos los saldos
            foreach (var item in listacuentas)
            {
                if (_db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejericicio && f.fkcuentas == item.Cuentaexistencias).FirstOrDefault() != null) {
                    var saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejericicio && f.fkcuentas == item.Cuentaexistencias).FirstOrDefault().saldo;
                    item.Saldoexistenciasiniciales = saldo;

                }
                else
                {
                    item.Saldoexistenciasiniciales = 0;
                }
                
            }


            return listacuentas;
        }

        public IEnumerable<CuentasRegularizacionGruposModel> BuscarCuentasGrupos6y7()
        {
            var ejericicio = int.Parse(_context.Ejercicio);
            var cuentaspyg = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().cuentasperdidas;
            var listacuentaspyg = cuentaspyg.Split(';');
            var pref1 = listacuentaspyg[0];
            var pref2 = listacuentaspyg[1];

            //las cuentas de grupos 6 y 7
            var listacuentas = _db.Cuentas.Where(f => f.empresa == Empresa && f.nivel == 0 && (f.id.StartsWith(pref1) || f.id.StartsWith(pref2)))
                                .Select(
                                        f => new CuentasRegularizacionGruposModel()
                                        {
                                            Cuentagrupos = f.id,
                                            SaldoDeudor = 0,
                                            SaldoAcreedor = 0
                                        }).ToList();

            //obtenemos los saldos
            foreach (var item in listacuentas)
            {
                if (_db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejericicio && f.fkcuentas == item.Cuentagrupos).FirstOrDefault() != null)
                {
                    var saldo = _db.Maes.Where(f => f.empresa == Empresa && f.fkejercicio == ejericicio && f.fkcuentas == item.Cuentagrupos).FirstOrDefault().saldo;

                    if (saldo > 0)
                    {
                        item.SaldoDeudor = saldo;
                    } 
                    else if (saldo < 0)
                    {
                        item.SaldoAcreedor = saldo * -1;//Valor absoluto
                    }
                    else
                    {
                        item.SaldoAcreedor = 0;
                        item.SaldoDeudor = 0;
                    }

                }

            }

            listacuentas.RemoveAll(y => y.SaldoAcreedor == 0 && y.SaldoDeudor == 0);

            return listacuentas;
        }
    }
}
