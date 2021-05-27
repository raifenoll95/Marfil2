using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Marfil.Dom.Persistencia.Model;
using System.Data.Entity.Infrastructure;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IDireccionesService
    {

    }

    public class DireccionesService  : GestionService<DireccionesLinModel, Direcciones>, IDireccionesService
    {
        public const char Separator = ';';

        #region CTR

        public DireccionesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region Override

        public override bool exists(string Id)
        {
            var vector = Id.Split(Separator);
            var empresa = Empresa;
            var tipotercero = Funciones.Qint(vector[0]).Value;
            var fkentidad = vector[1];
            var id = Funciones.Qint(vector[2]).Value;

            return
                _db.Set<Direcciones>()
                    .Any(
                        f =>
                            f.empresa == empresa && f.fkentidad == fkentidad && f.tipotercero == tipotercero && f.id ==id);
        }

        #endregion

        #region Api
       
        public IEnumerable<DireccionesLinModel> GetDirecciones(string empresa, int tipo,string fkCuentas)
        {
            var tipoInt = tipo;
            var list = _db.Direcciones.Where(f => f.empresa == empresa && f.fkentidad == fkCuentas && f.tipotercero == tipo).ToList();
            List<DireccionesLinModel> direcciones = new List<DireccionesLinModel>();

            foreach(var d in list)
            {
                direcciones.Add(new DireccionesLinModel
                {
                    Empresa = d.empresa,
                    Tipotercero = d.tipotercero,
                    Fkentidad = d.fkentidad,
                    Id = d.id,
                    Defecto = d.defecto.Value,
                    Descripcion = d.descripcion,
                    Fktipovia = d.fktipovia,
                    Direccion = d.direccion,
                    Cp = d.cp,
                    Fkpais = d.fkpais,
                    Fkprovincia = d.fkprovincia,
                    Poblacion = d.poblacion,
                    Personacontacto = d.personacontacto,
                    Telefono = d.telefono,
                    Telefonomovil = d.telefonomovil,
                    Fax = d.fax,
                    Email = d.email,
                    Web = d.web,
                    Notas = d.notas,
                    Fktipodireccion = d.fktipodireccion
                }) ;
            }

            //Hacemos uso de los servicios creados para las vistas para poder capturar del XML de las tablas varias un campo (nombre)
            using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                foreach (var item in direcciones)
                {
                    item.Pais = service.GetListPaises().Where(f => f.Valor == item.Fkpais).Select(f => f.Descripcion).SingleOrDefault();
                    item.Provincia = _db.Provincias.Where(f => f.codigopais == item.Fkpais && f.id == item.Fkprovincia).Select(f => f.nombre).SingleOrDefault();
                }
            }    

            return direcciones;
        }

        public IEnumerable<DireccionesLinModel> GetDirecciones(string empresa, TiposCuentas tipo, string fkCuentas)
        {
            return GetDirecciones(empresa, (int) tipo, fkCuentas);
        }

        public void CleanAllDirecciones(string empresa, int tipoInt, string fkCuentas)
        {
            var list =
             _db.Set<Direcciones>()
                 .Where(f => f.empresa == empresa && f.tipotercero == tipoInt && f.fkentidad == fkCuentas)
                 .ToList();


            foreach (var item in list)
            {
                _db.Set<Direcciones>().Remove(item);
            }
            _db.SaveChanges();
        }

        public void CleanAllDirecciones(string empresa, TiposCuentas tipo, string fkCuentas)
        {
            var tipoInt = (int)tipo;
            CleanAllDirecciones(empresa, tipoInt, fkCuentas);
        }

        #endregion

        //Te devuelve una direccion a partir del numero de la cuenta
        public DireccionesLinModel getDireccion(string numcuenta)
        {
            DireccionesLinModel d = new DireccionesLinModel(_context);
            var appService = new ApplicationHelper(_context);
            var _paises = appService.GetListPaises();

            var cuentasService = new CuentasService(_context);
            var clientesService = new ClientesService(_context);
            var proveedoresService = new ProveedoresService(_context);
            var acreedoresService = new AcreedoresService(_context);
            var tesoreriaService = new CuentastesoreriaService(_context);
            var transportistasService = new TransportistasService(_context);
            var agentesService = new AgentesService(_context);
            var aseguradorasService = new AseguradorasService(_context);
            var operariosService = new OperariosService(_context);
            var comercialesService = new ComercialesService(_context);
            var prospectosService = new ProspectosService(_context);

            var listaTerceros = cuentasService.GetCuentas(TiposCuentas.Todas).ToList();
            var listaClientes = cuentasService.GetCuentas(TiposCuentas.Clientes).ToList();
            var listaProveedores = cuentasService.GetCuentas(TiposCuentas.Proveedores).ToList();
            var listaAcreedores = cuentasService.GetCuentas(TiposCuentas.Acreedores).ToList();
            var listaTesoreria = cuentasService.GetCuentas(TiposCuentas.Cuentastesoreria).ToList();
            var listaTransportistas = cuentasService.GetCuentas(TiposCuentas.Transportistas).ToList();
            var listaAgentes = cuentasService.GetCuentas(TiposCuentas.Agentes).ToList();
            var listaAseguradoras = cuentasService.GetCuentas(TiposCuentas.Aseguradoras).ToList();
            var listaOperarios = cuentasService.GetCuentas(TiposCuentas.Operarios).ToList();
            var listaComerciales = cuentasService.GetCuentas(TiposCuentas.Comerciales).ToList();
            var listaProspectos = cuentasService.GetCuentas(TiposCuentas.Prospectos).ToList();

            if(listaClientes.Any(f => f.Id == numcuenta))
            {
                var clienteModel = clientesService.get(numcuenta) as ClientesModel;
                d.Empresa = clienteModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = clienteModel.Descripcion;
                d.Direccion = clienteModel.Direccion;
                d.Poblacion = clienteModel.Poblacion;
                d.Provincia = clienteModel.Provincia;
                d.Pais = clienteModel.Pais;
                d.Email = clienteModel.Email;
                d.Telefono = clienteModel.Telefono;
            }

            else if (listaProveedores.Any(f => f.Id == numcuenta))
            {
                var proveedorModel = proveedoresService.get(numcuenta) as ProveedoresModel;
                d.Empresa = proveedorModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = proveedorModel.Descripcion;
                d.Direccion = proveedorModel.Direccion;
                d.Poblacion = proveedorModel.Poblacion;
                d.Provincia = proveedorModel.Provincia;
                d.Pais = proveedorModel.Pais;
                d.Email = proveedorModel.Email;
                d.Telefono = proveedorModel.Telefono;
            }

            else if (listaAcreedores.Any(f => f.Id == numcuenta))
            {
                var acreedorModel = acreedoresService.get(numcuenta) as AcreedoresModel;
                d.Empresa = acreedorModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = acreedorModel.Descripcion;
                d.Direccion = acreedorModel.Direccion;
                d.Poblacion = acreedorModel.Poblacion;
                d.Provincia = acreedorModel.Provincia;
                d.Pais = acreedorModel.Pais;
                d.Email = acreedorModel.Email;
                d.Telefono = acreedorModel.Telefono;
            }

            else if (listaTesoreria.Any(f => f.Id == numcuenta))
            {
                var tesoreriaModel = tesoreriaService.get(numcuenta) as CuentastesoreriaModel;
                d.Empresa = tesoreriaModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = tesoreriaModel.Descripcion;
                d.Direccion = tesoreriaModel.Direccion;
                d.Poblacion = tesoreriaModel.Poblacion;
                d.Provincia = tesoreriaModel.Provincia;
                d.Pais = tesoreriaModel.Pais;
                d.Email = tesoreriaModel.Email;
                d.Telefono = tesoreriaModel.Telefono;
            }

            else if (listaTransportistas.Any(f => f.Id == numcuenta))
            {
                var transportistaModel = transportistasService.get(numcuenta) as TransportistasModel;
                d.Empresa = transportistaModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = transportistaModel.Descripcion;
                d.Direccion = transportistaModel.Direccion;
                d.Poblacion = transportistaModel.Poblacion;
                d.Provincia = transportistaModel.Provincia;
                d.Pais = transportistaModel.Pais;
                d.Email = transportistaModel.Email;
                d.Telefono = transportistaModel.Telefono;
            }

            else if (listaAgentes.Any(f => f.Id == numcuenta))
            {
                var agenteModel = agentesService.get(numcuenta) as AgentesModel;
                d.Empresa = agenteModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = agenteModel.Descripcion;
                d.Direccion = agenteModel.Direccion;
                d.Poblacion = agenteModel.Poblacion;
                d.Provincia = agenteModel.Provincia;
                d.Pais = agenteModel.Pais;
                d.Email = agenteModel.Email;
                d.Telefono = agenteModel.Telefono;
            }

            else if (listaAseguradoras.Any(f => f.Id == numcuenta))
            {
                var aseguradoraModel = aseguradorasService.get(numcuenta) as AseguradorasModel;
                d.Empresa = aseguradoraModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = aseguradoraModel.Descripcion;
                d.Direccion = aseguradoraModel.Direccion;
                d.Poblacion = aseguradoraModel.Poblacion;
                d.Provincia = aseguradoraModel.Provincia;
                d.Pais = aseguradoraModel.Pais;
                d.Email = aseguradoraModel.Email;
                d.Telefono = aseguradoraModel.Telefono;
            }

            else if (listaComerciales.Any(f => f.Id == numcuenta))
            {
                var comercialModel = comercialesService.get(numcuenta) as ComercialesModel;
                d.Empresa = comercialModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = comercialModel.Descripcion;
                d.Direccion = comercialModel.Direccion;
                d.Poblacion = comercialModel.Poblacion;
                d.Provincia = comercialModel.Provincia;
                d.Pais = comercialModel.Pais;
                d.Email = comercialModel.Email;
                d.Telefono = comercialModel.Telefono;
            }

            else if (listaOperarios.Any(f => f.Id == numcuenta))
            {
                var operarioModel = operariosService.get(numcuenta) as OperariosModel;
                d.Empresa = operarioModel.Empresa;
                d.Fkentidad = numcuenta;
                d.Descripcion = operarioModel.Descripcion;
                d.Direccion = operarioModel.Direccion;
                d.Poblacion = operarioModel.Poblacion;
                d.Provincia = operarioModel.Provincia;
                d.Pais = operarioModel.Pais;
                d.Email = operarioModel.Email;
                d.Telefono = operarioModel.Telefono;
            }

            return d;
        }
    }
}
