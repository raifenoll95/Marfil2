using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class EmpresasConverterService : BaseConverterModel<EmpresaModel, Empresas>
    {
        public EmpresasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var result =  base.CreateView(id) as EmpresaModel;
            var auxContext = Context;
            auxContext.Empresa = id;
            var fService = FService.Instance;
            var direccionesService = fService.GetService(typeof(DireccionesLinModel), auxContext, _db) as DireccionesService;
            var ejerciciosService = fService.GetService(typeof(EjerciciosModel), auxContext, _db) as EjerciciosService;
            result.Direcciones=new DireccionesModel();
            result.Direcciones.Empresa = id;
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(id, -1,id);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero =-1;
            result.LstTarifasVentas = _appService.GetListTarifasBase(TipoFlujo.Venta,_db).Select(f => new SelectListItem() { Value = f.Fktarifa, Text = f.Descripcion });
            result.LstTarifasCompras = _appService.GetListTarifasBase(TipoFlujo.Compra,_db).Select(f => new SelectListItem() { Value = f.Fktarifa, Text = f.Descripcion });
            result.Ejercicios = ejerciciosService.getEjercicios(id);
            return result;
        }

        public override Empresas CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as EmpresaModel;
            var result = _db.Set<Empresas>().Create();
            result.id = viewmodel.Id;
            result.nombre = viewmodel.Nombre;
            result.razonsocial = viewmodel.Razonsocial;
            result.Fkplangeneralcontable = new Guid(viewmodel.Fkplangeneralcontable);
            result.fkPais = viewmodel.Fkpais;
            result.nif =viewmodel.Nif.Nif;
            result.fktipoidentificacion_nif = viewmodel.Nif.TipoNif;
            result.administrador = viewmodel.Administrador;
            result.nifcifadministrador = viewmodel.NifCifAdministrador.Nif;
            result.fktipoidentificacion_nifcifadministrador = viewmodel.NifCifAdministrador.TipoNif;
            result.actividadprincipal = viewmodel.ActividadPrincipal;
            result.cnae = viewmodel.Cnae;
            result.nivel = Funciones.Qint(viewmodel.Nivel);
            result.fkMonedabase = Funciones.Qint(viewmodel.FkMonedaBase);
            result.fkMonedaadicional = Funciones.Qint(viewmodel.FkMonedaAdicional);
            result.digitoscuentas = Funciones.Qint(viewmodel.DigitosCuentas);
            result.nivelcuentas = Funciones.Qint(viewmodel.NivelCuentas);
            result.cuentasanuales = viewmodel.CuentasAnuales;
            result.cuentasperdidas = viewmodel.CuentasPerdidas;
            result.cuentasexistencias = viewmodel.CuentasExistencias;
            result.cuentaPG = viewmodel.CuentaPG;
            result.cuentascaja = viewmodel.Cuentascaja;
            result.guiaanuales = viewmodel.GuiaAnuales;
            result.guiaperdidas = viewmodel.GuiaPerdidas;
            result.criterioiva = (int)viewmodel.Criterioiva;
            result.liquidacioniva = (int)viewmodel.Liquidacioniva;
            result.ivasoportado = (int)viewmodel.Ivasoportado;
            result.ivarepercutido = (int)viewmodel.Ivarepercutido;
            result.tipoempresa = viewmodel.TipoEmpresa;
            result.datosregistrales = viewmodel.Datosregistrales;
            result.fktarifascompras = viewmodel.Fktarifascompras;
            result.fktarifasventas = viewmodel.Fktarifasventas;
            result.fkregimeniva = viewmodel.Fkregimeniva;
            result.fkCuentaEntradasVariasAlmacen = viewmodel.FkCuentaEntradasVariasAlmacen;
            result.fkCuentaSalidasVariasAlmacen = viewmodel.FkCuentaSalidasVariasAlmacen;
            result.ean13 = viewmodel.Ean13;
            result.decimalesprecios = viewmodel.Decimalesprecios;
            return result;
        }

        public override Empresas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as EmpresaModel;
            var result = _db.Empresas.Find(viewmodel.Id);
            result.nombre = viewmodel.Nombre;
            result.razonsocial = viewmodel.Razonsocial;
            result.Fkplangeneralcontable = new Guid(viewmodel.Fkplangeneralcontable);
            result.fkPais = viewmodel.Fkpais;
            result.nif = viewmodel.Nif.Nif;
            result.fktipoidentificacion_nif = viewmodel.Nif.TipoNif;
            result.administrador = viewmodel.Administrador;
            result.nifcifadministrador = viewmodel.NifCifAdministrador.Nif;
            result.fktipoidentificacion_nifcifadministrador = viewmodel.NifCifAdministrador.TipoNif;
            result.actividadprincipal = viewmodel.ActividadPrincipal;
            result.cnae = viewmodel.Cnae;
            result.nivel = Funciones.Qint(viewmodel.Nivel);
            result.fkMonedabase = Funciones.Qint(viewmodel.FkMonedaBase);
            result.fkMonedaadicional = Funciones.Qint(viewmodel.FkMonedaAdicional);
            result.digitoscuentas = Funciones.Qint(viewmodel.DigitosCuentas);
            result.nivelcuentas = Funciones.Qint(viewmodel.NivelCuentas);
            result.cuentasanuales = viewmodel.CuentasAnuales;
            result.cuentasperdidas = viewmodel.CuentasPerdidas;
            result.cuentasexistencias = viewmodel.CuentasExistencias;
            result.cuentaPG = viewmodel.CuentaPG;
            result.cuentascaja = viewmodel.Cuentascaja;
            result.guiaanuales = viewmodel.GuiaAnuales;
            result.guiaperdidas = viewmodel.GuiaPerdidas;
            result.criterioiva = (int)viewmodel.Criterioiva;
            result.liquidacioniva = (int)viewmodel.Liquidacioniva;
            result.ivasoportado = (int)viewmodel.Ivasoportado;
            result.ivarepercutido = (int)viewmodel.Ivarepercutido;
            result.tipoempresa = viewmodel.TipoEmpresa;
            result.datosregistrales = viewmodel.Datosregistrales;
            result.fktarifascompras = viewmodel.Fktarifascompras;
            result.fktarifasventas = viewmodel.Fktarifasventas;
            result.fkregimeniva = viewmodel.Fkregimeniva;
            result.fkCuentaEntradasVariasAlmacen = viewmodel.FkCuentaEntradasVariasAlmacen;
            result.fkCuentaSalidasVariasAlmacen = viewmodel.FkCuentaSalidasVariasAlmacen;
            result.ean13 = viewmodel.Ean13;
            result.decimalesprecios = viewmodel.Decimalesprecios;
            return result;
        }

        public override IModelView GetModelView(Empresas obj)
        {
            var result = new EmpresaModel();
            result.Id = obj.id;
            result.Nombre = obj.nombre;
            result.Razonsocial = obj.razonsocial;
            result.Fkplangeneralcontable = obj.Fkplangeneralcontable.Value.ToString();
            result.Fkpais = obj.fkPais;
            result.Nif = new NifCifModel() {Nif=obj.nif, TipoNif = obj.fktipoidentificacion_nif };
            result.Administrador = obj.administrador;
            result.NifCifAdministrador = new NifCifModel() {Nif = obj.nifcifadministrador,TipoNif = obj.fktipoidentificacion_nifcifadministrador};
            result.ActividadPrincipal = obj.actividadprincipal;
            result.Cnae = obj.cnae;
            result.Nivel = obj.nivel.HasValue ? obj.nivel.ToString() : string.Empty;
            result.FkMonedaBase = obj.fkMonedabase.HasValue ? obj.fkMonedabase.ToString() : string.Empty;
            result.FkMonedaAdicional = obj.fkMonedaadicional.HasValue ? obj.fkMonedaadicional.ToString() : string.Empty;
            result.DigitosCuentas = obj.digitoscuentas.HasValue ? obj.digitoscuentas.ToString() : string.Empty;
            result.NivelCuentas = obj.nivelcuentas.HasValue ? obj.nivelcuentas.ToString() : string.Empty;
            result.CuentasAnuales = obj.cuentasanuales;
            result.CuentasPerdidas = obj.cuentasperdidas;
            result.CuentasExistencias = obj.cuentasexistencias;
            result.CuentaPG = obj.cuentaPG;
            result.Cuentascaja = obj.cuentascaja;
            result.GuiaAnuales = obj.guiaanuales;
            result.GuiaPerdidas = obj.guiaperdidas;
            result.Liquidacioniva = (LiquidacionIva)obj.liquidacioniva.Value;
            result.Ivarepercutido = (IvaRepercutido)obj.ivarepercutido.Value;
            result.Ivasoportado = (IvaSoportado)obj.ivasoportado.Value;
            result.Criterioiva = (CriterioIva)obj.criterioiva.Value;
            result.TipoEmpresa =obj.tipoempresa;
            result.Datosregistrales = obj.datosregistrales;
            result.Fktarifascompras = obj.fktarifascompras;
            result.Fktarifasventas = obj.fktarifasventas;
            result.Fkregimeniva = obj.fkregimeniva;
            result.FkCuentaEntradasVariasAlmacen = obj.fkCuentaEntradasVariasAlmacen;
            result.FkCuentaSalidasVariasAlmacen = obj.fkCuentaSalidasVariasAlmacen;
            result.Ean13 = obj.ean13;
            result.Decimalesprecios = obj.decimalesprecios;
            return result;
        }
    }
}
