using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class CriteriosAgrupacionStartup:IStartup
    {
        #region Members

        private readonly CriteriosagrupacionService _tablasVariasService;
        private IContextService _context;

        #endregion

        #region CTR

        public CriteriosAgrupacionStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = new CriteriosagrupacionService(context, db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearCriterios(contenido);
            }
        }

        private void CrearCriterios(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearCriterio(item);
            }
                
        }

        private void CrearCriterio(string linea)
        {
            var vector = linea.Split(';');
            var model = new CriteriosagrupacionModel();
            model.Id = vector[0];
            model.Nombre = vector[1];

            if(vector[2]=="true")
            {
                model.Ordenaralbaranes = true;
            }

            if (vector[2] == "false")
            {
                model.Ordenaralbaranes = false;
            }
            
            if(vector.Length>3)
            {
                model.Lineas.Add(new CriteriosagrupacionLinModel
                {
                    Id = Int32.Parse(vector[3]),
                    Campo = Int32.Parse(vector[4]),
                    Orden = Int32.Parse(vector[5])
                });

                if (vector.Length > 6)
                {
                    model.Lineas.Add(new CriteriosagrupacionLinModel
                    {
                        Id = Int32.Parse(vector[6]),
                        Campo = Int32.Parse(vector[7]),
                        Orden = Int32.Parse(vector[8])
                    });

                    if (vector.Length > 9)
                    {
                        model.Lineas.Add(new CriteriosagrupacionLinModel
                        {
                            Id = Int32.Parse(vector[9]),
                            Campo = Int32.Parse(vector[10]),
                            Orden = Int32.Parse(vector[11])
                        });

                        if (vector.Length > 12)
                        {
                            model.Lineas.Add(new CriteriosagrupacionLinModel
                            {
                                Id = Int32.Parse(vector[12]),
                                Campo = Int32.Parse(vector[13]),
                                Orden = Int32.Parse(vector[14])
                            });

                            if (vector.Length > 15)
                            {
                                model.Lineas.Add(new CriteriosagrupacionLinModel
                                {
                                    Id = Int32.Parse(vector[15]),
                                    Campo = Int32.Parse(vector[16]),
                                    Orden = Int32.Parse(vector[17])
                                });

                                if (vector.Length > 18)
                                {
                                    model.Lineas.Add(new CriteriosagrupacionLinModel
                                    {
                                        Id = Int32.Parse(vector[18]),
                                        Campo = Int32.Parse(vector[19]),
                                        Orden = Int32.Parse(vector[20])
                                    });

                                    if (vector.Length > 21)
                                    {
                                        model.Lineas.Add(new CriteriosagrupacionLinModel
                                        {
                                            Id = Int32.Parse(vector[21]),
                                            Campo = Int32.Parse(vector[22]),
                                            Orden = Int32.Parse(vector[23])
                                        });

                                        if (vector.Length > 24)
                                        {
                                            model.Lineas.Add(new CriteriosagrupacionLinModel
                                            {
                                                Id = Int32.Parse(vector[24]),
                                                Campo = Int32.Parse(vector[25]),
                                                Orden = Int32.Parse(vector[26])
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
