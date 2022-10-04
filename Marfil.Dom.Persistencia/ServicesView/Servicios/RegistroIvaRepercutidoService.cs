﻿using Marfil.Dom.Persistencia.Model;
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
            var propiedadesVisibles = new[] { "Origendoc", "Id", "Fechafactura", "Periodo", "Referencia", "Tipofactura", "Cuentacliente", "Nombre Cliente" };
            var propiedades = Helpers.Helper.getProperties<RegistroIvaRepercutidoModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Id" };

            return model;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();
            result.Append(" select r.origendoc, r.id, r.fechafactura, r.periodo, r.referencia, r.tipofactura, r.cuentacliente, c.descripcion as [Nombre Cliente] from RegistroIVARepercutido r, Cuentas c ");
            result.AppendFormat(" where r.empresa = c.empresa and r.cuentacliente = c.id and r.empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        #endregion

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as RegistroIvaRepercutidoModel;

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

                editado = Recalculartotales(editado);

                base.edit(obj);
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
            var suma = 0d;


            foreach (var item in model.Totales)
            {
                suma += item.Baseimponible.Value;
            }

            model.Baseretencion = Math.Round(suma, 3);
            model.Importeretencion = Math.Round(model.Baseretencion * (model.Porcentajeretencion / 100), 3);

            model.Totalfactura = Math.Round(model.Importeretencion + model.Operacionesexluidasbi);

            return model;
        }

        public string ModificarPeriodo(List<SelectListItem> listperiodo, DateTime Fechafactura, DateTime Fechaoperacion)
        {
            //var service = new EmpresasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos));
            //var tipofechaliquidacion = service.GetFechaLiquidacionIvaRepercutido(_context.Empresa);

            if (Fechaoperacion == null || (Fechafactura.Month == Fechaoperacion.Month && Fechafactura.Year == Fechaoperacion.Year))
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
            else if (Fechafactura.Month > Fechaoperacion.Month && Fechafactura.Year == Fechaoperacion.Year)
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
