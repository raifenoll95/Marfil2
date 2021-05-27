using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RCuenta = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CuentasConverterService : BaseConverterModel<CuentasModel, Cuentas>
    {
        public CuentasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

       

        public override IEnumerable<IModelView> GetAll()
        {
            var list = _db.Cuentas.Where(f => f.empresa == Empresa).ToList();

            var result=new List<CuentasModel>();
            foreach (var item in list)
            {
                result.Add(GetModelView(item) as CuentasModel);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Cuentas>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Cuentas>().Single(f => f.id == id && f.empresa==Empresa);
            var result = GetModelView(obj) as CuentasModel;
            var fservice=FService.Instance;
            var tipocuentaService = fservice.GetService(typeof (TiposCuentasModel), Context, _db);
            if (obj.tipocuenta.HasValue)
            {
                var tipoCuentaModel = tipocuentaService.get(obj.tipocuenta.Value.ToString()) as TiposCuentasModel;
                result.Nifrequired = tipoCuentaModel.Nifobligatorio;
            }
            var service = new TablasVariasService(Context,_db);
            var motivobloquedescripcion = string.Empty;
            if (!string.IsNullOrEmpty(obj.fkMotivosbloqueo))
            {
                var motivosbloqueo = service.GetTablasVariasByCode(12);
                motivobloquedescripcion = motivosbloqueo.Lineas.Single(f => f.Valor == obj.fkMotivosbloqueo).Descripcion;
            }
            result.BloqueoModel = new BloqueoEntidadModel
            {
                Entidad=RCuenta.TituloEntidadSingular,
                Campoclaveprimaria = "Id",
                Valorclaveprimaria = result.Id,
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

        public override Cuentas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CuentasModel;
            var result = _db.Set<Cuentas>().Create();
            result.id = viewmodel.Id;
            result.descripcion2 = viewmodel.Descripcion2;
            result.descripcion = viewmodel.Descripcion;
            result.nivel = viewmodel.Nivel;
            result.empresa = viewmodel.Empresa;
            result.fkUsuarios = new Guid(viewmodel.UsuarioId);
            result.fechaalta = viewmodel.Fechaalta;
            
            if (viewmodel.Nivel == 0)
            {
                result.fkPais = viewmodel.FkPais;
                result.nif = viewmodel.Nif?.Nif;
                result.fktipoidentificacion_nif = viewmodel.Nif?.TipoNif;
                result.contrapartida = viewmodel.Contrapartida;
                result.tipocuenta = viewmodel.Tiposcuentas;

                result.bloqueada = viewmodel.BloqueoModel?.Bloqueada;
                result.fkMotivosbloqueo = viewmodel.BloqueoModel?.FkMotivobloqueo;
                if (!string.IsNullOrEmpty(viewmodel.BloqueoModel?.FkUsuario))
                {
                    result.fechamodificacionbloqueo = viewmodel.BloqueoModel?.Fecha;
                    result.fkUsuariobloqueo = new Guid(viewmodel.BloqueoModel?.FkUsuario);
                }
            }
            
            return result;
        }

        public override Cuentas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CuentasModel;
            var result = _db.Cuentas.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
            result.id = viewmodel.Id;
            result.descripcion = viewmodel.Descripcion;
            result.descripcion2 = viewmodel.Descripcion2;
            result.nivel = viewmodel.Nivel;
            result.empresa = viewmodel.Empresa;
            result.fkUsuarios = new Guid(viewmodel.UsuarioId);
            
            if (viewmodel.Nivel == 0)
            {
                result.fkPais = viewmodel.FkPais;
                result.nif = viewmodel.Nif.Nif;
                result.fktipoidentificacion_nif = viewmodel.Nif.TipoNif;
                result.contrapartida = viewmodel.Contrapartida;
                result.tipocuenta = viewmodel.Tiposcuentas;

                result.bloqueada = viewmodel.BloqueoModel?.Bloqueada;
                result.fkMotivosbloqueo = viewmodel.BloqueoModel?.FkMotivobloqueo;
                if (!string.IsNullOrEmpty(viewmodel.BloqueoModel?.FkUsuario))
                {
                    result.fechamodificacionbloqueo = viewmodel.BloqueoModel?.Fecha;
                    result.fkUsuariobloqueo = new Guid(viewmodel.BloqueoModel?.FkUsuario);
                }

            }
            return result;
        }

        public override IModelView GetModelView(Cuentas obj)
        {
            
            var result =  new CuentasModel
            {
                Id = obj.id,
                Descripcion = obj.descripcion,
                Descripcion2 = obj.descripcion2,
                Nivel = obj.nivel.Value,
                Empresa = obj.empresa,
                FkPais = obj.fkPais,
                Nif = new NifCifModel() { Nif = obj.nif,TipoNif = obj.fktipoidentificacion_nif },
                
                FechaModificacion = obj.fechamodificacion ?? DateTime.MinValue,
                UsuarioId =  obj.fkUsuarios?.ToString() ?? string.Empty,
                Usuario = obj.fkUsuarios == Guid.Empty? ApplicationHelper.UsuariosAdministrador : _db.Usuarios.SingleOrDefault(f=>f.id==obj.fkUsuarios.Value)?.usuario,
                Contrapartida= obj.contrapartida,
                ContrapartidaDescripcion = !string.IsNullOrEmpty(obj.contrapartida) ? _db.Cuentas.Single(f=>f.id==obj.contrapartida && f.empresa== obj.empresa).descripcion : string.Empty,
                Tiposcuentas = obj.tipocuenta,
                BloqueoModel = new BloqueoEntidadModel() { Bloqueada = obj.bloqueada ?? false }
            };

            

            return result;
        }

        
    }
}
