using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;
using Resources;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    //public interface ISeguimientosCorreoService
    //{

    //}


    public class SeguimientosCorreoService : GestionService<SeguimientosCorreoModel, SeguimientosCorreo>
    {

        public SeguimientosCorreoService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        //Crear los correos del seguimiento
        public void createLineasCorreos(SeguimientosCorreoModel correo)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                if(_db.SeguimientosCorreo.Any(f => f.empresa == Empresa && f.fkseguimientos == correo.Fkseguimientos && f.fkorigen == correo.Fkorigen))
                {
                    var id = _db.SeguimientosCorreo.Where(f => f.empresa == Empresa && f.fkseguimientos == correo.Fkseguimientos && f.fkorigen == correo.Fkorigen).Max(f => (f.id) + 1);
                    {
                        correo.Id = id;
                    }
                }

                else
                {
                    correo.Id = 0;
                }
               
                base.create(correo);
                _db.SaveChanges();
                tran.Complete();
            }
        }
    }

}
