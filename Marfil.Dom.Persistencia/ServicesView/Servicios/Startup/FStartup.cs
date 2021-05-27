using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public class FStartup:IDisposable
    {
        private readonly Dictionary<string,IStartup>  _dictionary=new Dictionary<string, IStartup>();
        private readonly MarfilEntities _db;
        private readonly IContextService _context;
        public FStartup(IContextService context,MarfilEntities db)
        {
            _db = db;
            _context = context;
            _dictionary.Add("2110", new EstadosStockStartup(context,db));
            _dictionary.Add("3166", new PaisesStartup(context,db));
            _dictionary.Add("2000", new ModosPagoStartup(context, db));
            _dictionary.Add("2050", new CargosEmpresaStartup(context, db));
            _dictionary.Add("2070", new TiposNifStartup(context, db));
            _dictionary.Add("1010", new TiposAlbaranesStartup(context, db));
            _dictionary.Add("2120", new TiposDocumentosStartup(context, db));
            _dictionary.Add("72", new TiposAsientosStartup(context, db));
            _dictionary.Add("2121", new TiposDocumentosContablesStartup(context, db));
            _dictionary.Add("12", new MotivosBloqueoCuentasStartup(context, db));
            _dictionary.Add("tablasvarias", new TablaVariaStartup(context, this,db));
            _dictionary.Add("monedas", new MonedasStartup(context, db));
            _dictionary.Add("bancos", new BancosStartup(context, db));
            _dictionary.Add("cabecera", new TablasVariasCabecerasStartup(context, db));
            _dictionary.Add("municipios", new MunicipiosStartup(context, db));
            _dictionary.Add("provincias", new ProvinciasStartup(context, db));
            _dictionary.Add("plangeneral", new PlanesGeneralesStartup(context, db));
            _dictionary.Add("tarifas", new TarifasbaseStartup(context, db));
            _dictionary.Add("estados", new EstadosdocumentosStartup(context, db));
            _dictionary.Add("documentos", new DocumentosStartup(context, db));
            //_dictionary.Add("agrupacion", new DatosgeneralesaplicacionStartup(context, db));
            _dictionary.Add("criteriosagrupacion", new CriteriosAgrupacionStartup(context, db));
            _dictionary.Add("unidades", new UnidadesStartup(context, db));
            _dictionary.Add("situacionestesoreria", new SituacionesTesoreriaStartup(context, db));
            _dictionary.Add("circuitostesoreria", new CircuitosTesoreriaStartup(context, db));
        }

        public IStartup CreateService( string id)
        {
            return _dictionary.ContainsKey(id) ? _dictionary[id] : new GenericTablaVariaStartup(_context,_db);
        }

        public void Dispose()
        {

        }
    }
}
