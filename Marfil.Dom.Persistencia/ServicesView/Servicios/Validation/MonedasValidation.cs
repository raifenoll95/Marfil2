using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Resources;
using RMonedas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Monedas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class MonedasValidation : BaseValidation<Monedas>
    {
        #region CTR

        public MonedasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
            
        }

        #endregion

        public override bool ValidarGrabar(Monedas model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMonedas.Descripcion));

            if (string.IsNullOrEmpty(model.abreviatura))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMonedas.Abreviatura));
            
            if (!model.fkUsuarios.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMonedas.Usuario));

            if (string.IsNullOrEmpty(model.fkUsuariosnombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMonedas.Usuario));
            
            model.fechamodificacion=DateTime.Now;

            if (HasChanged(model))
            {
                AddItemToLog(model);
            }

            return base.ValidarGrabar(model);
        }

        private bool HasChanged(Monedas model)
        {
            using (var newContext = MarfilEntities.ConnectToSqlServer(_db.Database.Connection.Database))
            {
                var dbItem = newContext.Monedas.Find(model.id);
                if (dbItem == null) return false;
                return dbItem.cambiomonedabase != model.cambiomonedabase ||
                        dbItem.cambiomonedaadicional != model.cambiomonedaadicional;
            }

        }

        private void AddItemToLog(Monedas model)
        {
            using (var newContext = MarfilEntities.ConnectToSqlServer(_db.Database.Connection.Database))
            {
                var dbItem = newContext.Monedas.Find(model.id);
                var newItem = _db.MonedasLog.Create();
                newItem.cambiomonedabase = dbItem.cambiomonedabase;
                newItem.cambiomonedaadicional = dbItem.cambiomonedaadicional;
                newItem.fechamodificacion = dbItem.fechamodificacion;
                newItem.fkUsuarios = dbItem.fkUsuarios;
                newItem.fkUsuariosnombre = dbItem.fkUsuariosnombre;

                model.MonedasLog.Add(newItem);
            }
                
        }
    }
}
