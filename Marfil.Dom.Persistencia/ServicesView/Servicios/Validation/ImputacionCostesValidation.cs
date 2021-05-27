using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ImputacionCostesValidation : BaseValidation<ImputacionCostes>
    {
        public string EjercicioId { get; set; }

        public ImputacionCostesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(ImputacionCostes model)
        {
            ValidarEstado(model); //Los finalizados no se pueden modificar
            return true;
        }

        private void ValidarEstado(ImputacionCostes model)
        {
            //Miramos si anteriormente habia ya uno en curso
            var estadosService = FService.Instance.GetService(typeof(EstadosModel), Context, _db) as EstadosService;
            var listestados = estadosService.GetStates(DocumentoEstado.DivisionesLotes); //Lista de estados
            var estadoFinalizado = listestados.First(f => f.Tipoestado == TipoEstado.Finalizado);      
            var fkestadosantiguo = _db.ImputacionCostes.Where(f => f.id == model.id).Select(f => f.fkestados).SingleOrDefault();

            //fkestados antiguo debe tener algo y estar finalizado
            if(!string.IsNullOrEmpty(fkestadosantiguo))
            {
                if (fkestadosantiguo == estadoFinalizado.CampoId)
                {
                    string message;
                    if (!_appService.ValidarEstado(model.fkestados, _db, out message))
                        throw new ValidationException(message);
                }
            }
        }
    }
}
