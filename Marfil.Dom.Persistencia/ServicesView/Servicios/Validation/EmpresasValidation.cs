using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.NifValidators;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Resources;
using REmpresa = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Empresas;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class EmpresasValidation : BaseValidation<Empresas>
    {
        public EmpresasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Empresas model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.razonsocial))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REmpresa.Razonsocial));

                if (string.IsNullOrEmpty(model.nombre))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REmpresa.Nombre));

                if (string.IsNullOrEmpty(model.nif))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REmpresa.NifCifAdministrador));

                if (!model.fkMonedabase.HasValue)
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REmpresa.FkMonedaBase));

                if (!String.IsNullOrEmpty(model.ean13) && model.ean13.Length > 10)
                    throw new ValidationException(string.Format(General.ErrorEan13));

                if (model.decimalesprecios.HasValue && model.decimalesprecios > 4)
                    throw new ValidationException(string.Format(General.ErrorDecimalesPrecios));

                if (!ValdiateNif(model))
                    throw new ValidationException(string.Format(General.ErrorFormatoCampo, REmpresa.NifCifAdministrador));

                if (DigitosCuentasModificados(model))
                {
                    var service = FService.Instance;
                    var auxContext = Context;
                    auxContext.Empresa = model.id;
                    var cuentasservice = service.GetService(typeof(CuentasModel), auxContext, _db) as CuentasService;
                    cuentasservice.ActualizarCuentas(model.id);
                }

                VerificarMonedasActivas(model);

                return base.ValidarGrabar(model);
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
           
        }

        private bool ValdiateNif(Empresas model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.fkPais) && !string.IsNullOrEmpty(model.nif))
                {
                    var appService=new ApplicationHelper(Context);
                    var fNifValidatorService = new FNifValidatorService(appService.CheckPaisEuropeoService(model.fkPais));
                    var validator = fNifValidatorService.GetValidator(model.fkPais);
                    var tiposnifs = appService.GetListTiposNif(_db);
                    var hayQueVerificar = tiposnifs.SingleOrDefault(f => f.Valor == model.fktipoidentificacion_nif)?.VerificaValor;
                    if (hayQueVerificar.HasValue && hayQueVerificar.Value)
                    {
                        var cadenaNif = model.nif;
                        if (!validator.Validate(ref cadenaNif))
                            return false;
                        model.nif = cadenaNif;
                    }

                    if (!string.IsNullOrEmpty(model.nifcifadministrador))
                    {
                        var cadenaNif = model.nifcifadministrador;
                        if (!validator.Validate(ref cadenaNif))
                            return false;
                        model.nifcifadministrador = cadenaNif;
                    }

                    model.nif = model.nif.ToUpper();
                }
                
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex.Message);
            }
           

            return true;
        }

        private void VerificarMonedasActivas(Empresas model)
        {
            var fservice= FService.Instance;
            var auxContext = Context;
            auxContext.Empresa = model.id;
            var monedasService = fservice.GetService(typeof (MonedasModel), auxContext, _db);
            if (model.fkMonedabase.HasValue)
            {
                try
                {
                    var monedaBaseObj = monedasService.get(model.fkMonedabase.Value.ToString()) as MonedasModel;
                    if (!monedaBaseObj?.Activado ?? false)
                    {
                        monedaBaseObj.Activado = true;
                        monedasService.edit(monedaBaseObj);
                    }
                }
                catch (Exception)
                {
                    
                    
                }
                
            }

            if (model.fkMonedaadicional.HasValue)
            {
                try
                {
                    var monedaAdicionalObj = monedasService.get(model.fkMonedaadicional.Value.ToString()) as MonedasModel;
                    if (!monedaAdicionalObj.Activado)
                    {
                        monedaAdicionalObj.Activado = true;
                        monedasService.edit(monedaAdicionalObj);
                    }
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
        }

        private bool DigitosCuentasModificados(Empresas model)
        {

            using (var newdbcontext = MarfilEntities.ConnectToSqlServer(_db.Database.Connection.Database))
            {
                var empresaBd = newdbcontext.Empresas.Find(model.id);
                if (empresaBd != null && empresaBd.digitoscuentas != model.digitoscuentas)
                {
                    if (empresaBd.digitoscuentas > model.digitoscuentas)
                        throw new ValidationException(REmpresa.ErrorCantidadDigitosCuentas + " " +
                                                      empresaBd.digitoscuentas);
                    
                        return true;
                }
            }

            return false;
        }

    }
}
