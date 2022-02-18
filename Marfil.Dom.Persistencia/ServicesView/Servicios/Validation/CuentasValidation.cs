using System;
using System.Linq;
using System.Reflection;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.Genericos.NifValidators;
using Resources;
using RCuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CuentasValidation : BaseValidation<Cuentas>
    {

        #region Members

        private int _totalNivelesSupercuentas;
        private int _totalLongitud;
        private FService _fservice;
        #endregion

        #region Properties

        internal bool FlagDeleteFromThird { get; set; }

        #endregion

        #region CTR

        public CuentasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        #region Validar grabar

        public override bool ValidarGrabar(Cuentas model)
        {
            if (model.bloqueada.HasValue && model.bloqueada.Value)
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);
            var appService=new ApplicationHelper(Context);
            _totalNivelesSupercuentas = appService.NivelesCuentas(model.empresa, _db);
            _totalLongitud = appService.DigitosCuentas(model.empresa, _db);

            _fservice = FService.Instance;
            var cuentasService = _fservice.GetService(typeof(CuentasModel),Context, _db) as CuentasService;
            var tiposcuentasService = _fservice.GetService(typeof(TiposCuentasModel), Context, _db) as TiposcuentasService;

            if (string.IsNullOrEmpty(model.id))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuentas.Id));

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuentas.Descripcion));

            FormatearId(model);

            if (model.tipocuenta.HasValue)
            {
                var tipoobj =
                    _db.Tiposcuentas.Single(f => f.empresa == model.empresa && f.tipos == model.tipocuenta.Value);
                model.categoria = tipoobj.categoria;
            }
            else model.categoria = (int)CategoriasCuentas.Contables;

            if (model.categoria == (int)CategoriasCuentas.Contables)
                CrearNivelesSuperiores(model);


            model.nivel = GetCurrentLevel(model.id);

            if (model.nivel == 0 && model.tipocuenta.HasValue && model.tipocuenta.Value >= 0)
            {
                if (!ValidarNifCifObligatorio(tiposcuentasService, model))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuentas.Nif));

                if (string.IsNullOrEmpty(model.fkPais))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuentas.FkPais));
            }

            if (!ValidateNif(model))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RCuentas.Nif));

            if (!model.fkUsuarios.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCuentas.Usuario));

            model.fechamodificacion = DateTime.Now;






            return base.ValidarGrabar(model);
        }

        private bool ValidateNif(Cuentas model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.fkPais) && !string.IsNullOrEmpty(model.nif))
                {
                    var appService=new ApplicationHelper(Context);
                    var tiposnifs = appService.GetListTiposNif(_db);
                    var hayQueVerificar =
                        tiposnifs.SingleOrDefault(f => f.Valor == model.fktipoidentificacion_nif)?.VerificaValor;
                    if (hayQueVerificar.HasValue && hayQueVerificar.Value)
                    {
                        var fNifValidatorService =
                            new FNifValidatorService(appService.CheckPaisEuropeoService(model.fkPais));
                        var validator = fNifValidatorService.GetValidator(model.fkPais);
                        var cadenaNif = model.nif;

                        if (!validator.Validate(ref cadenaNif))
                            return false;
                        model.nif = cadenaNif;
                    }

                    model.nif = model.nif.ToUpper();
                }
            }
            catch (ReflectionTypeLoadException ex2)
            {
                throw new ValidationException(ex2.Message);
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }


            return true;
        }

        private int GetCurrentLevel(string id)
        {
            return id.Length > _totalNivelesSupercuentas ? 0 : id.Length;
        }

        private bool ValidarNifCifObligatorio(TiposcuentasService tiposcuentasService, Cuentas model)
        {

            if (string.IsNullOrEmpty(model.nif))
            {

                var tiposcuentasModel = tiposcuentasService.get(model.tipocuenta.Value.ToString()) as TiposCuentasModel;

                return !tiposcuentasModel.Nifobligatorio;
            }
            return true;
        }

        private void FormatearId(Cuentas model)
        {
            var currentLevel = GetCurrentLevel(model.id);
            if (currentLevel == 0)
            {
                var superCuentas = model.id.Substring(0, _totalNivelesSupercuentas);
                var cuentaCliente = Tools.RellenaCeros(model.id.Substring(_totalNivelesSupercuentas), _totalLongitud - _totalNivelesSupercuentas);

                model.id = superCuentas + cuentaCliente;
            }
        }

        private void CrearNivelesSuperiores(Cuentas cuenta)
        {


            var id = cuenta.id;
            var currentLevel = GetCurrentLevel(id) == 0 ? _totalNivelesSupercuentas + 1 : GetCurrentLevel(id);
            for (var i = currentLevel; i > 1; i--, currentLevel--)
            {
                var step = currentLevel > _totalNivelesSupercuentas ? (_totalLongitud - _totalNivelesSupercuentas) : 1;//la logitud del identificador del nivel
                var subCuenta = id.Substring(0, id.Length - step);
                id = subCuenta;

                if (!_db.Cuentas.Any(f => f.id == subCuenta && f.empresa == cuenta.empresa))
                {
                    var fmodel = new FModel();
                    var descripcion = GetDescripcionSubcuenta(subCuenta, cuenta.empresa, (currentLevel - 1).ToString());
                    var descripcion2 = descripcion;
                    var cuentaService = FService.Instance.GetService(typeof(CuentasModel), Context, _db);
                    var cuentaModel = new CuentasModel();
                    cuentaModel.Empresa = cuenta.empresa;
                    cuentaModel.Id = subCuenta;
                    cuentaModel.Descripcion = descripcion;
                    cuentaModel.Descripcion2 = descripcion2;
                    cuentaModel.UsuarioId = cuenta.fkUsuarios.ToString();
                    cuentaModel.Nif = new NifCifModel();
                    cuentaModel.BloqueoModel = new BloqueoEntidadModel() { Entidad = RCuentas.TituloEntidadSingular };
                    cuentaService.create(cuentaModel);
                }
            }
        }

        private string GetDescripcionSubcuenta(string id, string empresa, string level)
        {
            var currentLevel = GetCurrentLevel(id) == 0 ? _totalNivelesSupercuentas + 1 : GetCurrentLevel(id);
            var result = "NIVEL " + level;
            var subCuenta = id.Substring(0, id.Length - 1);
            if (currentLevel == 1)
            {
                var cuenta = _db.Cuentas.FirstOrDefault(f => f.id == subCuenta && f.empresa == empresa);
                if (cuenta != null)
                    result = cuenta.descripcion;
            }
            else
            {
                var cuenta = _db.Cuentas.FirstOrDefault(f => f.id == subCuenta && f.empresa == empresa);
                if (cuenta != null)
                    result = cuenta.descripcion;
                else
                {
                    result = GetDescripcionSubcuenta(subCuenta, empresa, level);
                }
            }

            return result;
        }

        #endregion

        #region Validar borrar

        public override bool ValidarBorrar(Cuentas model)
        {
            if (Context.Ejercicio != null)
            {
                var ejercicioparse = int.Parse(Context.Ejercicio);
                if (_db.Maes.Where(f => f.empresa == Context.Empresa && f.fkejercicio == ejercicioparse && f.fkcuentas == model.id).FirstOrDefault() != null)
                {
                    throw new ValidationException(General.ErrorCuentaConSaldos);
                }
            }
            else//Se elimina la empresa
            {
                if (_db.Maes.Where(f => f.empresa == Context.Empresa && f.fkcuentas == model.id).FirstOrDefault() != null)
                {
                    throw new ValidationException(General.ErrorCuentaConSaldos);
                }
            }
           

            if (ExistenNivelesInferiores(model.id, model.empresa))
                throw new ValidationException(RCuentas.ErrorBorrado);

            if (model.tipocuenta.HasValue && !FlagDeleteFromThird)
            {
                var controller = ((TiposCuentas)model.tipocuenta.Value).ToString();
                throw new ValidationException(string.Format(RCuentas.ErrorOperacionTipoCuenta, Funciones.GetEnumByStringValueAttribute(((TiposCuentas)model.tipocuenta.Value)), controller, model.id));
            }

            //Todo Elimianr cuentas asociadas al tipo de cuenta si existiesen

            return base.ValidarBorrar(model);
        }

        private bool ExistenNivelesInferiores(string id, string empresa)
        {
            return _db.Cuentas.Any(f => f.empresa == empresa && f.id != id && f.id.StartsWith(id));
        }

        #endregion
    }
}
