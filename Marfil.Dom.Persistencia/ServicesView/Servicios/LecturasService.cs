using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class LecturasService : GestionService<LecturasModel, Lecturas>
    {

        #region CTR

        public LecturasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Identificador", "Fecha", "Registros"};
            var propiedades = Helpers.Helper.getProperties<LecturasModel>();

            model.PrimaryColumnns = new[] { "Identificador", "Lote", "Usuario" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Identificador", 0);
            model.OrdenColumnas.Add("Fecha", 1);
            model.OrdenColumnas.Add("Registros", 2);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select identificador, fecha, count(*) as Registros from Lecturas where usuario = '{0}' and empresa = '{1}' and insertado = 0 group by identificador, fecha;", Usuarioid, Empresa);
        }

        #endregion

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as LecturasModel;
                var lista = _db.Lecturas.Where(f => f.identificador == model.Identificador && f.empresa == model.Empresa && f.usuario == model.Usuario).ToList();

                foreach (var item in lista)
                {
                    //var lectura = _converterModel.CreateView(item.identificador);

                    _db.Set<Lecturas>().Remove(item);
                }
                _db.SaveChanges();
                tran.Complete();
            }

        }

        #region Importar

        public void Importar(DataTable dt, int idPeticion, IContextService context)
        {
            string errores = "";
            List<LecturasModel> Lista = new List<LecturasModel>();


            foreach (DataRow row in dt.Rows)
            {
                LecturasModel lectura = new FModel().GetModel<LecturasModel>(context);

                var identificador = row["Identificador"].ToString();
                var hora = row["Hora"].ToString();
                var lote = row["Lote"].ToString();
                var cantidad = 0;
                if (row["Cantidad"].ToString() != "" && row["Cantidad"].ToString() != null)
                {
                    cantidad = int.Parse(row["Cantidad"].ToString());
                }
                else
                {
                    cantidad = 1;
                }
                
                var usuario = Usuarioid;

                var existe = _db.Lecturas.Where(f => f.identificador == identificador && f.usuario == usuario && f.lote == lote).SingleOrDefault();

                if (existe == null){

                    lectura.Identificador = identificador;
                    DateTime fecha;
                    if (DateTime.TryParseExact(row["Fecha"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                    {
                        lectura.Fecha = fecha;
                    }
                    lectura.Hora = hora;
                    lectura.Lote = lote;
                    lectura.Cantidad = cantidad;
                    lectura.Usuario = usuario;
                    lectura.Insertado = false;
                    lectura.Empresa = Empresa;

                    Lista.Add(lectura);
                }

            }

            foreach (var item in Lista)
            {
                try
                {
                    create(item);
                }
                catch (Exception ex)
                {
                    errores += item.Identificador + ";" + item.Lote + ";" + ex.Message + Environment.NewLine;
                }
            }

        }
        #endregion

        public IEnumerable<LecturasAsistenteModel> BuscarLecturas()
        {
            //lecturas
            var lecturas = _db.Lecturas.Where(f => f.usuario == Usuarioid && f.empresa == Empresa && f.insertado == false).GroupBy(x => new { x.identificador, x.fecha })
                        .Select( group => new LecturasAsistenteModel()  { 
                                    Identificador = group.Key.identificador,
                                    Fecha = group.Key.fecha,
                                    Numregistros = group.Count()
                                })
                        .ToList();

            return lecturas;
        }

        public Guid GetUsuarioid()
        {
            return Usuarioid;
        }

        public void ActualizarLecturas(string identificador)
        {
            var lecturas = _db.Lecturas.Where(f => f.identificador == identificador && f.usuario == Usuarioid).ToList();

            foreach (var item in lecturas)
            {
                item.insertado = true;

                _db.Set<Lecturas>().AddOrUpdate(item);
            }

            _db.SaveChanges();
        }
    }
}
