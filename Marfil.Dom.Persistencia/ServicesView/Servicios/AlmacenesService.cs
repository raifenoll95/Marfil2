using System;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IAlmacenesService
    {

    }

    public class AlmacenesService : GestionService<AlmacenesModel, Almacenes>, IAlmacenesService
    {
        #region CTR

        public AlmacenesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion","Coordenadas" };
            var propiedades = Helpers.Helper.getProperties<AlmacenesModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return st;
        }

        #endregion

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlmacenesModel;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Almacenes, _context, _db);

                base.create(obj);
                //guardar direcciones
                GuardarDirecciones(model);
                tran.Complete();
            }

        }

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlmacenesModel;
                //comentario (Prevencion)
                //DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Almacenes, _context, _db);
                base.edit(obj);
                //guardar direcciones
                GuardarDirecciones(model);
                tran.Complete();
            }
        }

        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlmacenesModel;

                base.delete(obj);
                DeleteDirecciones(model);
                tran.Complete();
            }
        }


        #endregion

        #region Almacen defecto

        public string GetAlmacenDefecto(Guid tusuario, MarfilEntities db, string tempresa)
        {
            using (var preferencias = new PreferenciasUsuarioService(db))
            {
                var preferenciaItem = preferencias.GePreferencia(TiposPreferencias.AlmacenDefecto, tusuario, PreferenciaAlmacenDefecto.Id, PreferenciaAlmacenDefecto.Nombre) as PreferenciaAlmacenDefecto;
                if (preferenciaItem != null)
                {
                    var Almacens = preferenciaItem.ListAlmacenes;
                    if (Almacens?.Any()??false)
                    {
                        if (Almacens.ContainsKey(tempresa))
                        {
                            return Almacens[tempresa];
                        }
                    }


                }
            }

            return GetAlmacenActual(tempresa, db);
        }

        public string GetAlmacenActual(string empresa, MarfilEntities db = null)
        {
            return (db ?? _db).Almacenes.FirstOrDefault(f =>f.empresa == empresa)?.id ?? string.Empty;
        }

        #endregion

        #region Helper Direcciones

        private void ProcessDirecciones(AlmacenesModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Empresa;
                item.Fkentidad = model.Id;
                item.Tipotercero =ApplicationHelper.ALMACENDIRECCIONINT;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(AlmacenesModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(AlmacenesModel model)
        {
            var direccionesService = FService.Instance.GetService(typeof(DireccionesLinModel), _context, _db) as DireccionesService;
            direccionesService.CleanAllDirecciones(Empresa, ApplicationHelper.ALMACENDIRECCIONINT, model.Id);
            if (model.Direcciones == null) return;

            ProcessDirecciones(model);
           
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void DeleteDirecciones(AlmacenesModel model)
        {
            if (model.Direcciones == null) return;
            if (model.Direcciones.Direcciones == null) return;
            var fservice = FService.Instance;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), _context, _db);
            //ProcessDirecciones(model);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.delete(item);
            }
        }
        #endregion
    }
}
