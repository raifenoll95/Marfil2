using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class GrupoMaterialesService : GestionService<GrupoMaterialesModel, GrupoMateriales>
    {
        public GrupoMaterialesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }

        private string _ejercicioId;
        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
            }
        }

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, true, true, controller);
            st.List = st.List.OfType<GrupoMaterialesModel>();
            var propiedadesVisibles = new[] { "Cod", "Descripcion", "InglesDescripcion"};
            var propiedades = Helper.getProperties<GrupoMaterialesModel>();
            st.PrimaryColumnns = new[] { "Cod" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from GrupoMateriales where empresa='{0}'", Empresa);
        }

        #endregion

        #region CRUD
        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as GrupoMaterialesModel;

                var validation = _validationService as GrupoMaterialesValidation;
                validation.EjercicioId = EjercicioId;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.GrupoMateriales, _context, _db);

                //Llamamos al base
                base.create(obj);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                //var original = get(Funciones.Qnull(obj.get("id"))) as GrupoMaterialesModel;
                //var editado = obj as GrupoMaterialesModel;
                base.edit(obj);
                _db.SaveChanges();
                tran.Complete();

                }
            }
        }

        #endregion
    }
