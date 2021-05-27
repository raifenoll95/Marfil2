using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    public interface IDocumentosVentasPorReferencia<T>
    {
        T GetByReferencia(string referencia);
    }

    public interface IDocumentosServices
    {
        void SetEstado(IModelView model, EstadosModel nuevoEstado);
    }
    public interface IGestionService:IDisposable 
    {
        string Empresa { get; set; }
        List<string> WarningList { get; }
        ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller);
        EmpresaModel get(object empresa);
        IEnumerable<IModelView> getAll();
        IEnumerable<T> GetAll<T>() where T : IModelView;
        IModelView get(string id);
        void create(IModelView obj);
        void edit(IModelView obj);
        void delete(IModelView obj);
        bool exists(string id);
        string FirstRegister();
        string LastRegister();
        string NextRegister(string id);
        string PreviousRegister(string id);
        string GetSelectPrincipal();
       

    }
}
