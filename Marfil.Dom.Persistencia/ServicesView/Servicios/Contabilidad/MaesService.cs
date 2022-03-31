using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{


    public enum TipoOperacionMaes
    {
        Alta,
        Baja
    }

    public interface IMaesService
    {
        //void GenerarLineas(MovsModel model);
        //void GenerarLineas(MovsModel model, MaesModel.TipoOperacion operacion);
    }

    public class MaesService : GestionService<MaesModel, Maes>, IMaesService
    {

        #region Member

        private string _ejercicioId;

        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((MaesValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR
        public MaesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            EjercicioId = context.Ejercicio;
        }
        #endregion CTR


        #region Api

        public override IModelView get(string id)
        {
            ((MaesConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

        public void GenerarMovimiento(MovsModel model, TipoOperacionMaes tipo)// short multiplo)
        // multiplo 1 para alta  -1 para baja
        {
            // nivel 0
            foreach (var item in model.Lineas.GroupBy(l => l.Fkcuentas))
            {
                string fkcuentas = item.Key;

                //Creamos la cuenta en caso de que no exista en el proceso de asignar a cartera
                if (!_db.Cuentas.Any(f => f.empresa == model.Empresa && f.id == fkcuentas) && model.Generar == GenerarMovimientoAPartirDe.AsignarCartera)
                {
                    var nuevacuenta = new CuentasModel(_context);
                    nuevacuenta.Empresa = model.Empresa;
                    nuevacuenta.Id = fkcuentas;
                    nuevacuenta.Descripcion = "**ALTA CUENTA AUTOMÁTICA**";
                    nuevacuenta.Descripcion2 = "**ALTA CUENTA AUTOMÁTICA**";
                    nuevacuenta.UsuarioId = _db.Usuarios.Where(f => f.usuario == _context.Usuario).Select(f => f.id.ToString()).SingleOrDefault() ?? "";
                    nuevacuenta.Nivel = 0;
                    var id = model.Lineas[1].Fkcuentas.ToString();

                    var newNif = new NifCifModel
                    {
                        Nif = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == id).Select(f => f.nif).SingleOrDefault() ?? ""
                    };

                    nuevacuenta.Nif = newNif;
                    nuevacuenta.FkPais = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == id).Select(f => f.fkPais).SingleOrDefault() ?? "";
                    nuevacuenta.Fechaalta = DateTime.Now;

                    var cuentaService = new CuentasService(_context);
                    cuentaService.create(nuevacuenta);
                }

                var itemmaes = _db.Maes.SingleOrDefault(f => f.empresa == model.Empresa && f.fkcuentas == fkcuentas && f.fkejercicio == model.Fkejercicio)
                              ?? _db.Maes.Create();

                if (string.IsNullOrWhiteSpace(itemmaes.empresa))
                {
                    itemmaes.empresa = model.Empresa;
                    itemmaes.fkcuentas = fkcuentas;
                    itemmaes.fkejercicio = model.Fkejercicio;
                }
                int multiplo = (tipo == TipoOperacionMaes.Alta ? 1 : -1);

                itemmaes.debe = (itemmaes.debe ?? 0) + (item.Where(l => l.Esdebe == 1).Sum(l => l.Importe) * (multiplo));
                itemmaes.haber = (itemmaes.haber ?? 0) + (item.Where(l => l.Esdebe == -1).Sum(l => l.Importe) * (multiplo));
                itemmaes.saldo = (itemmaes.debe ?? 0) - (itemmaes.haber ?? 0);

                _db.Maes.AddOrUpdate(itemmaes);
            }


            // nivel 0 - 4
            for (int nivel = 4; nivel > 0; nivel--)
            {
                foreach (var item in model.Lineas.GroupBy(l => l.Fkcuentas.Substring(0, nivel)))
                {
                    string fkcuentas = item.Key;//.Substring(0, nivel);

                    var itemmaes = _db.Maes.SingleOrDefault(f => f.empresa == model.Empresa && f.fkcuentas == fkcuentas && f.fkejercicio == model.Fkejercicio)
                                   ?? _db.Maes.Create();

                    if (string.IsNullOrWhiteSpace(itemmaes.empresa))
                    {
                        itemmaes.empresa = model.Empresa;
                        itemmaes.fkcuentas = fkcuentas;
                        itemmaes.fkejercicio = model.Fkejercicio;
                    }
                    int multiplo = (tipo == TipoOperacionMaes.Alta ? 1 : -1);

                    itemmaes.debe = (itemmaes.debe ?? 0) + (item.Where(l => l.Esdebe == 1).Sum(l => l.Importe) * (multiplo));
                    itemmaes.haber = (itemmaes.haber ?? 0) + (item.Where(l => l.Esdebe == -1).Sum(l => l.Importe) * (multiplo));
                    itemmaes.saldo = (itemmaes.debe ?? 0) - (itemmaes.haber ?? 0);

                    _db.Maes.AddOrUpdate(itemmaes);
                }
            }
        }

        #endregion
    }
}