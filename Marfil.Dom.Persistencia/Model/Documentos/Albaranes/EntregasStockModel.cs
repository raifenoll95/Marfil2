using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    public class EntregasStockModel:AlbaranesModel
    {
        public EntregasStockModel()
        {
            Modo= ModoAlbaran.Constock;
        }

        public EntregasStockModel(IContextService context) : base(context)
        {
            Modo = ModoAlbaran.Constock;
        }

        public EntregasStockModel(AlbaranesModel model):base(model.Context)
        {
          
            var properties = model.GetType().GetProperties();
            foreach (var p in properties)
            {
                if(p.CanWrite && p.CanRead)
                    p.SetValue(this,p.GetValue(model));
            }

            Lineas = model.Lineas;
            Totales = model.Totales;
            
            Modo = ModoAlbaran.Constock;
        }
    }

    public class SaldarPedidosModel
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public double Cantidadpedida { get; set; }
        public string Largo { get; set; }
        public string Ancho { get; set; }
        public string Grueso { get; set; }
        public string Metros { get; set; }
        public double Cantidadalbaran { get; set; }
        public string Metrosalbaran { get; set; }
        public double Cantidadpendiente { get; set; } 
    }

    public class OperacionSaldarPedidosModel
    {
        [Required]
        public string Fkpedidos { get; set; }
        [Required]
        public string Referenciaentrega { get; set; }
        public List<SaldarPedidosModel> Lineas { get; set; } 
    }
}
