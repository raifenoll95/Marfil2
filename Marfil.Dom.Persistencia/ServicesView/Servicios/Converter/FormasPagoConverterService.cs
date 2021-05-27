using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RFormaPago = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Formaspago;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class FormasPagoConverterService : BaseConverterModel<FormasPagoModel, FormasPago>
    {
        public FormasPagoConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<FormasPago>().Include(f => f.FormasPagoLin).ToList().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idInt = int.Parse(id);
            var obj = _db.Set<FormasPago>().Include(f => f.FormasPagoLin).Single(f => f.id == idInt);
            var result = GetModelView(obj) as FormasPagoModel;

            var service = new TablasVariasService(Context,_db);
            var motivobloquedescripcion = string.Empty;
            if (!string.IsNullOrEmpty(obj.fkMotivosbloqueo))
            {
                var motivosbloqueo = service.GetTablasVariasByCode(12);
                motivobloquedescripcion = motivosbloqueo.Lineas.Single(f => f.Valor == obj.fkMotivosbloqueo).Descripcion;
            }
            result.BloqueoModel = new BloqueoEntidadModel
            {
                Entidad = RFormaPago.TituloEntidadSingular,
                Campoclaveprimaria = "Id",
                Valorclaveprimaria = result.Id.ToString(),
                Bloqueada = obj.bloqueada.HasValue && obj.bloqueada.Value,
                FkMotivobloqueo = obj.fkMotivosbloqueo,
                FkMotivobloqueoDescripcion = motivobloquedescripcion,
                Fecha = obj.fechamodificacionbloqueo ?? DateTime.MinValue,
                FkUsuario = obj.fkUsuariobloqueo?.ToString() ?? string.Empty,
                FkUsuarioNombre =
                    obj.fkUsuariobloqueo == Guid.Empty
                        ? ApplicationHelper.UsuariosAdministrador
                        : _db.Usuarios.SingleOrDefault(f => f.id == obj.fkUsuariobloqueo.Value)?.usuario,
            };
            return result;
        }

        public override FormasPago CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as FormasPagoModel;
            var result = _db.Set<FormasPago>().Create();
            result.id = viewmodel.Id;
            result.nombre = viewmodel.Nombre;
            result.nombre2 = viewmodel.Nombre2;
            result.imprimirvencimientos = viewmodel.ImprimirVencimientoFacturas;
            result.recargofinanciero = viewmodel.RecargoFinanciero;
            result.fkModosPago = viewmodel.ModoPago;
            result.efectivo = viewmodel.Efectivo;
            result.remesable = viewmodel.Remesable;
            result.mandato = viewmodel.Mandato;
            result.excluirfestivos = viewmodel.ExcluirFestivos;
            result.fkgruposformaspago = viewmodel.FkGruposformaspago;
            result.bloqueada = viewmodel.BloqueoModel?.Bloqueada;
            result.fkMotivosbloqueo = viewmodel.BloqueoModel?.FkMotivobloqueo;
            if (!string.IsNullOrEmpty(viewmodel.BloqueoModel?.FkUsuario))
            {
                result.fechamodificacionbloqueo = viewmodel.BloqueoModel?.Fecha;
                result.fkUsuariobloqueo = new Guid(viewmodel.BloqueoModel?.FkUsuario);
            }

            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<FormasPagoLin>().Create();
                newItem.diasvencimiento = item.DiasVencimiento;
                newItem.porcentajerecargo = item.PorcentajePago;
                result.FormasPagoLin.Add(newItem);
            }



            return result;
        }

        public override FormasPago EditPersitance(IModelView obj)
        {
            var viewmodel = obj as FormasPagoModel;
            var result = _db.FormasPago.Where(f => f.id == viewmodel.Id).Include(b => b.FormasPagoLin).ToList().Single();
            result.id = viewmodel.Id;
            result.nombre = viewmodel.Nombre;
            result.nombre2 = viewmodel.Nombre2;
            result.imprimirvencimientos = viewmodel.ImprimirVencimientoFacturas;
            result.recargofinanciero = viewmodel.RecargoFinanciero;
            result.fkModosPago = viewmodel.ModoPago;
            result.efectivo = viewmodel.Efectivo;
            result.remesable = viewmodel.Remesable;
            result.mandato = viewmodel.Mandato;
            result.excluirfestivos = viewmodel.ExcluirFestivos;
            result.fkgruposformaspago = viewmodel.FkGruposformaspago;
            result.bloqueada = viewmodel.BloqueoModel?.Bloqueada;
            result.fkMotivosbloqueo = viewmodel.BloqueoModel?.FkMotivobloqueo;
            if (!string.IsNullOrEmpty(viewmodel.BloqueoModel?.FkUsuario))
            {
                result.fechamodificacionbloqueo = viewmodel.BloqueoModel?.Fecha;
                result.fkUsuariobloqueo = new Guid(viewmodel.BloqueoModel?.FkUsuario);
            }
            result.FormasPagoLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<FormasPagoLin>().Create();
                newItem.diasvencimiento = item.DiasVencimiento;
                newItem.porcentajerecargo = item.PorcentajePago;
                result.FormasPagoLin.Add(newItem);
            }

            return result;
        }

        public override IModelView GetModelView(FormasPago obj)
        {
            var result = new FormasPagoModel
            {
                Id = obj.id,
                Nombre = obj.nombre,
                Nombre2 = obj.nombre2,
                ImprimirVencimientoFacturas = obj.imprimirvencimientos.Value,
                RecargoFinanciero = obj.recargofinanciero.Value,
                ModoPago = obj.fkModosPago,
                Efectivo = obj.efectivo.HasValue && obj.efectivo.Value,
                Remesable = obj.remesable.HasValue && obj.remesable.Value,
                Mandato = obj.mandato.HasValue && obj.mandato.Value,
                FkGruposformaspago = obj.fkgruposformaspago,
                ExcluirFestivos = obj.excluirfestivos.HasValue && obj.excluirfestivos.Value,
                Lineas = obj.FormasPagoLin.Select(f => new FormasPagoLinModel() { Id = f.id, DiasVencimiento = f.diasvencimiento.Value, PorcentajePago = f.porcentajerecargo.Value }),
                BloqueoModel = new BloqueoEntidadModel() {  Bloqueada = obj.bloqueada ?? false}
                
            };
           
            return result;

        }
    }
}
