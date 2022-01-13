using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System.Text;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public class VencimientosService : GestionService<VencimientosModel, Persistencia.Vencimientos>
    {

        #region CONSTRUCTOR
        public VencimientosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion    

        #region CRUD
        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as VencimientosModel;

                if(model.Importeasignado == null)
                {
                    model.Importeasignado = 0;
                }
                
                if(_db.Vencimientos.Any())
                {
                    model.Id = _db.Vencimientos.Where(f => f.empresa == Empresa).Select(f => f.id).Max() + 1;
                }

                else
                {
                    model.Id = 0;
                }

                model.Fecha = DateTime.Now;

                //Calculo ID
                var contador = ServiceHelper.GetNextIdContable<Marfil.Dom.Persistencia.Vencimientos>(_db, Empresa, model.Fkseriescontables);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReferenceContable<Marfil.Dom.Persistencia.Vencimientos>(_db, model.Empresa, model.Fkseriescontables, contador, model.Fecha.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Llamamos al base
                base.create(model);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as VencimientosModel;
                base.edit(model);
                tran.Complete();
            }
        }

        public bool GetRemesable(string circuitotesoreria)
        {
            var idCircuito = int.Parse(circuitotesoreria);
            var situacion = _db.CircuitosTesoreriaCobros.Where(f => f.id == idCircuito).FirstOrDefault().situacionfinal;
            return (bool)_db.SituacionesTesoreria.Where(f => f.cod == situacion).FirstOrDefault().remesable;
        }

        public string GetDocumentoCreado(StAsistenteTesoreria model)
        {
            var idcircuito = Int32.Parse(model.Circuitotesoreria);
            var circuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();
            var situacionInicialCircuito = circuito.situacioninicial ?? "";
            var situacioninicialcobros = _db.SituacionesTesoreria.Where(f => f.valorinicialcobros == true).Select(f => f.cod).SingleOrDefault() ?? "";
            var situacioninicialpagos = _db.SituacionesTesoreria.Where(f => f.valorinicialpagos == true).Select(f => f.cod).SingleOrDefault() ?? "";
            bool esprevision = String.Equals(situacionInicialCircuito, situacioninicialcobros) || String.Equals(situacionInicialCircuito, situacioninicialpagos) ? true : false;
            var carteraId = 0;

            if (esprevision)
            {
                carteraId = _db.CarteraVencimientos.Where(f => f.empresa == Empresa).Select(f => f.id).Max();
            }
            else
            {
                carteraId = int.Parse(model.Vencimientos.Split(';')[0]);            
            }
            return _db.CarteraVencimientos.Where(f => f.empresa == Empresa && f.id == carteraId).FirstOrDefault().referenciaremesa;
        }
        #endregion

        public string getFacturaByTraza(string traza)
        {
            var id = _db.Facturas.Where(f => f.empresa == Empresa && f.referencia == traza).Select(f => f.id).SingleOrDefault();
            return id.ToString();
        }


        //Index Vencimientos Cobros
        public ListIndexModel GetListIndexModelCobros(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var fmodel = new FModel();
            var obj = fmodel.GetModel<VencimientosModel>(_context);
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;
            var model = new ListIndexModel()
            {
                Entidad = display.DisplayName,
                List = GetAllCobros<VencimientosModel>(),
                PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }
            };

            var situacionesService = new SituacionesTesoreriaService(_context, _db);

            var propiedadesVisibles = new[] { "Traza", "Referencia", "Origen", "Fkcuentas", "Descripcioncuenta", "Importegiro", "Importeasignado", "Estado", "Situacion" };
            var propiedades = Helpers.Helper.getProperties<VencimientosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.ColumnasCombo.Add("Situacion", situacionesService.getAll().OfType<SituacionesTesoreriaModel>().Select(f => new Tuple<string, string>(f.Cod, f.Descripcion)));
            model.PrimaryColumnns = new[] { "Id" };
            model.ColumnaColor = "8";
            return model;
        }

        public IEnumerable<T> GetAllCobros<T>() where T : VencimientosModel
        {
            var a = _db.Database.SqlQuery<T>(GetSelectPrincipalCobros()).ToList();
            return a;
        }

        public string GetSelectPrincipalCobros()
        {
            var result = new StringBuilder();
            result.Append(" select v.*, cuen.Descripcion as Descripcioncuenta ");
            result.Append(" from Vencimientos as v ");
            result.AppendFormat(" left join Cuentas as cuen on cuen.id = v.fkcuentas and cuen.empresa ='{0}' ", _context.Empresa);
            result.AppendFormat(" where v.tipo = 0 and v.empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        //--Pagos

        public ListIndexModel GetListIndexModelPagos(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var fmodel = new FModel();
            var obj = fmodel.GetModel<VencimientosModel>(_context);
            var instance = obj as IModelView;
            var extension = obj as IModelViewExtension;
            var display = obj as ICanDisplayName;
            var model = new ListIndexModel()
            {
                Entidad = display.DisplayName,
                List = GetAllPagos<VencimientosModel>(),
                PrimaryColumnns = extension.primaryKey.Select(f => f.Name).ToList(),
                VarSessionName = "__" + t.Name,
                Properties = instance.getProperties(),
                Controller = controller,
                PermiteEliminar = canEliminar,
                PermiteModificar = canModificar,
                ExcludedColumns = new[] { "Toolbar" }
            };

            var situacionesService = new SituacionesTesoreriaService(_context, _db);

            var propiedadesVisibles = new[] { "Traza", "Referencia", "Origen", "Fkcuentas", "Descripcioncuenta", "Importegiro", "Importeasignado", "Estado", "Situacion" };
            var propiedades = Helpers.Helper.getProperties<VencimientosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.ColumnasCombo.Add("Situacion", situacionesService.getAll().OfType<SituacionesTesoreriaModel>().Select(f => new Tuple<string, string>(f.Cod, f.Descripcion)));
            model.PrimaryColumnns = new[] { "Id" };
            model.ColumnaColor = "8";
            return model;
        }

        public IEnumerable<T> GetAllPagos<T>() where T : VencimientosModel
        {
            var a = _db.Database.SqlQuery<T>(GetSelectPrincipalPagos()).ToList();
            return a;
        }

        public string GetSelectPrincipalPagos()
        {
            var result = new StringBuilder();
            result.Append(" select v.*, cuen.Descripcion as Descripcioncuenta ");
            result.Append(" from Vencimientos as v ");
            result.AppendFormat(" left join Cuentas as cuen on cuen.id = v.fkcuentas and cuen.empresa ='{0}' ", _context.Empresa);
            result.AppendFormat(" where v.tipo = 1 and v.empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        public override IModelView get(string id)
        {
            var model = _converterModel.CreateView(id) as VencimientosModel;

            var lineas = _db.PrevisionesCartera.Where(f => f.empresa == Empresa && f.codvencimiento == model.Id.Value).ToList();
            var serviceCartera = new CarteraVencimientosService(_context);

            foreach (var linea in lineas)
            {
                var cartera = _db.CarteraVencimientos.Where(f => f.empresa == Empresa && f.id == linea.codcartera).SingleOrDefault();
                model.LineasCartera.Add(new CarteraVencimientosModel(_context)
                {
                    Id = linea.codcartera,
                    Referencia = cartera.referencia,
                    Fkcuentas = cartera.fkcuentas,
                    Importegiro = Math.Round(cartera.importegiro.Value,2),
                    Fechavencimiento = cartera.fechavencimiento,
                    Fecha = cartera.fecha,
                    Situacion = cartera.situacion,
                    Imputadoaux = _db.PrevisionesCartera.Where(f => f.empresa == Empresa && f.codvencimiento == model.Id.Value &&
                        f.codcartera == linea.codcartera).Select(f => f.imputado).SingleOrDefault()
                });
            }

            return model;
        }

        //Seccion Api Asistente
        public IEnumerable<VencimientosModel> getVencimientos(string tipoasignacion, string circuito, string fkcuentas, string importe)
        {
            List<VencimientosModel> vencimientos = new List<VencimientosModel>();
            var newtipoasignacion = Int32.Parse(tipoasignacion);
            var situacion = newtipoasignacion == 0 ? "X" : "Y";

            //Vencimientos con estado Inicial, pendiente de cobro o de pago y que aun tengan cantidad por asignar
            var posiblesvencimientos = _db.Vencimientos.Where(f => f.empresa == Empresa && f.fkcuentas == fkcuentas && f.tipo == newtipoasignacion
                && f.situacion == situacion && ((f.importegiro - f.importeasignado) > 0)).
                OrderByDescending(f => f.fechavencimiento).OrderByDescending(f => f.importegiro).ToList();

            //Tenemos que sacar todos los vencimientos
            foreach (var vencimiento in posiblesvencimientos)
            {
                var formapago = _db.FormasPago.Where(x => x.id == vencimiento.fkformaspago).Select(x => x.nombre).SingleOrDefault().ToString();
                vencimientos.Add(new VencimientosModel
                {
                    Id = vencimiento.id,
                    Referencia = vencimiento.referencia,
                    Traza = vencimiento.traza,
                    FechaStrfactura = vencimiento.fechafactura.ToString().Split(' ')[0],
                    FechaStrvencimiento = vencimiento.fechavencimiento.ToString().Split(' ')[0],
                    Importegiro = Math.Round(vencimiento.importegiro.Value - vencimiento.importeasignado.Value,2),
                    Fkformaspago = vencimiento.fkformaspago,
                    FormaPago = formapago,
                    Fkcuentatesoreria = vencimiento.fkcuentatesoreria
                });
            }

            return vencimientos;
        }


        //Seccion Api Asistente
        public IEnumerable<GridAsistenteMovimientosTesoreriaModel> getVencimientosMovimientosTesoreria(string tipoasignacion, string circuito, string fkmodospago,
            string cuentadesde, string cuentahasta, string fechadesde, string fechahasta, string referenciaRemesa)
        {
            List<VencimientosModel> vencimientos = new List<VencimientosModel>();
            var appService = new ApplicationHelper(_context);
            var idcircuito = Int32.Parse(circuito);

            var situacionInicialCircuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Select(f => f.situacioninicial).SingleOrDefault() ?? "";
            var situacioninicialcobros = _db.SituacionesTesoreria.Where(f => f.valorinicialcobros == true).Select(f => f.cod).SingleOrDefault() ?? "";
            var situacioninicialpagos = _db.SituacionesTesoreria.Where(f => f.valorinicialpagos == true).Select(f => f.cod).SingleOrDefault() ?? "";
            var idformaspagos = _db.FormasPago.Where(f => f.fkgruposformaspago == fkmodospago).Select(f => f.id).ToList();

            List<GridAsistenteMovimientosTesoreriaModel> registros = new List<GridAsistenteMovimientosTesoreriaModel>();

            //Registros provenientes de prevision
            if (string.Equals(situacionInicialCircuito, situacioninicialcobros) || string.Equals(situacionInicialCircuito, situacioninicialpagos))
            {
                var tipo = string.Equals(situacionInicialCircuito, situacioninicialcobros) ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;

                var posiblesvencimientos = _db.Vencimientos.Where(f => f.empresa == Empresa && f.tipo == (int)tipo && f.situacion == situacionInicialCircuito
                && ((f.importegiro ?? 0) - (f.importeasignado ?? 0)) > 0 && idformaspagos.Contains(f.fkformaspago.Value)).ToList();

                if (!String.IsNullOrEmpty(cuentadesde))
                {
                    posiblesvencimientos = posiblesvencimientos.Where(f => string.Compare(f.fkcuentas, cuentadesde) >= 0).ToList();
                }

                if (!String.IsNullOrEmpty(cuentahasta))
                {
                    posiblesvencimientos = posiblesvencimientos.Where(f => string.Compare(cuentahasta, f.fkcuentas) >= 0).ToList();
                }

                if (!String.IsNullOrEmpty(fechadesde))
                {
                    DateTime? datedesde = DateTime.Parse(fechadesde);
                    posiblesvencimientos = posiblesvencimientos.Where(f => f.fechavencimiento >= datedesde).ToList();
                }

                if (!String.IsNullOrEmpty(fechahasta))
                {
                    DateTime? datehasta = DateTime.Parse(fechahasta);
                    posiblesvencimientos = posiblesvencimientos.Where(f => f.fechavencimiento <= datehasta).ToList();
                }

                foreach (var venc in posiblesvencimientos.OrderBy(f => f.fechavencimiento).ThenBy(f => f.importegiro - f.importeasignado))
                {
                    var formapago = _db.FormasPago.Where(x => x.id == venc.fkformaspago).Select(x => x.nombre).SingleOrDefault().ToString();
                    var descripcioncuenta = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == venc.fkcuentas).Select(f => f.descripcion).SingleOrDefault() ?? "";

                    registros.Add(new GridAsistenteMovimientosTesoreriaModel(_context)
                    {
                        Id = venc.id,
                        Referencia = venc.referencia,
                        Tipo = venc.tipo,
                        Traza = venc.traza,
                        Fkcuentas = venc.fkcuentas,
                        FkcuentasDescripcion = descripcioncuenta,
                        Fechavencimiento = venc.fechavencimiento.ToString().Split(' ')[0],
                        Importegiro = Math.Round((double)(venc.importegiro - venc.importeasignado), 2),
                        ImporteAsignado = venc.importeasignado,
                        Situacion = venc.situacion,
                        Fkformaspago = formapago,
                        FkcuentaTesoreria = venc.fkcuentatesoreria,
                        Fkseriescontables = venc.fkseriescontables
                    });
                }
            }

            //Registros provenientes de cartera
            else
            {
                var tipo = string.Equals(Int32.Parse(tipoasignacion), (int)TipoVencimiento.Cobros) ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;
                var registroscartera = _db.CarteraVencimientos.Where(f => f.empresa == Empresa ).ToList();

                if (String.IsNullOrEmpty(referenciaRemesa))
                {
                    registroscartera = registroscartera.Where(f => f.tipovencimiento == (int)tipo && f.situacion == situacionInicialCircuito &&
                                        idformaspagos.Contains(f.fkformaspago.Value)).ToList();
                }

                if (!String.IsNullOrEmpty(cuentadesde))
                {
                    registroscartera = registroscartera.Where(f => string.Compare(f.fkcuentas, cuentadesde) >= 0).ToList();
                }

                if (!String.IsNullOrEmpty(cuentahasta))
                {
                    registroscartera = registroscartera.Where(f => string.Compare(cuentahasta, f.fkcuentas) >= 0).ToList();
                }

                if (!String.IsNullOrEmpty(fechadesde))
                {
                    DateTime? datedesde = DateTime.Parse(fechadesde);
                    registroscartera = registroscartera.Where(f => f.fechavencimiento >= datedesde).ToList();
                }

                if (!String.IsNullOrEmpty(fechahasta))
                {
                    DateTime? datehasta = DateTime.Parse(fechahasta);
                    registroscartera = registroscartera.Where(f => f.fechavencimiento <= datehasta).ToList();
                }

                if (!String.IsNullOrEmpty(referenciaRemesa))
                {
                    var anulada = _db.Remesas.Where(f=> f.empresa == Empresa && f.referenciaremesa == referenciaRemesa.ToUpper()).FirstOrDefault().estado;
                    //Si ya está anulada no mostramos registros
                    if (anulada == 0) {
                        registroscartera = registroscartera.Where(f => f.referenciaremesa == referenciaRemesa.ToUpper()).ToList();
                    }
                    else
                    {
                        registroscartera.Clear();
                    }                  
                }

                foreach (var venc in registroscartera.OrderBy(f => f.fkcuentas).ThenBy(f => f.fechavencimiento).ThenBy(f => f.importegiro))
                {
                    var formapago = _db.FormasPago.Where(x => x.id == venc.fkformaspago).Select(x => x.nombre).SingleOrDefault().ToString();
                    var descripcioncuenta = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == venc.fkcuentas).Select(f => f.descripcion).SingleOrDefault() ?? "";

                    registros.Add(new GridAsistenteMovimientosTesoreriaModel(_context)
                    {
                        Id = venc.id,
                        Referencia = venc.referencia,
                        Tipo = venc.tipovencimiento,
                        Traza = venc.traza,
                        Fkcuentas = venc.fkcuentas,
                        FkcuentasDescripcion = descripcioncuenta,
                        Fechavencimiento = venc.fechavencimiento.ToString().Split(' ')[0],
                        Importegiro = Math.Round((double)venc.importegiro, 2),
                        ImporteAsignado = null,
                        Situacion = venc.situacion,
                        Fkformaspago = formapago,
                        FkcuentaTesoreria = venc.fkcuentastesoreria,
                        Fkseriescontables = venc.fkseriescontables
                    });
                }

            }

            return registros;
        }


        //Paso final, asignar cartera
        public void AsignarCartera(StAsistenteTesoreria model)
        {
            var vencimientosAux = model.Vencimientos.Split(';');
            List<String> vencimientos = new List<String>();      
            foreach(var aux in vencimientosAux)
            {
                vencimientos.Add(aux);
            }

            var serviceCarteraVencimientos = new CarteraVencimientosService(_context);

            var idcircuito = Int32.Parse(model.Circuitotesoreria);
            var circuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();

            CarteraVencimientosModel cartera = new CarteraVencimientosModel(_context);
            cartera.Empresa = Empresa;
            cartera.Tipovencimiento = model.Tipo == "0" ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;
            cartera.Fkseriescontables = model.Tipo == "0" ? _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRC").Select(f => f.id).SingleOrDefault() :
                _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRP").Select(f => f.id).SingleOrDefault();
            cartera.Situacion = circuito.situacionfinal;    
            cartera.Fkcuentas = model.Fkcuentas;
            cartera.Importegiro = double.Parse(model.ImportePantalla3, CultureInfo.InvariantCulture);
            cartera.Comentario = model.Comentario;

            var vencimientoModel = get(vencimientos[0]) as VencimientosModel;
            cartera.Traza = vencimientoModel.Traza; //Num Doc. *NF*
            cartera.Fkformaspago = vencimientoModel.Fkformaspago;
            cartera.Fkcuentastesoreria = model.Fkcuentatesoreria;
            cartera.Fechavencimiento = DateTime.Parse(model.FechaVencimiento);
            cartera.Fechacreacion = DateTime.Now;
            cartera.Fecha = DateTime.Now;
            cartera.Usuario = _context.Usuario;
            cartera.Letra = model.Letra;
            cartera.Banco = model.Banco;  

            //Asignar importes completos
            if (String.IsNullOrEmpty(model.Importe))
            {
                if (circuito.asientocontable.Value)
                {
                    generarAsientoContable(model, true, idcircuito, vencimientos);
                }

                var importeaux = double.Parse(model.ImportePantalla3, CultureInfo.InvariantCulture);

                foreach (var vencimientoId in vencimientos)
                {
                    var vencimiento = get(vencimientoId) as VencimientosModel;

                    cartera.LineasCartera.Add(new PrevisionesCarteraModel(_context)
                    {
                        Codvencimiento = Int32.Parse(vencimientoId),
                        Codcartera = _db.CarteraVencimientos.Any() ? _db.CarteraVencimientos.Select(f => f.id).Max() + 1 : 0,
                        Imputado = vencimiento.Importegiro - vencimiento.Importeasignado
                    });

                    editarSituacionPrevision(vencimiento, model, circuito.situacionfinal, null);
                }

                serviceCarteraVencimientos.create(cartera);
            }

            else
            {
                var importeaux = double.Parse(model.Importe, CultureInfo.InvariantCulture);
                List<VencimientosModel> previsionesAuxiliares = new List<VencimientosModel>();

                foreach (var vencimientoId in vencimientos)
                {
                    //Quedan previsiones por cubrir
                    if(importeaux > 0)
                    {
                        var vencimiento = get(vencimientoId) as VencimientosModel;
                        var restante = vencimiento.Importegiro - vencimiento.Importeasignado;

                        //El importe cubre lo que queda de asignado
                        if (importeaux >= (restante))
                        {
                            editarSituacionPrevision(vencimiento, model, circuito.situacionfinal, null);
                            importeaux = importeaux - restante.Value;

                            cartera.LineasCartera.Add(new PrevisionesCarteraModel(_context)
                            {
                                Codvencimiento = Int32.Parse(vencimientoId),
                                Codcartera = _db.CarteraVencimientos.Any() ? _db.CarteraVencimientos.Select(f => f.id).Max() + 1 : 0,
                                Imputado = restante
                            });
                        }

                        else
                        {
                            vencimiento.Situacion = model.Tipo == "0" ? "X" : "Y";
                            vencimiento.Importeasignado = vencimiento.Importeasignado + importeaux;
                            vencimiento.Estado = TipoEstado.CubiertoParcial;                

                            cartera.LineasCartera.Add(new PrevisionesCarteraModel(_context)
                            {
                                Codvencimiento = Int32.Parse(vencimientoId),
                                Codcartera = _db.CarteraVencimientos.Any() ? _db.CarteraVencimientos.Select(f => f.id).Max() + 1 : 0,
                                Imputado = importeaux
                            });

                            importeaux = importeaux - vencimiento.Importeasignado.Value;
                        }

                        previsionesAuxiliares.Add(vencimiento);
                        //edit(vencimiento);
                    }             
                }

                //--------------------!!!!!!Explicacion¡¡¡¡¡¡¡------------------
                //Antes de editar previsiones y generar Cartera, debemos asegurarnos de si se puede generar un asiento contable. Al asiento contable le paso una lista de previsiones auxiliares,
                //que son las previsiones editadas, si se genera excepcion, se corta y manda el mensaje. Si consigue crear el asiento, editamos las previsiones y generamos la cartera.

                List<String> idVencimientosAuxiliares = new List<String>();
                foreach(var v in previsionesAuxiliares)
                {
                    idVencimientosAuxiliares.Add(v.Id.ToString());
                }
                generarAsientoContable(model, true, idcircuito, idVencimientosAuxiliares);


                //Actualizar Previsiones
                foreach(var auxiliar in previsionesAuxiliares)
                {
                    edit(auxiliar);
                }

                serviceCarteraVencimientos.create(cartera);
            }          
        }




        //-----------------------GENERAR REGISTRO EN CARTERA-----------------------
        public CarteraVencimientosModel CrearRegistroCartera(VencimientosModel registro, StAsistenteTesoreria model, string nuevasituacion, string fkseriescontablesremesa, string identificadorsegmentoremesa, string referenciaremesa)
        {
            CarteraVencimientosModel cartera = new CarteraVencimientosModel(_context);
            cartera.Empresa = Empresa;
            cartera.Tipovencimiento = model.Tipo == "0" ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;
            cartera.Fkseriescontables = model.Tipo == "0" ? _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRC").Select(f => f.id).FirstOrDefault() :
                _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRP").Select(f => f.id).FirstOrDefault();
            cartera.Situacion = nuevasituacion;
            cartera.Traza = registro.Traza; //Num Doc. *NF*
            cartera.Fkcuentas = registro.Fkcuentas;
            cartera.Importegiro = registro.Importegiro;
            cartera.Comentario = !String.IsNullOrEmpty(model.Comentario) ? model.Comentario : registro.Comentario;
            cartera.Fkformaspago = registro.Fkformaspago;
            cartera.Fkcuentastesoreria = !String.IsNullOrEmpty(model.Fkcuentatesoreria) ? model.Fkcuentatesoreria : registro.Fkcuentatesoreria;
            cartera.Fechavencimiento = registro.Fechavencimiento;
            cartera.Fechacreacion = DateTime.Now;
            cartera.Fecha = DateTime.Now;
            cartera.Usuario = _context.Usuario;
            cartera.Letra = !String.IsNullOrEmpty(model.Letra) ? model.Letra : "";
            cartera.Banco = model.Banco;

            cartera.LineasCartera.Add(new PrevisionesCarteraModel(_context)
            {
                Codvencimiento = registro.Id,
                Codcartera = _db.CarteraVencimientos.Any() ? _db.CarteraVencimientos.Select(f => f.id).Max() + 1 : 0,
                Imputado = registro.Importegiro - registro.Importeasignado //Se imputa todo
            });
            var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == nuevasituacion).FirstOrDefault().remesable;
            if (nuevasituacion.Equals("P"))
            {
                if (!String.IsNullOrEmpty(model.FechaPago))
                {
                    cartera.Fechapago = DateTime.Parse(model.FechaPago);
                }
            }

            if((bool)esRemesable)
            {
                //misma referencia para cada registro
                cartera.Fkseriescontablesremesa = fkseriescontablesremesa;
                cartera.Referenciaremesa = referenciaremesa;
                cartera.Identificadorsegmentoremesa = identificadorsegmentoremesa;

                /*cartera.Fkseriescontablesremesa = model.Fkseriescontables;
                var contador = ServiceHelper.GetNextIdContableMovimientosTesoreria<CarteraVencimientos>(_db, Empresa, cartera.Fkseriescontablesremesa);         
                var identificadorsegmentoremesa = "";
                cartera.Referenciaremesa = ServiceHelper.GetReferenceContableMovimientosTesoreria<CarteraVencimientos>(_db, cartera.Empresa, cartera.Fkseriescontablesremesa, contador, cartera.Fecha.Value, out identificadorsegmentoremesa);
                cartera.Identificadorsegmentoremesa = identificadorsegmentoremesa;*/

                if (!String.IsNullOrEmpty(model.Fecharemesa))
                {
                    cartera.Fecharemesa = DateTime.Parse(model.Fecharemesa);
                }               
            }

            return cartera;
        }

        //-----------------------GENERAR REGISTRO EN REMESA-----------------------
        public RemesasModel CrearRegistroRemesa(VencimientosModel registro, StAsistenteTesoreria model, string nuevasituacion, string fkseriescontablesremesa, string identificadorsegmentoremesa, string referenciaremesa, string referencia)
        {
            RemesasModel remesa = new RemesasModel(_context);
            remesa.Empresa = Empresa;
            remesa.Tipovencimiento = model.Tipo == "0" ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;
            remesa.Fkseriescontables = model.Tipo == "0" ? _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRC").Select(f => f.id).FirstOrDefault() :
                _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRP").Select(f => f.id).FirstOrDefault();
            remesa.Situacion = nuevasituacion;
            remesa.Traza = registro.Traza; //Num Doc. *NF*
            remesa.Fkcuentas = registro.Fkcuentas;
            remesa.Importegiro = registro.Importegiro;
            remesa.Comentario = !String.IsNullOrEmpty(model.Comentario) ? model.Comentario : registro.Comentario;
            remesa.Fkformaspago = registro.Fkformaspago;
            remesa.Fkcuentastesoreria = !String.IsNullOrEmpty(model.Fkcuentatesoreria) ? model.Fkcuentatesoreria : registro.Fkcuentatesoreria;
            remesa.Fechavencimiento = registro.Fechavencimiento;
            remesa.Fechacreacion = DateTime.Now;
            remesa.Fecha = DateTime.Now;
            remesa.Usuario = _context.Usuario;
            remesa.Letra = !String.IsNullOrEmpty(model.Letra) ? model.Letra : "";
            remesa.Banco = model.Banco;

            remesa.Referencia = referencia;

            var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == nuevasituacion).FirstOrDefault().remesable;
            if ((bool)esRemesable)
            {
                //Misma referencia para cada registro
                remesa.Fkseriescontablesremesa = fkseriescontablesremesa;
                remesa.Referenciaremesa = referenciaremesa;
                remesa.Identificadorsegmentoremesa = identificadorsegmentoremesa;

                if (!String.IsNullOrEmpty(model.Fecharemesa))
                {
                    remesa.Fecharemesa = DateTime.Parse(model.Fecharemesa);
                }
            }

            return remesa;
        }

        //-----------------------GENERAR REGISTRO EN REMESA-----------------------
        public RemesasModel CrearRegistroRemesa(CarteraVencimientosModel registro, StAsistenteTesoreria model, string nuevasituacion, string fkseriescontablesremesa, string identificadorsegmentoremesa, string referenciaremesa, string referencia)
        {
            RemesasModel remesa = new RemesasModel(_context);
            remesa.Empresa = Empresa;
            remesa.Tipovencimiento = model.Tipo == "0" ? TipoVencimiento.Cobros : TipoVencimiento.Pagos;
            remesa.Fkseriescontables = model.Tipo == "0" ? _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRC").Select(f => f.id).FirstOrDefault() :
                _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "CRP").Select(f => f.id).FirstOrDefault();
            remesa.Situacion = nuevasituacion;
            remesa.Traza = registro.Traza; //Num Doc. *NF*
            remesa.Fkcuentas = registro.Fkcuentas;
            remesa.Importegiro = registro.Importegiro;
            remesa.Comentario = !String.IsNullOrEmpty(model.Comentario) ? model.Comentario : registro.Comentario;
            remesa.Fkformaspago = registro.Fkformaspago;
            remesa.Fkcuentastesoreria = !String.IsNullOrEmpty(model.Fkcuentatesoreria) ? model.Fkcuentatesoreria : registro.Fkcuentastesoreria;
            remesa.Fechavencimiento = registro.Fechavencimiento;
            remesa.Fechacreacion = DateTime.Now;
            remesa.Fecha = DateTime.Now;
            remesa.Usuario = _context.Usuario;
            remesa.Letra = !String.IsNullOrEmpty(model.Letra) ? model.Letra : "";
            remesa.Banco = model.Banco;

            remesa.Referencia = registro.Referencia;

            var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == nuevasituacion).FirstOrDefault().remesable;
            if ((bool)esRemesable)
            {
                //Misma referencia para cada registro
                remesa.Fkseriescontablesremesa = fkseriescontablesremesa;
                remesa.Referenciaremesa = referenciaremesa;
                remesa.Identificadorsegmentoremesa = identificadorsegmentoremesa;

                if (!String.IsNullOrEmpty(model.Fecharemesa))
                {
                    remesa.Fecharemesa = DateTime.Parse(model.Fecharemesa);
                }
            }

            return remesa;
        }

        //Editar Prevision: Situacion, Estado, Importe Asignado, (Pagado y fechapago si es P)
        public void editarSituacionPrevision(VencimientosModel registro, StAsistenteTesoreria model, string situacion, double? impagado)
        {
            registro.Importeasignado = registro.Importegiro;
            registro.Estado = TipoEstado.Total;
            registro.Situacion = situacion;

            if(situacion.Equals("P"))
            {
                registro.Importepagado = registro.Importegiro;
                if (!String.IsNullOrEmpty(model.FechaPago))
                {
                    registro.Fechapago = DateTime.Parse(model.FechaPago);
                } 
            }

            if (situacion.Equals("I")) //-> Prevision de 100 con dos registros en cartera de 40 y 60. El de 40 pasa a impagado. 100 - 40 = 60
            {
                registro.Importepagado = registro.Importepagado - impagado;
            }

            edit(registro);
        }

        //Editar Cartera: Situacion, (fechapago si es P), (fecha remesa y documento remesa si es remesa) y lineas de previsiones
        public void editarSituacionCartera(CarteraVencimientosModel cartera, StAsistenteTesoreria model, string situacion, bool? anularremesa, string fkseriescontablesremesa, string referenciaremesa, string identificadorsegmentoremesa, bool esprevision)
        {
            var carteraService = new CarteraVencimientosService(_context);
            cartera.Situacion = situacion;
            cartera.Fkcuentastesoreria = model.Fkcuentatesoreria;
            var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == situacion).FirstOrDefault().remesable;

            if (situacion.Equals("P"))
            {
                if (!String.IsNullOrEmpty(model.FechaPago))
                {
                    cartera.Fechapago = DateTime.Parse(model.FechaPago);
                }
            }

            if ((bool)anularremesa)
            {
                cartera.Referenciaremesa = "";
                cartera.Identificadorsegmentoremesa = "";
                cartera.Fecharemesa = null;
            }

            if((bool)esRemesable)
            {
                cartera.Fkseriescontablesremesa = fkseriescontablesremesa;

                if (!(bool)anularremesa)
                {
                    cartera.Referenciaremesa = referenciaremesa;
                    cartera.Identificadorsegmentoremesa = identificadorsegmentoremesa;
                    if (!String.IsNullOrEmpty(model.Fecharemesa))
                    {
                        cartera.Fecharemesa = DateTime.Parse(model.Fecharemesa);
                    }
                    carteraService.create(CrearRegistroRemesa(cartera, model, situacion, cartera.Fkseriescontablesremesa, identificadorsegmentoremesa, cartera.Referenciaremesa, cartera.Referencia));
                }               
            }

            foreach (var registro in cartera.LineasPrevisiones)
            {
                editarSituacionPrevision(registro, model, situacion, cartera.Importegiro); //En caso de Impagado -> restarle al Pagado total de la prevision, el importe de ese registro en cartera
            }

            carteraService.edit(cartera);
        }

        //Asistente 2
        public void AsignarMovimientosTesoreria(StAsistenteTesoreria model)
        {
            var idcircuito = Int32.Parse(model.Circuitotesoreria);
            var circuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();
            var situacionInicialCircuito = circuito.situacioninicial ?? "";
            var situacioninicialcobros = _db.SituacionesTesoreria.Where(f => f.valorinicialcobros == true).Select(f => f.cod).SingleOrDefault() ?? "";
            var situacioninicialpagos = _db.SituacionesTesoreria.Where(f => f.valorinicialpagos == true).Select(f => f.cod).SingleOrDefault() ?? "";
            var serviceCarteraVencimientos = new CarteraVencimientosService(_context);

            var registrosaux = model.Vencimientos.Split(';');
            List<String> registros = new List<String>();
            foreach (var aux in registrosaux)
            {
                registros.Add(aux);
            }

            var fechacreacion = DateTime.Now;
            bool esprevision = String.Equals(situacionInicialCircuito, situacioninicialcobros) || String.Equals(situacionInicialCircuito, situacioninicialpagos) ? true : false;

            //Asiento Contable
            if (circuito.asientocontable.Value)
            {
                generarAsientoContable(model, esprevision, idcircuito, registros);
            }

            //Previsiones----> X-C | X-P | X-R | X-G
            if (esprevision)
            {
                esprevision = true;
                var fkseriescontablesremesa = "";
                var identificadorsegmentoremesa = "";
                var referenciaremesa = "";
                var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == circuito.situacionfinal).FirstOrDefault().remesable;

                //Asignamos referencia remesa
                if ((bool)esRemesable && circuito.anularremesa == false)
                {
                    fkseriescontablesremesa = model.Fkseriescontables;
                    var contador = ServiceHelper.GetNextIdContableMovimientosTesoreria<Remesas>(_db, Empresa, fkseriescontablesremesa);
                    referenciaremesa = ServiceHelper.GetReferenceContableMovimientosTesoreria<Remesas>(_db, Empresa, fkseriescontablesremesa, contador, DateTime.Now, out identificadorsegmentoremesa);

                }

                foreach (var prevision in registros)
                {
                    var registro = get(prevision) as VencimientosModel;
                    registro.Importegiro = Math.Round((double)registro.Importegiro, 2);
                    serviceCarteraVencimientos.create(CrearRegistroCartera(registro, model, circuito.situacionfinal, fkseriescontablesremesa, identificadorsegmentoremesa, referenciaremesa));

                    if ((bool)esRemesable)
                    {
                        serviceCarteraVencimientos.create(CrearRegistroRemesa(registro, model, circuito.situacionfinal, fkseriescontablesremesa, identificadorsegmentoremesa, referenciaremesa, ""));
                    }

                    editarSituacionPrevision(registro, model, circuito.situacionfinal, null);
                }
            }

            //Cartera ----> C-P | C-R | C-G | C-I
            else
            {
                var carteraService = new CarteraVencimientosService(_context);
                var remesaService = new RemesasService(_context);
                var situacionFinalCircuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Select(f => f.situacionfinal).SingleOrDefault() ?? "";
                var fkseriescontablesremesa = "";
                var identificadorsegmentoremesa = "";
                var referenciaremesa = "";
                var esRemesable = _db.SituacionesTesoreria.Where(f => f.cod == circuito.situacionfinal).FirstOrDefault().remesable;
                //Asignamos referencia remesa
                if ((bool)esRemesable && circuito.anularremesa == false)
                {
                    fkseriescontablesremesa = model.Fkseriescontables;
                    var contador = ServiceHelper.GetNextIdContableMovimientosTesoreria<Remesas>(_db, Empresa, fkseriescontablesremesa);
                    referenciaremesa = ServiceHelper.GetReferenceContableMovimientosTesoreria<Remesas>(_db, Empresa, fkseriescontablesremesa, contador, DateTime.Now, out identificadorsegmentoremesa);

                }

                foreach (var id in registros)
                {
                    var carteraModel = carteraService.get(id) as CarteraVencimientosModel;
                    carteraModel.Importegiro = Math.Round((double)carteraModel.Importegiro, 2);
                    var referenciaRemesa = carteraModel.Referenciaremesa;
                    editarSituacionCartera(carteraModel, model, circuito.situacionfinal,circuito.anularremesa, fkseriescontablesremesa, referenciaremesa, identificadorsegmentoremesa, esprevision);
                    if ((bool)circuito.anularremesa)
                    {                     
                        var remesaid = _db.Remesas.Where(f => f.empresa == Empresa && f.referenciaremesa == referenciaRemesa && f.referencia == carteraModel.Referencia && f.estado == 0).FirstOrDefault().id;
                        var remesaModel = remesaService.getModel(remesaid) as RemesasModel;
                        editarEstadoRemesa(remesaModel, model, circuito.situacionfinal);
                    }
                    
                }
            }
        }

        private void editarEstadoRemesa(RemesasModel remesaModel, StAsistenteTesoreria model, string situacionfinal)
        {
            var remesaService = new RemesasService(_context);

            remesaModel.Estado = TipoEstadoRemesa.Anulada;

            remesaService.edit(remesaModel);
        }

        //Genera un asiento contable
        public void generarAsientoContable(StAsistenteTesoreria model, bool esprevision, int idcircuito, List<String> registros)
        {

            //Campos del asiento
            MovsModel documento = new FModel().GetModel<MovsModel>(_context);
            var appService = new ApplicationHelper(_context);
            documento.Fkseriescontables = _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "AST").Select(f => f.id).SingleOrDefault() ?? "";
            documento.Fecha = DateTime.Now;
            documento.Tipoasiento = "F1"; //Por defecto, Normal
            var coddescripcion = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Select(f => f.codigodescripcionasiento).SingleOrDefault() ?? "";
            documento.Codigodescripcionasiento = coddescripcion;
            documento.Descripcionasiento = appService.GetListDescripcionAsientos().Where(f => f.Valor == coddescripcion).Select(f => f.Descripcion).SingleOrDefault() ?? "";

            var circuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();

            //Lineas del asiento
            if (circuito.importecuentacargo1 != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentacargo1;
                documento.Lineas = generarLineas(true, documento, importe, circuito.cuentacargo1, circuito.desccuentacargo1, registros, esprevision, model);
            }

            if (circuito.importecuentacargo2 != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentacargo2;
                documento.Lineas = generarLineas(true, documento, importe, circuito.cuentacargo2, circuito.desccuentacargo2, registros, esprevision, model);
            }

            if (circuito.importecuentacargorel != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentacargorel;
                documento.Lineas = generarLineas(true, documento, importe, circuito.cuentacargorel, circuito.desccuentacargorel, registros, esprevision, model);
            }

            if (circuito.importecuentaabono1 != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentaabono1;
                documento.Lineas = generarLineas(false, documento, importe, circuito.cuentaabono1, circuito.desccuentaabono1, registros, esprevision, model);
            }

            if (circuito.importecuentaabono2 != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentaabono2;
                documento.Lineas = generarLineas(false, documento, importe, circuito.cuentaabono2, circuito.desccuentaabono2, registros, esprevision, model);
            }

            if (circuito.importecuentaabonorel != (int)TipoImporte.Blanco)
            {
                var importe = (TipoImporte)circuito.importecuentaabonorel;
                documento.Lineas = generarLineas(false, documento, importe, circuito.cuentaabonorel, circuito.desccuentaabonorel, registros, esprevision, model);
            }

            documento.Debe = 0;
            documento.Haber = 0;
            int id = 0;

            //Suma debe y haber
            foreach(var linea in documento.Lineas)
            {
                if(linea.Esdebe == 1)
                {
                    documento.Debe = documento.Debe + linea.Importe;
                }

                if (linea.Esdebe == -1)
                {
                    documento.Haber = documento.Haber + linea.Importe;
                }
                linea.Id = id++;
            }

            documento.Saldo = documento.Debe - documento.Haber;

            if(documento.Saldo != 0)
            {
                throw new ValidationException("Asiento descuadrado. Transacción cancelada.");
            }

            else
            {
                documento.Generar = GenerarMovimientoAPartirDe.AsignarCartera;
                documento.Fkmonedas = Funciones.Qint(appService.GetCurrentEmpresa().FkMonedaBase);

                //Create
                var serviceMovs = new MovsService(_context);
                serviceMovs.create(documento);
            }     
        }


        //Generar Lineas del asiento
        public List<MovsLinModel> generarLineas(bool debe, MovsModel documento, TipoImporte tipo, string cuenta, string descripcion, List<String> registros, bool esprevision, StAsistenteTesoreria model)
        {
            if(tipo == TipoImporte.Importelineapunteada)
            {
                documento.Lineas = generarLineasTipoLineaPunteada(debe, documento, cuenta, descripcion, registros, esprevision, model, tipo);
            }

            else if(tipo == TipoImporte.Sumaimportelineaspunteadas)
            {
                documento.Lineas = generarLineasTipoSumaImportes(debe, documento, cuenta, descripcion, registros, esprevision, model, tipo);
            }

            else if(tipo == TipoImporte.Importecuentacargo2)
            {
                documento.Lineas = generarLineasTipoImporteCargo2Abono2(debe, documento, cuenta, descripcion, registros, esprevision, model.ImporteCargo2, model, tipo);
            }

            else if (tipo == TipoImporte.Importe2menosimporte3)
            {
                documento.Lineas = generarImporteLineasPunteadasMenosOtroImporte(esprevision, debe, documento, cuenta, descripcion, model.ImporteCargo2, registros, model, tipo);
            }

            else if (tipo == TipoImporte.Importe2masimporte3)
            {
                documento.Lineas = generarImporteLineasPunteadasMasOtroImporte(esprevision, debe, documento, cuenta, descripcion, model.ImporteCargo2, registros, model, tipo);
            }

            else if (tipo == TipoImporte.Importecuentaabono2)
            {
                documento.Lineas = generarLineasTipoImporteCargo2Abono2(debe, documento, cuenta, descripcion, registros, esprevision, model.ImporteAbono2, model, tipo);
            }

            else if (tipo == TipoImporte.Importe2menosimporte6)
            {
                documento.Lineas = generarImporteLineasPunteadasMenosOtroImporte(esprevision, debe, documento, cuenta, descripcion, model.ImporteAbono2, registros, model, tipo);
            }

            else if (tipo == TipoImporte.Importe2masimporte6)
            {
                documento.Lineas = generarImporteLineasPunteadasMasOtroImporte(esprevision, debe, documento, cuenta, descripcion, model.ImporteAbono2, registros, model, tipo);
            }

            return documento.Lineas;
        }

        //C/T
        public string getFullNameCuenta(bool esprevision, string registro, string nombrecuenta, string cuentatesoreria)
        {
            var serviceCartera = new CarteraVencimientosService(_context);
            string cuentatercero = nombrecuenta; //Resultado
            string cuentaregistro = "";

            if(esprevision)
            {
                var prevision = get(registro) as VencimientosModel;
                cuentaregistro = prevision.Fkcuentas;
            }

            else
            {
                var cartera = serviceCartera.get(registro) as CarteraVencimientosModel;
                cuentaregistro = cartera.Fkcuentas;
            }

            if (nombrecuenta.Contains("T"))
            {
                cuentatercero = String.Concat(nombrecuenta.Split('T')[0], cuentaregistro.Substring(nombrecuenta.Split('T')[0].Length));
            }

            else if (nombrecuenta.Contains("C"))
            {
                cuentatercero = String.Concat(nombrecuenta.Split('C')[0], cuentatesoreria.Substring(nombrecuenta.Split('C')[0].Length));
            }

            return cuentatercero;
        }
        

        public List<MovsLinModel> generarLineasTipoLineaPunteada(bool debe, MovsModel documento, string cuenta, string descripcioncuenta, List<String> registros, bool esprevision, StAsistenteTesoreria model, TipoImporte tipo)
        {
            var serviceCartera = new CarteraVencimientosService(_context);

            foreach (var registro in registros)
            {
                if(esprevision)
                {
                    var vencimiento = get(registro) as VencimientosModel;
                    string cuentatercero = getFullNameCuenta(esprevision, registro, cuenta,  model.Fkcuentatesoreria);

                    documento.Lineas.Add(generarDebeOHaber(debe, cuentatercero, vencimiento.Importegiro,
                                getComentario(tipo, registro, vencimiento.Fkcuentas, descripcioncuenta, model, registros, esprevision)));
                }

                //Cartera
                else
                {
                    var vencimiento = serviceCartera.get(registro) as CarteraVencimientosModel;
                    string cuentatercero = getFullNameCuenta(esprevision, registro, cuenta, model.Fkcuentatesoreria);

                    documento.Lineas.Add(generarDebeOHaber(debe, cuentatercero, vencimiento.Importegiro,
                            getComentario(tipo, registro, vencimiento.Fkcuentas, descripcioncuenta, model, registros, esprevision)));
                }
            }

            return documento.Lineas;
        }

        //Calcular el importe de las lineas punteadas
        public double? importeLineasPunteadas(List<String> registros, bool esprevision)
        {
            var serviceCartera = new CarteraVencimientosService(_context);
            double? importe = 0;

            if (esprevision)
            {
                foreach (var registro in registros)
                {
                    var vencimientoModel = get(registro) as VencimientosModel;
                    importe = importe + vencimientoModel.Importegiro;
                }
            }

            else
            {
                foreach (var registro in registros)
                {
                    var carteraModel = serviceCartera.get(registro) as CarteraVencimientosModel;
                    importe = importe + carteraModel.Importegiro;
                }
            }

            return importe;
        }

        //Se genera un asiento con la suma de los importes de las lineas punteadas
        public List<MovsLinModel> generarLineasTipoSumaImportes(bool debe, MovsModel documento, string cuenta, string descripcioncuenta, List<String> registros, bool esprevision, StAsistenteTesoreria model, TipoImporte tipo)
        {
            string cuentatercero = getFullNameCuenta(esprevision, registros[0], cuenta, model.Fkcuentatesoreria); //Le pasamos el primero, no tiene sentido un importe y multiples cuentas

            documento.Lineas.Add(generarDebeOHaber(debe, cuentatercero, importeLineasPunteadas(registros,esprevision),
                    getComentario(tipo, null, null, descripcioncuenta, model, registros, esprevision)));

            return documento.Lineas;
        }

        public List<MovsLinModel> generarImporteLineasPunteadasMenosOtroImporte(bool esprevision, bool debe, MovsModel documento, string cuenta, string descripcioncuenta, string otroimporte, List<String> registros, StAsistenteTesoreria model, TipoImporte tipo)
        {
            if(!String.IsNullOrEmpty(otroimporte))
            {
                var importe = importeLineasPunteadas(registros, esprevision) - Convert.ToDouble(otroimporte);
                string cuentatercero = getFullNameCuenta(esprevision, registros[0], cuenta, model.Fkcuentatesoreria); //Le pasamos el primero, no tiene sentido un importe y multiples cuentas

                documento.Lineas.Add(generarDebeOHaber(debe, cuentatercero, Convert.ToDouble(importe),
                    getComentario(tipo, null, null, descripcioncuenta, model, registros, esprevision)));
            }         

            return documento.Lineas;
        }

        public List<MovsLinModel> generarImporteLineasPunteadasMasOtroImporte(bool esprevision, bool debe, MovsModel documento, string cuenta, string descripcioncuenta, string otroimporte, List<String> registros, StAsistenteTesoreria model, TipoImporte tipo)
        {
            if(!String.IsNullOrEmpty(otroimporte))
            {
                var importe = importeLineasPunteadas(registros, esprevision) + Convert.ToDouble(otroimporte);
                string cuentatercero = getFullNameCuenta(esprevision, registros[0], cuenta, model.Fkcuentatesoreria); //Le pasamos el primero, no tiene sentido un importe y multiples cuentas

                documento.Lineas.Add(generarDebeOHaber(debe, cuentatercero, Convert.ToDouble(importe),
                    getComentario(tipo, null, null, descripcioncuenta, model, registros, esprevision)));
            }         

            return documento.Lineas;
        }

        //Se crea una linea con la cuenta de cargo 2 y el importe que el usuario ha puesto en la 3 pantalla del asistente
        public List<MovsLinModel> generarLineasTipoImporteCargo2Abono2(bool debe, MovsModel documento, string cuenta, string descripcioncuenta, List<String> registros, bool esprevision, string importe, StAsistenteTesoreria model, TipoImporte tipo)
        {
            if(!String.IsNullOrEmpty(importe))
            {
                documento.Lineas.Add(generarDebeOHaber(debe, cuenta, Convert.ToDouble(importe),
                getComentario(tipo, null, null, descripcioncuenta, model, registros, esprevision)));
            }
                
            return documento.Lineas;
        }

        public String getComentario(TipoImporte tipo, string id, string cuenta, string comentary, StAsistenteTesoreria model, List<String> registros, bool esprevision)
        {
            var idcircuito = Int32.Parse(model.Circuitotesoreria);
            var circuito = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();
            var serviceCartera = new CarteraVencimientosService(_context);
            string comentario = comentary;

            if(!String.IsNullOrEmpty(comentario))
            {
                if (comentario.Contains("*CO*") && !String.IsNullOrEmpty(model.Fkcuentatesoreria))
                {
                    comentario = comentario.Replace("*CO*", model.Fkcuentatesoreria);
                }

                //Tipo Importe 1
                if (comentario.Contains("*CL*") && tipo == TipoImporte.Importelineapunteada && !String.IsNullOrEmpty(id))
                {
                    comentario = comentario.Replace("*CL*", model.Letra);
                }

                if (comentario.Contains("*CM*") && !String.IsNullOrEmpty(model.Comentario))
                {
                    comentario = comentario.Replace("*CM*", model.Comentario);
                }

                if (comentario.Contains("*GT*"))
                {
                    comentario = comentario.Replace("*GT*", registros.Count.ToString());
                }

                if (comentario.Contains("*NL*"))
                {
                    comentario = comentario.Replace("*NL*", registros.Count.ToString());
                }

                //Cuenta Abono 1
                if (comentario.Contains("*CA*"))
                {
                    var cuentaabono = !String.IsNullOrEmpty(circuito.cuentaabono1) ? circuito.cuentaabono1 : !String.IsNullOrEmpty(circuito.cuentaabono2) ? circuito.cuentaabono2 : circuito.cuentaabonorel;
                    comentario = comentario.Replace("*CA*", cuentaabono.ToString());
                }

                //Tipo Importe 1
                if (comentario.Contains("*FV*") && tipo == TipoImporte.Importelineapunteada && esprevision && !String.IsNullOrEmpty(id))
                {
                    var vencimientosModel = get(id) as VencimientosModel;
                    comentario = comentario.Replace("*FV*", vencimientosModel.Fechavencimiento.ToString().Split(' ')[0]);
                }

                //Tipo Importe 1
                if (comentario.Contains("*FV*") && tipo == TipoImporte.Importelineapunteada && !esprevision && !String.IsNullOrEmpty(id))
                {
                    var carteraModel = serviceCartera.get(id) as CarteraVencimientosModel;
                    comentario = comentario.Replace("*FV*", carteraModel.Fechavencimiento.ToString().Split(' ')[0]);
                }

                //Descripcion Cta Abono
                if (comentario.Contains("*DA*"))
                {
                    var cuentaabono = !String.IsNullOrEmpty(circuito.cuentaabono1) ? circuito.cuentaabono1 : !String.IsNullOrEmpty(circuito.cuentaabono2) ? circuito.cuentaabono2 : circuito.cuentaabonorel;
                    var descuenta = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == cuentaabono).Select(f => f.descripcion).SingleOrDefault() ?? "";
                    comentario = comentario.Replace("*DA*", descuenta.ToString());
                }

                //Tipo Importe 1. Num factura
                if (comentario.Contains("*NF*") && tipo == TipoImporte.Importelineapunteada && esprevision && !String.IsNullOrEmpty(id))
                {
                    var vencimientosModel = get(id) as VencimientosModel;
                    comentario = comentario.Replace("*NF*", vencimientosModel.Traza.ToString());
                }

                if (comentario.Contains("*NF*") && tipo == TipoImporte.Importelineapunteada && !esprevision && !String.IsNullOrEmpty(id))
                {
                    var carteraModel = serviceCartera.get(id) as CarteraVencimientosModel;
                    comentario = comentario.Replace("*NF*", carteraModel.Traza.ToString());
                }

                if (comentario.Contains("*CC*"))
                {
                    var cuentacargo = !String.IsNullOrEmpty(circuito.cuentacargo1) ? circuito.cuentacargo1 : !String.IsNullOrEmpty(circuito.cuentacargo2) ? circuito.cuentacargo2 : circuito.cuentacargorel;
                    comentario = comentario.Replace("*CA*", cuentacargo.ToString());
                }

                if (comentario.Contains("*FA*"))
                {
                    comentario = comentario.Replace("*FA*", DateTime.Now.ToString().Split(' ')[0]);
                }

                //Tipo Importe 1
                if (comentario.Contains("*FF*") && tipo == TipoImporte.Importelineapunteada && esprevision && !String.IsNullOrEmpty(id))
                {
                    var vencimientosModel = get(id) as VencimientosModel;
                    comentario = comentario.Replace("*FF*", vencimientosModel.Fechafactura.ToString());
                }

                //Tipo Importe 1
                if (comentario.Contains("*CT*") && tipo == TipoImporte.Importelineapunteada && !String.IsNullOrEmpty(cuenta))
                {
                    comentario = comentario.Replace("*CT*", cuenta);
                }

                //Tipo Importe 1
                if (comentario.Contains("*FP*") && tipo == TipoImporte.Importelineapunteada && esprevision && !String.IsNullOrEmpty(id))
                {
                    var vencimientosModel = get(id) as VencimientosModel;
                    var desfp = _db.FormasPago.Where(f => f.id == vencimientosModel.Fkformaspago).Select(f => f.nombre).SingleOrDefault() ?? "";
                    comentario = comentario.Replace("*FP*", desfp.ToString());
                }

                //Tipo Importe 1
                if (comentario.Contains("*FP*") && tipo == TipoImporte.Importelineapunteada && !esprevision && !String.IsNullOrEmpty(id))
                {
                    var vencimientosModel = serviceCartera.get(id) as CarteraVencimientosModel;
                    var desfp = _db.FormasPago.Where(f => f.id == vencimientosModel.Fkformaspago).Select(f => f.nombre).SingleOrDefault() ?? "";
                    comentario = comentario.Replace("*FP*", desfp.ToString());
                }

                //Tipo Importe 1
                if (comentario.Contains("*DT*") && tipo == TipoImporte.Importelineapunteada && !String.IsNullOrEmpty(cuenta))
                {
                    var desc = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == cuenta).Select(f => f.descripcion).SingleOrDefault() ?? "";
                    comentario = comentario.Replace("*DT*", desc.ToString());
                }
            }

            return comentario;
        }

        //Genera un asiento dependiendo si es debe o es haber
        public MovsLinModel generarDebeOHaber(bool debe, string fkcuentas, double? importe, string comentario)
        {
            MovsLinModel linea = new MovsLinModel();
            linea.Fkcuentas = fkcuentas;
            linea.Importe = Convert.ToDecimal(importe);
            linea.Comentario = comentario;
            linea.Esdebe = debe ? Convert.ToInt16(1) : Convert.ToInt16(-1);

            return linea;
        }

        public MovsModel GenerarAsientoRegularizacionExistencias(string fecharegularizacion, string comentarioiniciales, string comentariofinales, string[] listacuentasexistencias, string[] listasaldoiniciales, string[] listacuentasvariacion, string[] listaimportefinales)
        {
            //Comprobamos el estado del ejercicio
            var serviceEjercicios = new EjerciciosService(_context);
            var idejerc = int.Parse(_context.Ejercicio);
            var ejercicio = serviceEjercicios.getAll().Where(f => ((Model.Configuracion.EjerciciosModel)f).Id == idejerc).Select(f => f as Model.Configuracion.EjerciciosModel).FirstOrDefault();

            if (ejercicio.Estado != Model.Configuracion.EstadoEjercicio.Abierto)
            {
                throw new ValidationException("No se puede realizar una regularización de existencias en un ejercicio que no esté abierto");
            }

            var registros = listacuentasexistencias.Length;

            //Cabecera del asiento
            MovsModel documento = new FModel().GetModel<MovsModel>(_context);
            var appService = new ApplicationHelper(_context);
            documento.Fkseriescontables = _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "AST").Select(f => f.id).SingleOrDefault() ?? "";
            documento.Fecha = DateTime.ParseExact(fecharegularizacion, "dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture);
            documento.Tipoasiento = "R3";
            documento.Codigodescripcionasiento = "";
            documento.Descripcionasiento = "REGULARIZACION EXISTENCIAS";
            documento.Referencia = "";
            documento.Canalcontable = "";

            //recorremos los registros para su tratamiento
            for (int i = 0; i < registros; i++)
            {
                var cuentavariacion = listacuentasvariacion[i];

                if (cuentavariacion != "" && _db.Cuentas.Where(f => f.id == cuentavariacion).FirstOrDefault() == null)
                {
                    throw new ValidationException("La cuenta de variación " + listacuentasvariacion[i] + " no existe en el plan contable");
                }

                //Si ambos importes son 0 no se procesa
                if (listasaldoiniciales[i] == "0" && listaimportefinales[i] == "0")
                {
                    continue;
                }
                else if (listasaldoiniciales[i] != "0")
                {
                    if (listacuentasvariacion[i] == "" || listacuentasvariacion[i] == null)
                    {
                        throw new ValidationException("Si existe una cuenta con saldo de existencia inicial, se debe indicar una cuenta de variación.");
                    }

                    documento.Lineas.Add(generarDebeOHaber(true, listacuentasvariacion[i], double.Parse(listasaldoiniciales[i]),comentarioiniciales));
                    documento.Lineas.Add(generarDebeOHaber(false, listacuentasexistencias[i], double.Parse(listasaldoiniciales[i]), comentarioiniciales));
                }
                else if (listaimportefinales[i] != "0")
                {                 
                    documento.Lineas.Add(generarDebeOHaber(true, listacuentasexistencias[i], double.Parse(listaimportefinales[i]), comentariofinales));
                    documento.Lineas.Add(generarDebeOHaber(false, listacuentasvariacion[i], double.Parse(listaimportefinales[i]), comentariofinales));
                }

            }           

            documento.Debe = 0;
            documento.Haber = 0;
            int id = 0;

            //Suma debe y haber
            foreach (var linea in documento.Lineas)
            {
                if (linea.Esdebe == 1)
                {
                    documento.Debe = documento.Debe + linea.Importe;
                }

                if (linea.Esdebe == -1)
                {
                    documento.Haber = documento.Haber + linea.Importe;
                }
                linea.Id = id++;
            }

            documento.Saldo = documento.Debe - documento.Haber;

            if (documento.Saldo != 0)
            {
                throw new ValidationException("Asiento descuadrado. Transacción cancelada.");
            }

            else
            {
                documento.Generar = GenerarMovimientoAPartirDe.AsignarCartera;
                documento.Fkmonedas = Funciones.Qint(appService.GetCurrentEmpresa().FkMonedaBase);

                //Create
                var serviceMovs = new MovsService(_context);
                serviceMovs.create(documento);
            }

            //Cambiamos el estado del ejercicio;
            ejercicio.Estado = Model.Configuracion.EstadoEjercicio.Existencias;
            _db.SaveChanges();

            return documento;
        }
    }
}
