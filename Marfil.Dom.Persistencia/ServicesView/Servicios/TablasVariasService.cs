using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITablasVariasService
    {

    }

    public class TablasVariasService : GestionService<BaseTablasVariasModel, Tablasvarias>, ITablasVariasService
    {
        #region CTR

        public TablasVariasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public BaseTablasVariasModel GetTablasVariasByType(Type tipo)
        {
            var tabla = _db.Tablasvarias.Single(f => f.clase == tipo.FullName);
            return get(tabla.id.ToString()) as BaseTablasVariasModel;
        }

        public BaseTablasVariasModel GetTablasVariasByCode(int codigo,bool general=false)
        {
            return get(codigo.ToString()) as BaseTablasVariasModel;
        }

        public IEnumerable<TablasVariasPaisesModel> GetListPaises()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByType(typeof(TablasVariasPaisesModel));
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasPaisesModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposVias()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2030);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListGrupoIncidencias()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(800);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        
        public IEnumerable<TablasVariasGeneralModel> GetListTiposObras()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(25);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListModosContacto()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2150);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        

        public IEnumerable<TablasVariasGeneralModel> GetListCanales()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(30);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMotivosDevolucion()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(10);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMotivosRectificacion()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(10);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListGruposFormasPago()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(940);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        public IEnumerable<TablasVariasGeneralModel> GetListTiposUbicacion()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(970);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        public IEnumerable<TablasVariasGeneralModel> GetListTiposDireccion()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2010);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposContacto()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2040);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        
        public IEnumerable<TablasVariasTiposNif> GetListTiposNif()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2070);
            return tablavaria.Lineas.OrderBy(f => f.Valor).Select(f => (TablasVariasTiposNif)f);
        }


        public IEnumerable<TablasVariasCargosEmpresaModel> GetListCargosEmpresa()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2050);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasCargosEmpresaModel)f);
        }

        public IEnumerable<TablasVariasTiposAlbaranesModel> GetListTiposAlbaranes()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(1010);
            return tablavaria.Lineas.OrderBy(f => f.Defecto).ThenBy(f => f.Descripcion).Select(f => (TablasVariasTiposAlbaranesModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTipoTransporte()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(930);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }


        public IEnumerable<TablasVariasGeneralModel> GetListIdiomas()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(500);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListEstadoCivil()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(960);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposEmpresas()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2023);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasMotivoBloqueoCuentasModel> GetListMotivosBloqueo()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(12);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasMotivoBloqueoCuentasModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTiposContrato()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(950);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListOperaciones340()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2060);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListFamiliaProveedor()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(1);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListDelegaciones()
        {

            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(400);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListFamiliaMateriales()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(20);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        /*
        public IEnumerable<TablasVariasGeneralModel> GetListGrupoMateriales()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(21);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        */

        public IEnumerable<TablasVariasGeneralModel> GetListLabores()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(55);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TipoInforme> GetListTipoInforme()
        {
            return _db.TipoInforme.ToList();
        }
        public IEnumerable<TipoGuia> GetListTipoGuia()
        {
            return _db.TipoGuia.ToList();
        }

        public IEnumerable<TablasVariasGeneralModel> GetListZonaClienteProveedor()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        public IEnumerable<TablasVariasGeneralModel> GetTiposDocumentos()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(2120);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        #region Contabilidad
        public IEnumerable<TablasVariasGeneralModel> GetTiposDocumentosContables()
        {

            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2121);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        
        public IEnumerable<TablasVariasGeneralModel> GetTiposAsientosContables()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(72);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListComentariosAsientos()
        {
            var service = new TablasVariasService(_context, _db);
            var comentariosasientos = service.GetTablasVariasByCode(70);
            return comentariosasientos.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        
        public string GetComentarioAsiento(string valor)
        {
            var service = new TablasVariasService(_context, _db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(70);
            string myDescripcion;
            myDescripcion = getTablasVariasByCode.Lineas.Where(f => f.Valor == valor)
                            .Select(f => f.valor).ToString();
            return myDescripcion;
        }

        public IEnumerable<TablasVariasGeneralModel> GetListDescripcionAsientos()
        {
            var service = new TablasVariasService(_context, _db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(76);
            return getTablasVariasByCode.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public static IEnumerable<TablasVariasGeneralModel> GetListDescripcionAsientos(IContextService context, MarfilEntities db = null)
        {
            var service = new TablasVariasService(context, db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(76);
            return getTablasVariasByCode.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        // ver si se usa
        public string GetDescripcionAsiento(string valor)
        {
            var service = new TablasVariasService(_context, _db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(76);
            string myDescripcion;
            myDescripcion = getTablasVariasByCode.Lineas.Where(f => f.Valor == valor)
                            .Select(f => f.Valor).ToString();
            return myDescripcion;
        }

        public IEnumerable<TablasVariasTiposAsientosModel> GetListTiposAsientos()
        {
            var service = new TablasVariasService(_context, _db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(72);
            return getTablasVariasByCode.Lineas.OrderByDescending(f => f.Defecto).ThenBy(d => d.Descripcion).Select(f => (TablasVariasTiposAsientosModel)f);            
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCanalContable()
        {
            var service = new TablasVariasService(_context, _db);
            var getTablasVariasByCode = service.GetTablasVariasByCode(2080);
            return getTablasVariasByCode.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        #endregion Contabilidad

        public IEnumerable<TablasVariasGeneralModel> GetListUnidadNegocio()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(15);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListIncoterm()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(5);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListSituacionComision()
        {

            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(50);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }
        

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new[] { "Noeditable", "Lineas", "Clase","Toolbar","Context" };
            model.List = model.List.Select(f => (BaseTablasVariasModel)f).OrderBy(f => f.Nombre);
            model.FiltroColumnas.Add("Id", FiltroColumnas.EmpiezaPor);
            model.Properties = Helpers.Helper.getProperties<BaseTablasVariasModel>();
            return model;
        }

        public IEnumerable<TablasVariasGeneralModel> GetListCalificacionComercial()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(710);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTipograno()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(700);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTono()
        {
            var service = new TablasVariasService(_context,_db);
            var tablavaria = service.GetTablasVariasByCode(705);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListTipoFactura()
        {
            var service = new TablasVariasService(_context, _db);
            var tiposfacturas = service.GetTablasVariasByCode(2088);
            return tiposfacturas.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListRegimenEspecialEmitidas()
        {
            var service = new TablasVariasService(_context, _db);
            var regimenesespecialesemitidas = service.GetTablasVariasByCode(2090);
            return regimenesespecialesemitidas.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListRegimenEspecialRecibidas()
        {
            var service = new TablasVariasService(_context, _db);
            var regimenesespecialrecibidas = service.GetTablasVariasByCode(2092);
            return regimenesespecialrecibidas.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListGrupoSecciones()
        {
            var service = new TablasVariasService(_context, _db);
            var gruposecciones = service.GetTablasVariasByCode(71);
            return gruposecciones.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }


        public IEnumerable<TablasVariasGeneralModel> GetListSeccionesProduccion()
        {
            var service = new TablasVariasService(_context, _db);
            var gruposecciones = service.GetTablasVariasByCode(80);
            return gruposecciones.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListAcciones()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2130);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListReacciones()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2140);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetListMargenTiempo()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2170);
            return tablavaria.Lineas.OrderBy(f => f.Descripcion).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetCRMValores()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2130);
            return tablavaria.Lineas.OrderBy(f => f.Valor).Select(f => (TablasVariasGeneralModel)f);
        }

        public IEnumerable<TablasVariasGeneralModel> GetClasificacionArticulos()
        {
            var service = new TablasVariasService(_context, _db);
            var tablavaria = service.GetTablasVariasByCode(2105);
            return tablavaria.Lineas.OrderBy(f => f.Valor).Select(f => (TablasVariasGeneralModel)f);
        }
    }
}
