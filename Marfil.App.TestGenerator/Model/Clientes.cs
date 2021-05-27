using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.App.TestGenerator.Model
{
    internal class Clientes
    {
        public bool EsProspecto { get { return false; } }

        
        public string Empresa { get; set; }

        
        public string Fkcuentas { get; set; }

        public CuentasModel Cuentas { get; set; }

        
        public string Descripcion { get { return Cuentas?.Descripcion; } }

        
        public string RazonSocial { get { return Cuentas?.Descripcion2; } }

        
        public string Nif
        {
            get { return Cuentas?.Nif?.Nif; }
        }

        

        public bool Bloqueado
        {
            get { return Cuentas?.Bloqueado ?? false; }
        }

        
        public string Fkidiomas { get; set; }

        
        public string Fkfamiliacliente { get; set; }

        
        public string Fkzonacliente { get; set; }

        
        public string Fktipoempresa { get; set; }

        
        public string Fkunidadnegocio { get; set; }

        
        public string Fkincoterm { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        
        public int Fkmonedas { get; set; }

        public int? Moneda { get { return Fkmonedas; } }

        
        public string Fkregimeniva { get; set; }

        
        public string Fkgruposiva { get; set; }

        public CriterioIVA Criterioiva { get; set; }

        public string Fktiposretencion { get; set; }

        public string Fktransportistahabitual { get; set; }

        public Tipoportes? Tipodeportes { get; set; }

        public string Cuentatesoreria { get; set; }

        public int Fkformaspago { get; set; }
        
        public double Descuentoprontopago { get; set; }

        public double Descuentocomercial { get; set; }
        
        public int Diafijopago1 { get; set; }
        
        public int Diafijopago2 { get; set; }
        
        public string Periodonopagodesde { get; set; }
        
        public string Periodonopagohasta { get; set; }

        public string Notas { get; set; }
        
        public string Fktarifas { get; set; }
        
        public int Numerocopiasfactura { get; set; }

        public string Fkcuentasagente { get; set; }
        
        public string Fkcuentascomercial { get; set; }
        
        public string Perteneceagrupo { get; set; }
        
        public string Fkcuentasaseguradoras { get; set; }
        
        public string Suplemento { get; set; }
        
        public double Porcentajeriesgocomercial { get; set; }
        
        public double Porcentajeriesgopolitico { get; set; }
        
        public int Riesgoconcedidoempresa { get; set; }

        public int Riesgosolicitado { get; set; }

        public int Riesgoaseguradora { get; set; }
        
        public int Diascondecidos { get; set; }
    }
}
