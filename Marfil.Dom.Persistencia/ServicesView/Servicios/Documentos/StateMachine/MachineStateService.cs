using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine
{
    public class MachineStateService
    {


        public enum Command
        {
            Iniciar,
            Ponerenmarcha,
            Finalizar,
            Anular,
            Caducar
        }


        class StateTransition
        {
            readonly TipoEstado CurrentState;
            readonly Command Command;

            public StateTransition(TipoEstado currentState, Command command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        //Dictionary<StateTransition, TipoEstado> transitions;
        private List<Tuple<TipoEstado, TipoEstado>> _estados;
        public TipoEstado CurrentState { get; private set; }

        public MachineStateService()
        {
            CurrentState = TipoEstado.Diseño;
            /*transitions = new Dictionary<StateTransition, TipoEstado>
            {
                //diseño
                { new StateTransition(TipoEstado.Diseño, Command.Iniciar), TipoEstado.Diseño },
                { new StateTransition(TipoEstado.Diseño, Command.Ponerenmarcha), TipoEstado.Curso },
                { new StateTransition(TipoEstado.Diseño, Command.Anular), TipoEstado.Anulado },
                { new StateTransition(TipoEstado.Diseño, Command.Caducar), TipoEstado.Caducado },
                //progreso
                { new StateTransition(TipoEstado.Curso, Command.Iniciar), TipoEstado.Diseño },
                { new StateTransition(TipoEstado.Curso, Command.Ponerenmarcha), TipoEstado.Curso },
                { new StateTransition(TipoEstado.Curso, Command.Finalizar), TipoEstado.Finalizado },
                { new StateTransition(TipoEstado.Curso, Command.Anular), TipoEstado.Anulado },
                { new StateTransition(TipoEstado.Curso, Command.Caducar), TipoEstado.Caducado },
                //Finalizado
                { new StateTransition(TipoEstado.Finalizado, Command.Ponerenmarcha), TipoEstado.Curso },
                { new StateTransition(TipoEstado.Finalizado, Command.Finalizar), TipoEstado.Finalizado },
                { new StateTransition(TipoEstado.Finalizado, Command.Anular), TipoEstado.Anulado },
                { new StateTransition(TipoEstado.Finalizado, Command.Caducar), TipoEstado.Caducado },
                //Anulado
                { new StateTransition(TipoEstado.Anulado, Command.Iniciar), TipoEstado.Diseño },
                { new StateTransition(TipoEstado.Anulado, Command.Anular), TipoEstado.Anulado },
                { new StateTransition(TipoEstado.Anulado, Command.Caducar), TipoEstado.Caducado },
                //Caducado
                { new StateTransition(TipoEstado.Caducado, Command.Iniciar), TipoEstado.Diseño },
                { new StateTransition(TipoEstado.Caducado, Command.Anular), TipoEstado.Anulado },
                { new StateTransition(TipoEstado.Caducado, Command.Caducar), TipoEstado.Caducado }
            };*/

            _estados = new List<Tuple<TipoEstado, TipoEstado>>
            {
                //diseño
                Tuple.Create( TipoEstado.Diseño, TipoEstado.Diseño ),
                 Tuple.Create( TipoEstado.Diseño, TipoEstado.Curso ),
                 Tuple.Create( TipoEstado.Diseño, TipoEstado.Anulado ),
                 Tuple.Create( TipoEstado.Diseño, TipoEstado.Caducado ),
                //progreso
                 Tuple.Create( TipoEstado.Curso, TipoEstado.Diseño ),
                 Tuple.Create( TipoEstado.Curso, TipoEstado.Curso ),
                 Tuple.Create( TipoEstado.Curso, TipoEstado.Finalizado ),
                 Tuple.Create( TipoEstado.Curso, TipoEstado.Anulado ),
                 Tuple.Create( TipoEstado.Curso, TipoEstado.Caducado ),
                //Finalizado
                 Tuple.Create( TipoEstado.Finalizado, TipoEstado.Curso ),
                 Tuple.Create( TipoEstado.Finalizado, TipoEstado.Finalizado ),
                 Tuple.Create( TipoEstado.Finalizado, TipoEstado.Anulado ),
                 Tuple.Create( TipoEstado.Finalizado, TipoEstado.Caducado ),
                //Anulado
                 Tuple.Create( TipoEstado.Anulado, TipoEstado.Diseño ),
                 Tuple.Create( TipoEstado.Anulado, TipoEstado.Anulado ),
                 Tuple.Create( TipoEstado.Anulado, TipoEstado.Caducado ),
                //Caducado
                 Tuple.Create( TipoEstado.Caducado, TipoEstado.Diseño ),
                 Tuple.Create( TipoEstado.Caducado, TipoEstado.Anulado ),
                 Tuple.Create( TipoEstado.Caducado, TipoEstado.Caducado )
            };
        }

        /* public TipoEstado GetNext(Command command)
         {
             StateTransition transition = new StateTransition(CurrentState, command);
             TipoEstado nextState;
             if (!transitions.TryGetValue(transition, out nextState))
                 throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
             return nextState;
         }

         public TipoEstado MoveNext(Command command)
         {
             CurrentState = GetNext(command);
             return CurrentState;
         }*/

        public void SetState(IDocumentosServices service, IDocument model, EstadosModel nuevoEstado)
        {
            var obj = model as IModelView;
            var objState = model as IDocumentState;

            if (!_estados.Any(f => f.Item1 == objState.Tipoestado(obj.get("Context") as IContextService) && f.Item2 == nuevoEstado.Tipoestado))
                throw new ValidationException("Invalid transition");

            objState.Fkestados = nuevoEstado.CampoId;

            service.SetEstado(obj, nuevoEstado);
        }

        public async Task SetStateAsync(IDocumentosServices service, IDocument model, EstadosModel nuevoEstado)
        {
            var obj = model as IModelView;
            var objState = model as IDocumentState;

            if (!_estados.Any(f => f.Item1 == objState.Tipoestado(obj.get("Context") as IContextService) && f.Item2 == nuevoEstado.Tipoestado))
                throw new ValidationException("Invalid transition");

            objState.Fkestados = nuevoEstado.CampoId;

            await service.SetEstadoAsync(obj, nuevoEstado);
        }

        public IEnumerable<TipoEstado> GetStatesFromState(TipoEstado estado)
        {
            return _estados.Where(f => f.Item1 == estado).Select(f => f.Item2);
        }
    }

}
