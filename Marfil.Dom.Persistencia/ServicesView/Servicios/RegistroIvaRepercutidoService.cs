﻿using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class RegistroIvaRepercutidoService : GestionService<RegistroIvaRepercutidoModel, RegistroIVARepercutido>
    {

        #region CONSTRUCTOR
        public RegistroIvaRepercutidoService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Origendoc", "Referencia", "Fechafactura", "Periodo", "Totalfactura", "Numfacturacliente", "Tipofactura", "Cuentacliente", "Nombrecliente"/*, "Cuentaventas"*/ };
            var propiedades = Helpers.Helper.getProperties<RegistroIvaRepercutidoModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Id" };

            model.OrdenColumnas.Add("Origendoc", 0);
            model.OrdenColumnas.Add("Referencia", 1);
            model.OrdenColumnas.Add("Fechafactura", 2);
            model.OrdenColumnas.Add("Periodo", 3);
            model.OrdenColumnas.Add("Totalfactura", 4);
            model.OrdenColumnas.Add("Numfacturacliente", 5);
            model.OrdenColumnas.Add("Tipofactura", 6);
            model.OrdenColumnas.Add("Cuentacliente", 7);
            model.OrdenColumnas.Add("Nombrecliente", 8);
            //model.OrdenColumnas.Add("Cuentaventas", 9);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();
            result.Append(" select r.origendoc, r.id, r.referencia, r.fechafactura, dbo._fn_enum_PeriodoIVA(r.periodo) as Periodo, r.totalfactura, r.numfacturacliente, t.descripcion as [Tipofactura], r.cuentacliente, r.cuentaventas, c.descripcion as [Nombrecliente] " +
                " from RegistroIVARepercutido r, Cuentas c , TiposFacturas t ");
            result.AppendFormat(" where r.empresa = c.empresa and r.empresa = t.empresa and r.cuentacliente = c.id and r.tipofactura = t.id and r.empresa ='{0}' order  by r.id desc", _context.Empresa);

            return result.ToString();
        }

        #endregion

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as RegistroIvaRepercutidoModel;

                //Calculo ID
                var contador = ServiceHelper.GetNextIdContable<RegistroIVARepercutido>(_db, Empresa, model.Fkseriescontables);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReferenceContable<RegistroIVARepercutido>(_db, model.Empresa, model.Fkseriescontables, contador, model.Fecharegistro, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Esto ya se controla en la pantalla
                /*var service = new TiposFacturasIvaService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos));              
                var regimen = service.GetRegimenivaRepercutido(_context.Empresa, model.Tipofactura);
                model.Fkregimeniva = regimen;*/

                if (model.Totales.Count() > 0)
                {
                    model.Descripcionoperacionemisor = string.IsNullOrEmpty(model.Descripcionoperacionemisor) ? GetDescripcionOperacion(model.Totales.First().Cuentaventas) : model.Descripcionoperacionemisor;
                }

                model = Recalculartotales(model);

                base.create(model);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as RegistroIvaRepercutidoModel;
                var editado = obj as RegistroIvaRepercutidoModel;

                //Esto ya se controla en la pantalla
                /*var service = new TiposFacturasIvaService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos));
                var regimen = service.GetRegimenivaRepercutido(_context.Empresa, editado.Tipofactura);
                editado.Fkregimeniva = regimen;*/

                if (editado.Totales.Count() > 0)
                {
                    editado.Descripcionoperacionemisor = string.IsNullOrEmpty(editado.Descripcionoperacionemisor) ? GetDescripcionOperacion(editado.Totales.First().Cuentaventas) : editado.Descripcionoperacionemisor;
                }
            
                editado = Recalculartotales(editado);

                base.edit(editado);
                _db.SaveChanges();
                tran.Complete();

            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                tran.Complete();
            }

        }

        #region Helpers

        /*public RegistroIvaRepercutidoSumaTotalesModel Recalculartotales(List<RegistroIvaRepercutidoTotalesModel> totales)
        {
            var suma = 0d;

            var result = new RegistroIvaRepercutidoSumaTotalesModel();
            result.Decimalesmonedas = 3;
            var max = _db.RegistroIVARepercutidoSumaTotales.Where(f => f.empresa == Empresa).Any() ? _db.RegistroIVARepercutidoSumaTotales.Where(f => f.empresa == Empresa).Max(f => f.id) + 1 : 1;
            result.Id = max;

            foreach (var item in totales)
            {
                suma += item.Baseimponible.Value;
            }

            result.Baseretencion = Math.Round(suma, (int)result.Decimalesmonedas);
            result.Porcentajeretencion = 21;
            result.Importeretencion = Math.Round(result.Baseretencion * (result.Porcentajeretencion / 100), (int)result.Decimalesmonedas);

            result.Totalfactura = Math.Round(result.Importeretencion + result.Operacionesexluidasbi);

            return result;
        }*/


        public RegistroIvaRepercutidoModel Recalculartotales(RegistroIvaRepercutidoModel model)
        {
            var sumaBases = 0d;
            var sumaSubtotal = 0d;
            var sumaCuotasiva = 0d;

            foreach (var item in model.Totales)
            {
                sumaBases += item.Baseimponible.Value;
                sumaSubtotal += item.Subtotal.Value;
                sumaCuotasiva += item.Cuotaiva.Value;
            }

            model.Baseretencion = Math.Round(sumaBases, 2);
            model.Importeretencion = Math.Round((double)(model.Baseretencion * (model.Porcentajeretencion / 100)), 2);
            model.Sumacuotasiva = Math.Round(sumaCuotasiva, 2);
            model.Operacionesexluidasbi = model.Operacionesexluidasbi ?? 0.0;

            model.Totalfactura = Math.Round((double)(sumaSubtotal - model.Importeretencion + model.Operacionesexluidasbi), 2);

            return model;
        }

        public string GetDescripcionOperacion(string cuentaventas)
        {
            if (string.IsNullOrEmpty(cuentaventas))
            {
                return "";
            }
            var descripcion = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == cuentaventas).FirstOrDefault().descripcion;
            return descripcion;
        }

        public string ModificarPeriodo(List<SelectListItem> listperiodo, DateTime Fechafactura, DateTime Fechaoperacion)
        {
            var service = new EmpresasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos));
            var tipofechaliquidacion = service.GetFechaLiquidacionIvaRepercutido(_context.Empresa);
                
                //Fecha Factura
            if (tipofechaliquidacion == 0 || Fechaoperacion == null || (Fechafactura.Month == Fechaoperacion.Month && Fechafactura.Year == Fechaoperacion.Year))
            {
                if (listperiodo.Count > 4)
                {
                    var mes = Fechafactura.Month;
                    return listperiodo[mes - 1].Value;

                }
                else
                {
                    var mes = Fechafactura.Month;

                    if (mes <= 3)
                    {
                        return listperiodo[0].Value;
                    }
                    else if (mes > 3 && mes <= 6)
                    {
                        return listperiodo[1].Value;
                    }
                    else if (mes > 6 && mes <= 9)
                    {
                        return listperiodo[2].Value;
                    }
                    else if (mes > 9 && mes <= 12)
                    {
                        return listperiodo[3].Value;
                    }

                }
            }        //Fecha Operación
            else if (tipofechaliquidacion == 1 || (Fechafactura.Month > Fechaoperacion.Month && Fechafactura.Year == Fechaoperacion.Year))
            {
                if (Fechafactura.Day <= 15)
                {
                    if (listperiodo.Count > 4)
                    {
                        var mes = Fechaoperacion.Month;
                        return listperiodo[mes - 1].Value;

                    }
                    else
                    {
                        var mes = Fechaoperacion.Month;

                        if (mes <= 3)
                        {
                            return listperiodo[0].Value;
                        }
                        else if (mes > 3 && mes <= 6)
                        {
                            return listperiodo[1].Value;
                        }
                        else if (mes > 6 && mes <= 9)
                        {
                            return listperiodo[2].Value;
                        }
                        else if (mes > 9 && mes <= 12)
                        {
                            return listperiodo[3].Value;
                        }
                    }
                }
                else
                {
                    if (listperiodo.Count > 4)
                    {
                        var mes = Fechafactura.Month;
                        return listperiodo[mes - 1].Value;

                    }
                    else
                    {
                        var mes = Fechafactura.Month;

                        if (mes <= 3)
                        {
                            return listperiodo[0].Value;
                        }
                        else if (mes > 3 && mes <= 6)
                        {
                            return listperiodo[1].Value;
                        }
                        else if (mes > 6 && mes <= 9)
                        {
                            return listperiodo[2].Value;
                        }
                        else if (mes > 9 && mes <= 12)
                        {
                            return listperiodo[3].Value;
                        }
                    }
                }
            }


            return listperiodo[0].Value;
        }

        #endregion
    }
}
