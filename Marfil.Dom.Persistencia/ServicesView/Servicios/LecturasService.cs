using Marfil.Dom.Persistencia.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class LecturasService : GestionService<LecturasModel, Lecturas>
    {

        #region CTR

        public LecturasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region Importar

        public void Importar(DataTable dt, int idPeticion, IContextService context)
        {
            string errores = "";
            List<LecturasModel> Lista = new List<LecturasModel>();


            foreach (DataRow row in dt.Rows)
            {
                LecturasModel lectura = new FModel().GetModel<LecturasModel>(context);

                var identificador = row["Identificador"].ToString();
                var hora = row["Hora"].ToString();
                var lote = row["Lote"].ToString();
                var cantidad = 0;
                if (row["Cantidad"].ToString() != "" && row["Cantidad"].ToString() != null)
                {
                    cantidad = int.Parse(row["Cantidad"].ToString());
                }
                else
                {
                    cantidad = 1;
                }
                
                var usuario = Usuarioid;

                var existe = _db.Lecturas.Where(f => f.identificador == identificador && f.usuario == usuario && f.lote == lote).SingleOrDefault();

                if (existe == null){

                    lectura.Identificador = identificador;
                    DateTime fecha;
                    if (DateTime.TryParseExact(row["Fecha"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                    {
                        lectura.Fecha = fecha;
                    }
                    lectura.Hora = hora;
                    lectura.Lote = lote;
                    lectura.Cantidad = cantidad;
                    lectura.Usuario = usuario;
                    lectura.Insertado = false;

                    Lista.Add(lectura);
                }

            }

            foreach (var item in Lista)
            {
                try
                {
                    create(item);
                }
                catch (Exception ex)
                {
                    errores += item.Identificador + ";" + item.Lote + ";" + ex.Message + Environment.NewLine;
                }
            }

        }
        #endregion

        public IEnumerable<LecturasAsistenteModel> BuscarLecturas()
        {
            //lecturas
            var lecturas = _db.Lecturas.Where(f => f.usuario == Usuarioid && f.insertado == false).GroupBy(x => new { x.identificador, x.fecha })
                        .Select( group => new LecturasAsistenteModel()  { 
                                    Identificador = group.Key.identificador,
                                    Fecha = group.Key.fecha,
                                    Numregistros = group.Count()
                                })
                        .ToList();

            return lecturas;
        }
    }
}
