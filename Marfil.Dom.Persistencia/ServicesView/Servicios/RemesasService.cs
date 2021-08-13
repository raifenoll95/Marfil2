using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class RemesasService : GestionService<RemesasModel, Remesas>
    {
        #region CTR

        public RemesasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Tipovencimiento", "Referenciaremesa", "Fecharemesa", "Fkcuentastesoreria", "Descripcioncuenta", "Referencia", "Importegiro" };
            var propiedades = Helpers.Helper.getProperties<RemesasModel>();

            model.PrimaryColumnns = new[] { "Id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Tipovencimiento", 0);
            model.OrdenColumnas.Add("Referenciaremesa", 1);
            model.OrdenColumnas.Add("Fecharemesa", 2);
            model.OrdenColumnas.Add("Fkcuentastesoreria", 3);
            model.OrdenColumnas.Add("Descripcioncuenta", 4);
            model.OrdenColumnas.Add("Referencia", 5);
            model.OrdenColumnas.Add("Importegiro", 6);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select i.id,i.tipovencimiento,i.referenciaremesa,i.fecharemesa,i.fkcuentastesoreria,c.descripcion as Descripcioncuenta,i.referencia,i.importegiro from remesas as i " +
                " inner join cuentas as c on c.id=i.fkcuentastesoreria and c.empresa=i.empresa" +
                " where i.empresa='{0}'", Empresa);
        }

        #endregion

        public int GetCuadernoId(string cuaderno)
        {
            return _db.CuadernosBancarios.Where(f => f.clave == cuaderno && f.empresa == Empresa).FirstOrDefault().id;
        }
        public CuadernosBancarios GetCuaderno(string cuaderno)
        {
            return _db.CuadernosBancarios.Where(f => f.clave == cuaderno && f.empresa == Empresa).FirstOrDefault();
        }
        public int? GetFormato(string cuaderno)
        {
            return _db.CuadernosBancarios.Where(f => f.clave == cuaderno && f.empresa == Empresa).FirstOrDefault().formato;
        }

        public List<CuadernosBancariosLin> GetCuadernoCabecera(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Cabecera").OrderBy(f => f.orden).ThenBy(f => f.posicion).ToList();
        }
        public List<CuadernosBancariosLin> GetCuadernoDetalle(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Detalle").OrderBy(f => f.orden).ThenBy(f => f.posicion).ToList();
        }
        public List<CuadernosBancariosLin> GetCuadernoTotal(int idCuaderno)
        {
            return _db.CuadernosBancariosLin.Where(f => f.idCab == idCuaderno && f.empresa == Empresa && f.registro == "Total").OrderBy(f => f.orden).ThenBy(f => f.posicion).ToList();
        }

        public string GetMapeo(string etiqueta, string cuaderno)
        {
            try
            {

                var tablaMapeo = _db.MapeoRemesas.Where(f => f.etiqueta == etiqueta).FirstOrDefault().tabla;
                //var tablas = _db.MapeoRemesas.ToList();
                var cuadernoBancario = GetCuaderno(cuaderno);
                //var campoMapeo = "";

                if (tablaMapeo != null)
                {
                    //campoMapeo = _db.MapeoRemesas.Where(f => f.etiqueta == etiqueta && f.tabla == tablaMapeo).FirstOrDefault().campo;

                    switch (tablaMapeo)
                    {
                        case "BancosMandatos":

                            if (etiqueta == "Iban Empresa")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                return _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                            }
                            else if (etiqueta == "Bic Empresa")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                return _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().bic;
                            }
                            else if (etiqueta == "Sufijo Acreedor")
                            {
                                return "**SufijoAcreedor**";
                            }
                            else if (etiqueta == "Contrato confirming")
                            {
                                return "**ContratoConfirming**";
                            }
                            else if (etiqueta == "Iban código")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                var iban = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                                return iban.Substring(0,4);
                            }
                            else if (etiqueta == "CCC1")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                var iban = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                                return iban.Substring(5, 4);
                            }
                            else if (etiqueta == "CCC2")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                var iban = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                                return iban.Substring(9, 4);
                            }
                            else if (etiqueta == "DC")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                var iban = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                                return iban.Substring(13, 4);
                            }
                            else if (etiqueta == "Cuenta")
                            {
                                var fkCuentaSalidasVariasAlmacen = _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                                var iban = _db.BancosMandatos.Where(f => f.empresa == Empresa && f.fkcuentas == fkCuentaSalidasVariasAlmacen).FirstOrDefault().iban;
                                return iban.Substring(15, 10);
                            }

                            break;
                        case "Direcciones":

                            if (etiqueta == "Localidad")
                            {
                                return _db.Direcciones.Where(f => f.empresa == Empresa && f.fkentidad == Empresa && f.tipotercero == -1).FirstOrDefault().poblacion;
                            }
                            else if (etiqueta == "Direccion")
                            {
                                return _db.Direcciones.Where(f => f.empresa == Empresa && f.fkentidad == Empresa && f.tipotercero == -1).FirstOrDefault().direccion;
                            }

                            break;
                        case "Empresas":

                            if (etiqueta == "Código cuenta")
                            {
                                return _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().fkCuentaSalidasVariasAlmacen;
                            }
                            else if (etiqueta == "Empresa")
                            {
                                return _db.Empresas.Where(f => f.id == Empresa).FirstOrDefault().nombre;
                            }
                            else if (etiqueta == "Nif Ordenante")
                            {
                                return "**NifOrdenante**";
                            }
                            else if (etiqueta == "Nombre ordenante")
                            {
                                return "**NombreOrdenante**";
                            }

                            break;
                        case "Provincias":

                            if (etiqueta == "Provincia")
                            {
                                var codProv = _db.Direcciones.Where(f => f.empresa == Empresa && f.fkentidad == Empresa && f.tipotercero == -1).FirstOrDefault().fkprovincia;
                                return _db.Provincias.Where(f => f.id == codProv).FirstOrDefault().nombre;
                            }

                            break;
                        case "Remesas":

                            if (etiqueta == "Fecha envío fichero")
                            {
                                return DateTime.Today.ToString();
                            }
                            else if (etiqueta == "Fecha envío ordenes")
                            {
                                return DateTime.Today.ToString(); ;
                            }

                            break;
                        case "Vencimientos":

                            if (etiqueta == "Fecha de vencimiento")
                            {
                                return "**FechaVencimiento**";
                            }

                            break;
                        default:
                            return "**CampoMapeo**";
                            break;
                    }

                    return "**campo**";

                }
                else
                {
                    return "**TablaMapeo**";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
