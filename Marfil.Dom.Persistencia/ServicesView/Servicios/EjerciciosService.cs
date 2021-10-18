using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IEjerciciosService
    {

    }

    public class EjerciciosService : GestionService<EjerciciosModel, Ejercicios>, IEjerciciosService
    {
        #region CTR

        public EjerciciosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] {  "Descripcion", "Descripcioncorta", "Desde", "Hasta", "Estado", "Contabilidadcerradahasta", "Registroivacerradohasta" };
            var propiedades = Helpers.Helper.getProperties<EjerciciosModel>();
            model.PrimaryColumnns = new[] { "Id" };       
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();



            return model;
        }

        public string GetEjercicioDefecto(Guid tusuario, MarfilEntities db, string tempresa)
        {
            using (var preferencias = new PreferenciasUsuarioService(db))
            {
                var preferenciaItem = preferencias.GePreferencia(TiposPreferencias.EjercicioDefecto,
                                                                    tusuario, 
                                                                    PreferenciaEjercicioDefecto.Id, 
                                                                    PreferenciaEjercicioDefecto.Nombre) 
                                        as PreferenciaEjercicioDefecto;

                if (preferenciaItem != null)
                {
                    var ejercicios = preferenciaItem.ListEjercicios;
                    if (ejercicios.Any())
                    {
                        if (ejercicios.ContainsKey(tempresa))
                        {
                            return ejercicios[tempresa];
                        }
                    }
                }
            }

            return GetEjercicioActual(tempresa,db);
        }

        public string GetEjercicioActual(string empresa,MarfilEntities db=null)
        {
            return
                (db ?? _db).Ejercicios.FirstOrDefault(
                    f =>
                        f.empresa == empresa && f.estado == (int)EstadoEjercicio.Abierto && (DateTime.Now >= f.desde) &&
                        (DateTime.Now <= f.hasta))?.id.ToString() ?? GetEjercicioUltimoAbierto(empresa, db);
        }


        public string GetEjercicioUltimoAbierto(string empresa, MarfilEntities db = null)
        {
            return
                (db ?? _db).Ejercicios.OrderByDescending( f => f.hasta).FirstOrDefault(
                    f =>
                        f.empresa == empresa && f.estado == (int)EstadoEjercicio.Abierto)?.id.ToString() ?? string.Empty;
        }

        public IEnumerable<EjerciciosModel> GetEjercicios(int? id)
        {
            
                var result = getAll().Select(f => (EjerciciosModel) f);

                if (id.HasValue)
                {
                    result = result.Where(f => f.Id != id.Value);
                }

                return result;
            
        }

        public IEnumerable<EjerciciosModel> getEjercicios(string empresa)
        {

            var result = new List<EjerciciosModel>();
            var list = _db.Ejercicios.Where(f => f.empresa == empresa);

            foreach (var ejercicio in list)
            {
                var ejercicioModel = _converterModel.GetModelView(ejercicio) as EjerciciosModel;
                result.Add(ejercicioModel);
            }
            return result;
        }

        public override string GetSelectPrincipal() {

            return string.Format("select * from Ejercicios where empresa='{0}'", Empresa);
        }

        public string DescSerieContable(string id)
        {
            return _db.SeriesContables.Where(f => f.empresa == Empresa && f.id == id).FirstOrDefault().descripcion;
        }
    }
}
