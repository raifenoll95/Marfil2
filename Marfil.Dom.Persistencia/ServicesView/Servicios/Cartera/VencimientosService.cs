using System;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Vencimientos
{

    public interface IVencimientosService
    {
        DateTime fechadocumento { get; }
        DateTime fechavencimiento { get; set; }
        int diasvencimiento { get;}
        int dia1 { get; }
        int dia2 { get; }
        double? importevencimiento { get; set; }
        bool excluirfestivos { get; }
    }

    public class VencimientosService : IVencimientosService //: GestionService<FacturasModel, Facturas>, IDocumentosServices,IBuscarDocumento, IDocumentosVentasPorReferencia<FacturasModel>, IVencimientosService
    {
        #region Member
        private DateTime _fechadocumento;
        private DateTime _fechavencimiento;
        private int _diasvencimiento;
        private int _dia1;
        private int _dia2;
        private double? _importevencimiento;
        private bool _excluirfestivos;
        #endregion

        #region Properties

        public DateTime fechadocumento
        {
            get { return _fechadocumento; }
        //    set
        //    {
        //        _ejercicioId = value;
        //        ((FacturasValidation)_validationService).EjercicioId = value;
        //    }
        }

        public DateTime fechavencimiento
        {
            get { return _fechavencimiento; }
            set
                {
                    _fechavencimiento = value;
                }
        }

        public int diasvencimiento
        {
            get { return _diasvencimiento; }
        }

        public int dia1
        {
            get { return _dia1; }
        }

        public int dia2
        {
            get { return _dia2; }
        }


        public double? importevencimiento
        {
            get { return _importevencimiento; }
            set
            {
                _importevencimiento = value;
            }
        }

        public bool excluirfestivos
        {
            get { return _excluirfestivos; }
        }



        #endregion

        public VencimientosService(DateTime fechadocumento, DateTime fechavencimiento,  int diasvencimiento, int dia1, int dia2, bool excluirfestivos)
        {
            _fechadocumento = fechadocumento;
            _fechavencimiento = fechavencimiento;
            _diasvencimiento = diasvencimiento;
            _dia1 = dia1;
            _dia2 = dia2;
            _excluirfestivos = excluirfestivos;
        }

        public DateTime GetFechavencimiento(DateTime fechafactura, int diavencimiento, int diapago1, int diapago2, bool excluirfestivos)
        {
            var resultado = fechafactura.AddDays(diavencimiento);

            if (diapago1 == 0 && diapago2 == 0)
            {
                if (excluirfestivos)
                {
                    var increment = 0;
                    if (resultado.DayOfWeek == DayOfWeek.Sunday)
                    {
                        increment = 1;
                    }
                    else if (resultado.DayOfWeek == DayOfWeek.Saturday)
                    {
                        increment = 2;
                    }
                    if (increment > 0)
                        resultado = resultado.AddDays(increment);
                }
            }
            else
            {
                var dia1 = diapago1 < diapago2 ? diapago1 : diapago2;
                var dia2 = diapago1 < diapago2 ? diapago2 : diapago1;

                if (dia1 > DateTime.DaysInMonth(resultado.Year, resultado.Month))
                {
                    dia1 = DateTime.DaysInMonth(resultado.Year, resultado.Month);
                }
                if (dia2 > DateTime.DaysInMonth(resultado.Year, resultado.Month))
                {
                    dia2 = DateTime.DaysInMonth(resultado.Year, resultado.Month);
                }

                if (resultado.Day <= dia1)
                {
                    //asignar al dia1
                    resultado = new DateTime(resultado.Year, resultado.Month, dia1);
                }
                else if (resultado.Day <= dia2)
                {
                    //asignar al dia2
                    resultado = new DateTime(resultado.Year, resultado.Month, dia2);
                }
                else if (resultado.Day > dia2)
                {
                    //asignar al dia1 del siguiente mes
                    var aux = new DateTime(resultado.Year, resultado.Month, dia1);
                    resultado = aux.AddMonths(1);
                }
            }

            return resultado;
        }


    }
}
