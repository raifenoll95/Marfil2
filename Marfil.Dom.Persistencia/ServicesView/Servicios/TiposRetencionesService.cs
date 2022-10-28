using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITiposRetencionesService
    {

    }

    public class TiposRetencionesService : GestionService<TiposRetencionesModel, Tiposretenciones>, ITiposRetencionesService
    {
        #region CTR

        public TiposRetencionesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            
            model.ExcludedColumns = new[] { "Empresa","Fkcuentarecargo", "Fkcuentaabono", "Tiporendimiento","Toolbar" };

            
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select tr.*, cc.descripcion as CuentaRecargoDescripcion, ca.descripcion as CuentaAbonoDescripcion from tiposretenciones as tr " +
                                 " left join cuentas as cc on cc.empresa=tr.empresa and cc.id= tr.fkcuentascargo  " +
                                 " left join cuentas as ca on ca.empresa=tr.empresa and ca.id=tr.fkcuentasabono " +
                   " where tr.empresa='{0}'",Empresa);
        }

        #endregion

        #region Helpers

        public double GetPorcentaje(string tipo)
        {
            var porcentaje = _db.Tiposretenciones.Where(f => f.empresa == Empresa && f.id == tipo).FirstOrDefault().porcentajeretencion;

            return (double)porcentaje;
        }

        public string GetIvaTercero(string cuenta)
        {
            var grupoiva = _db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == cuenta).FirstOrDefault().fkgruposiva;

            var tipoiva = _db.GruposIvaLin.Where(f => f.empresa == Empresa && f.fkgruposiva == grupoiva).OrderByDescending(x => x.desde).FirstOrDefault().fktiposivasinrecargo;

            //var tipoivaporcentaje = _db.TiposIva.Where(f => f.empresa == Empresa && f.id == tipoiva).FirstOrDefault().porcentajeiva;

            return tipoiva;
        }

        public string GetRegimenivaTercero(string cuenta)
        {
            var regimen = "";
            if (_db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == cuenta).FirstOrDefault() != null)
            {
                regimen = _db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == cuenta).FirstOrDefault().fkregimeniva;
            }
            
            return regimen;
        }

        public string GetPorcentajeIvaTercero(string tipoiva)
        {
            var tipoivaporcentaje = _db.TiposIva.Where(f => f.empresa == Empresa && f.id == tipoiva).FirstOrDefault().porcentajeiva;

            tipoivaporcentaje = Math.Round((double)tipoivaporcentaje, 2);

            return tipoivaporcentaje.ToString();
        }

        public string GetPorcentajeRecargoTercero(string tipoiva)
        {
            var tipoivaporcentaje = _db.TiposIva.Where(f => f.empresa == Empresa && f.id == tipoiva).FirstOrDefault().porcentajerecargoequivalente;

            tipoivaporcentaje = Math.Round((double)tipoivaporcentaje, 2);

            return tipoivaporcentaje.ToString();
        }

        #endregion
    }
}
