using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class RemesasService : GestionService<RemesasModel, Remesas>
    {
        #region CTR

        public RemesasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Tipovencimiento", "Referenciaremesa", "Fecharemesa", "Fkcuentastesoreria", "Descripcioncuenta", "Referencia", "Importegiro" };
            var propiedades = Helpers.Helper.getProperties<RemesasModel>();

            model.PrimaryColumnns = new[] { "Id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Tipovencimiento", 0);
            model.OrdenColumnas.Add("Referenciaremesa", 1);
            model.OrdenColumnas.Add("Fecharemesa", 2);
            model.OrdenColumnas.Add("Fkcuentastesoreria", 3);
            model.OrdenColumnas.Add("Descripcioncuenta", 4);
            model.OrdenColumnas.Add("Referencia", 5);
            model.OrdenColumnas.Add("Importegiro", 6);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select i.id,i.tipovencimiento,i.referenciaremesa,i.fecharemesa,i.fkcuentastesoreria,c.descripcion as Descripcioncuenta,i.referencia,i.importegiro from remesas as i " +
                " inner join cuentas as c on c.id=i.fkcuentastesoreria and c.empresa=i.empresa" +
                " where i.empresa='{0}'", Empresa);
        }

        #endregion

        public int GetCuaderno(string cuaderno)
        {
            return _db.CuadernosBancarios.Where(f => f.clave == cuaderno && f.empresa == Empresa).FirstOrDefault().id;
        }
        public int? GetFormato(string cuaderno)
        {
            return _db.CuadernosBancarios.Where(f => f.clave == cuaderno && f.empresa == Empresa).FirstOrDefault().formato;
        }

        public List<CuadernosBancariosLin> GetCuadernoCabecera(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Cabecera").ToList();
        }
        public List<CuadernosBancariosLin> GetCuadernoDetalle(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Detalle").ToList();
        }
        public List<CuadernosBancariosLin> GetCuadernoTotal(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Total").ToList();
        }
    }
}
