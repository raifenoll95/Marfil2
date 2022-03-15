using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITarifasService
    {

    }

    public class TarifasService : GestionService<TarifasModel, Tarifas>, ITarifasService
    {
        #region CTR

        public TarifasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.create(obj);

                CreateRateLines(obj as TarifasModel);
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.edit(obj);

                CreateRateLines(obj as TarifasModel);
                tran.Complete();
            }
        }

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "Fkcuentas", "Tipoflujo", "Tipotarifa", "Validodesde", "Validohasta" };
            var propiedades = Helpers.Helper.getProperties<TarifasModel>();
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return st;
        }

        

        public IEnumerable<TarifasEspecificasArticulosViewModel> GetTarifasEspecificas(TipoFlujo tipoflujo, string articulo, string empresa)
        {

            return _db.Tarifas.SqlQuery(
                "select t.* from tarifas as t inner join tarifaslin as tl on tl.empresa=t.empresa and tl.fktarifas=t.id and tl.fkarticulos=@articulo where t.empresa=@empresa and t.tipoflujo=@tipoflujo and t.tipotarifa=@tipotarifa " +
                " and (t.validodesde is null or t.validodesde<=@fechaactual) and (t.validohasta is null or t.validohasta>=@fechaactual)",
                new object[]
                {
                    new SqlParameter("@empresa", empresa),
                    new SqlParameter("@articulo", articulo),
                    new SqlParameter("@tipoflujo", (int)tipoflujo),
                    new SqlParameter("@tipotarifa", (int)TipoTarifa.Especifica),
                    new SqlParameter("@fechaactual", DateTime.Now)
                }).Select(f => new TarifasEspecificasArticulosViewModel()
                {
                    Descripcion = f.descripcion,
                    Id = f.id,
                    Fkcuenta = f.fkcuentas,
                    Precio = f.TarifasLin.ToList().FirstOrDefault(j => j.fkarticulos == articulo)?.precio ?? 0,
                    Obligatorio = f.precioobligatorio ?? false,
                    Descuento = f.TarifasLin.ToList().FirstOrDefault(j => j.fkarticulos == articulo)?.descuento ?? 0,

                }).ToList();


        }

        private void CreateRateLines(TarifasModel model)
        {
            var creartarifa = model.Asignartarifaalcreararticulos;
            if (creartarifa)
            {
                var articles = _db.Articulos.Where(f => f.empresa == Empresa);
                foreach (var item in articles)
                {
                    if (
                        !_db.TarifasLin.Any(
                            f => f.empresa == Empresa && f.fktarifas == model.Id && f.fkarticulos == item.id) && Filtrar(model, item.id))
                    {
                        var newLine = _db.TarifasLin.Create();
                        newLine.empresa = Empresa;
                        newLine.fktarifas = model.Id;
                        newLine.fkarticulos = item.id;
                        newLine.precio = CalcularPrecio(model, item);
                        newLine.descuento = 0;
                        _db.TarifasLin.Add(newLine);
                    }
                }
                _db.SaveChanges();
            }
        }

        private bool Filtrar(TarifasModel tarifa, string idarticulo)
        {
            var familia = idarticulo.Substring(0, 2);
            var material = idarticulo.Substring(2, 3);

            var result = true;
            if (!string.IsNullOrEmpty(tarifa.Precioautomaticofkfamiliasproductosdesde) && string.CompareOrdinal(familia, tarifa.Precioautomaticofkfamiliasproductosdesde) < 0)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(tarifa.Precioautomaticofkfamiliasproductoshasta) && string.CompareOrdinal(familia, tarifa.Precioautomaticofkfamiliasproductoshasta) > 0)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(tarifa.Precioautomaticofkmaterialesdesde) && string.CompareOrdinal(material, tarifa.Precioautomaticofkmaterialesdesde) < 0)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(tarifa.Precioautomaticofkmaterialeshasta) && string.CompareOrdinal(material, tarifa.Precioautomaticofkmaterialeshasta) > 0)
            {
                return false;
            }

            return result;
        }

        private double CalcularPrecio(TarifasModel tarifa, Articulos model)
        {
            var result = 0.0;

            if (tarifa.Precioautomaticobase == "---")
            {
                result = model.coste ?? 0;
                ///tarifa.Tipoflujo==TipoFlujo.Compra?  model.coste ??0 : model.preciominimoventa ?? 0;
            }
            else
            {
                result =
                        _db.TarifasLin.SingleOrDefault(
                            f =>
                                f.empresa == tarifa.Empresa && f.fktarifas == tarifa.Precioautomaticobase &&
                                f.fkarticulos == model.id)?.precio ?? 0.0;
            }

            var porcentajeIva = tarifa.Precioautomaticoporcentajebase;
            var constante = tarifa.Precioautomaticoporcentajefijo;

            result = result + (result * porcentajeIva / 100.0) + (constante);


            return result;
        }

        public void Bloquear(string id, string motivo, string user, bool bloqueado)
        {
            var tarifa = _db.Tarifas.Single(f => f.empresa == Empresa && f.id == id);
            tarifa.fkMotivosbloqueo = motivo;
            tarifa.fkUsuariobloqueo = new Guid(user);
            tarifa.fechamodificacionbloqueo = DateTime.Now;
            tarifa.bloqueada = bloqueado;
            _db.Entry(tarifa).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public double? getPrecioComponentes(string componente)
        {
            var tarifa = _db.Tarifas.Where(f => f.empresa == Empresa && f.valorarcomponentes == true).FirstOrDefault();
            var precio = tarifa != null ? _db.TarifasLin.Where(f => f.empresa == Empresa && f.fktarifas == tarifa.id && f.fkarticulos == componente).Select(f => f.precio).SingleOrDefault() : null; 
            return precio;
        }

        public IEnumerable<TarifasModel> GetTarifas()
        {
            return _db.Tarifas.Where(f => f.empresa == Empresa && f.tipotarifa == 0 && f.tipoflujo == 0).ToList().Select(f => _converterModel.GetModelView(f) as TarifasModel).ToList(); 
        }

        public IEnumerable<TarifasModel> GetTarifasTodas()
        {
            return _db.Tarifas.Where(f => f.empresa == Empresa).ToList().Select(f => _converterModel.GetModelView(f) as TarifasModel).ToList();
        }

        public TarifasModel GetTarifaCompleta(string id)
        {
            return _db.Tarifas.Where(f => f.empresa == Empresa && f.id == id).Include("TarifasLin").ToList().Select(f => _converterModel.GetModelView(f) as TarifasModel).FirstOrDefault();
        }
    }
}
