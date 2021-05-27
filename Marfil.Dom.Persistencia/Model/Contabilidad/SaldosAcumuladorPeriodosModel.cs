using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    public class SaldosAcumuladorPeriodosModel : BaseModel<SaldosAcumuladorPeriodosModel,Persistencia.SaldosAcomuladosPeriodos>
    {
        #region Propiedades
        public string Empresa { get; set; }
        public int fkEjercicio { get; set; }
        public bool Ejercicio { get; set; }
        public bool Existencia { get; set; }
        public bool Grupos { get; set; }
        public bool CierreEjercicio { get; set; }
        public bool IncluirAsientosSimulacion { get; set; }
        public bool ExcluirAsientosSimulacion { get; set; }
        public bool IncluirAjusteExistenciaPeriodo { get; set; }
        public bool IncluirProrrateoAmortizaciones { get; set; }
        public List<SaldosAcumuladosPeriodosLin> AcumuladosPeriodosLins { get; set; }
        #endregion
        public SaldosAcumuladorPeriodosModel()
        {

        }
        public SaldosAcumuladorPeriodosModel(IContextService context) : base(context)
        {

        }

        public override string DisplayName => throw new NotImplementedException();

        public override object generateId(string id)
        {
            throw new NotImplementedException();
        }
    }
}
