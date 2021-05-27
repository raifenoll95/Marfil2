using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IAcabadosService
    {
        
    }

    public class AcabadosService : GestionService<AcabadosModel, Acabados>, IAcabadosService
    {
        #region CTR

        public AcabadosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "Descripcion2", "Descripcionabreviada", "Bruto"};

            var propiedades = Helpers.Helper.getProperties<AcabadosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from Acabados as a where a.empresa='{0}'", Empresa);
        }

        #region crud

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AcabadosModel;

                //Hemos puesto a check el bruto
                if(model.Bruto)
                {
                    var posibleAcabado = _db.Acabados.Where(f => f.empresa == Empresa && f.bruto == true).Select(f => f.id).SingleOrDefault();

                    if (!String.IsNullOrEmpty(posibleAcabado)) {

                        var acabadoEnBruto = this.get(posibleAcabado);
                        this.delete(acabadoEnBruto);
                    }
                }

                //Llamamos al base
                base.create(obj);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public bool comprobarAcabadosEnBruto()
        {
            bool haybruto = false;
            var posibleAcabado = _db.Acabados.Where(f => f.empresa == Empresa && f.bruto == true).Select(f => f.id).SingleOrDefault();

            if (!String.IsNullOrEmpty(posibleAcabado))
            {
                var acabadoEnBruto = this.get(posibleAcabado);
                haybruto = true;
                //this.delete(acabadoEnBruto);
            }

            return haybruto;
        }

        public string nombreAcabado()
        {
            string acabado = "";
            acabado = _db.Acabados.Where(f => f.empresa == Empresa && f.bruto == true).Select(f => f.descripcion).SingleOrDefault().ToString();
            return acabado;
        }

        #endregion
    }
}
