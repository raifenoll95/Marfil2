using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public class CuentasStartup : IStartup, IDisposable
    {
        #region Members

        private readonly GestionService<CuentasModel, Cuentas> _cuentasServices;
        private readonly string _empresa;
        private readonly string _pais;
        private IContextService _context;

        #endregion

        public CuentasStartup(IContextService context,MarfilEntities db,string empresa,string pais)
        {
            _context = context;
            _cuentasServices = FService.Instance.GetService(typeof(CuentasModel), context, db) as CuentasService;
            _empresa = empresa;
            _pais = pais;
            
        }

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearModels(contenido);
            }
        }

        private void CrearModels(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                  if (!string.IsNullOrEmpty(item))
                    CrearModel(item);
            }

        }

        private void CrearModel(string linea)
        {
            var vector = linea.Split(';');
            var model = new CuentasModel()
            {
                Empresa = _empresa,
                Id = vector[0],
                Descripcion2 = vector[1],
                Descripcion = vector[2],
                Nivel = int.Parse(vector[3]),
                FkPais = _pais,
                UsuarioId  = Guid.Empty.ToString(),
                Nif = new NifCifModel(),
                Fechaalta = DateTime.Now,
                FechaModificacion = DateTime.Now
            };
           
            _cuentasServices.create(model);
        }

        public void Dispose()
        {
            _cuentasServices?.Dispose();
        }
    }
}
