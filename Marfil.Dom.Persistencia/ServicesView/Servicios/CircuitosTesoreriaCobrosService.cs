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
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public class CircuitosTesoreriaCobrosService : GestionService<CircuitoTesoreriaCobrosModel, Persistencia.CircuitosTesoreriaCobros>
    {

        #region CONSTRUCTOR
        public CircuitosTesoreriaCobrosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        #endregion
        
        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Tipocircuito", "Situacioninicialdescripcion", "Situacionfinaldescripcion", "Descripcion"};
            var propiedades = Helpers.Helper.getProperties<CircuitoTesoreriaCobrosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.PrimaryColumnns = new[] { "Id" };

            return model;
        }

        public override string GetSelectPrincipal()
        {
            var result = new StringBuilder();
            result.Append(" select c.*,sitini.descripcion as Situacioninicialdescripcion,sitfin.descripcion as Situacionfinaldescripcion ");
            result.Append(" from CircuitosTesoreriaCobros as c ");
            result.Append(" left join SituacionesTesoreria as sitini on sitini.cod = c.situacioninicial ");
            result.Append(" left join SituacionesTesoreria as sitfin on sitfin.cod = c.situacionfinal ");
            result.AppendFormat(" where c.empresa ='{0}' ", _context.Empresa);

            return result.ToString();
        }

        #endregion     

        #region CRUD
        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as CircuitoTesoreriaCobrosModel;

                //Llamamos al base
                base.create(obj);

                //Guardamos los cambios
                _db.SaveChanges();
                tran.Complete();
            }
        }
        #endregion

        public IEnumerable<CircuitoTesoreriaCobrosModel> GetCircuitosTesoreria(string tipoasignacion, bool soloiniciales)
        {
            var intasignacion = Int32.Parse(tipoasignacion);
            var circuitos = new List<CircuitoTesoreriaCobrosModel>();

            var inicialcobros = _db.SituacionesTesoreria.Where(f => f.valorinicialcobros == true).Select(f => f.cod).SingleOrDefault();
            var inicialpagos = _db.SituacionesTesoreria.Where(f => f.valorinicialpagos == true).Select(f => f.cod).SingleOrDefault();

            if (intasignacion ==  (int)TipoCircuito.Cobros)
            {
                var circuitosBD = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.tipocircuito == (int)TipoCircuito.Cobros);

                if(soloiniciales)
                {
                    circuitosBD = circuitosBD.Where(f => f.situacioninicial == inicialcobros && f.documentocartera == true);
                }

                foreach(var c in circuitosBD)
                {
                    circuitos.Add(new CircuitoTesoreriaCobrosModel() { Id = c.id, Descripcion = c.descripcion });
                }
            }

            else if (intasignacion == (int)TipoCircuito.Pagos)
            {
                var circuitosBD = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.tipocircuito == (int)TipoCircuito.Pagos);

                if (soloiniciales)
                {
                    circuitosBD = circuitosBD.Where(f => f.situacioninicial == inicialpagos);
                }

                foreach (var c in circuitosBD)
                {
                    circuitos.Add(new CircuitoTesoreriaCobrosModel() { Id = c.id, Descripcion = c.descripcion });
                }
            }

            else if (intasignacion != (int)TipoCircuito.Cobros && intasignacion != (int)TipoCircuito.Pagos)
            {
                circuitos.AddRange(_db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa)
                    .Select(f => new CircuitoTesoreriaCobrosModel() { Id = f.id, Descripcion = f.descripcion }));
            }
            return circuitos;
        }

        //Asistente

        public bool ActualizarFechaPago(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            return _db.CircuitosTesoreriaCobros.Any(f => f.empresa == Empresa && f.id == idcircuito && f.fechapago == true);
        }

        public bool SolicitarDatosDocumento(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            return _db.CircuitosTesoreriaCobros.Any(f => f.empresa == Empresa && f.id == idcircuito && f.datosdocumento == true);
        }

        public bool ImporteCargo2(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);

            //Importe 2 puede ser importe cuenta cargo2, imp2-imp3 o imp2+imp3
            return _db.CircuitosTesoreriaCobros.Any(f => f.empresa == Empresa && f.id == idcircuito && f.importecuentacargo2 == (int)TipoImporte.Importecuentacargo2 
                || f.importecuentacargo2 == (int)TipoImporte.Importe2masimporte3 || f.importecuentacargo2 == (int)TipoImporte.Importe2menosimporte3);
        }

        public bool ImporteAbono2(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            return _db.CircuitosTesoreriaCobros.Any(f => f.empresa == Empresa && f.id == idcircuito && f.importecuentaabono2 == (int)TipoImporte.Importecuentaabono2
                || f.importecuentaabono2 == (int)TipoImporte.Importe2masimporte6 || f.importecuentaabono2 == (int)TipoImporte.Importe2menosimporte6);
        }

        public bool Remesa(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            return _db.CircuitosTesoreriaCobros.Any(f => f.empresa == Empresa && f.id == idcircuito && f.fecharemesa == true && f.situacionfinal == "R");
        }

        public CuentasModel Cuentacargo2(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            var cuentasService = new CuentasService(_context);
            var cuenta = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Select(f => f.cuentacargo2).SingleOrDefault() ?? "";
            return !String.IsNullOrEmpty(cuenta) ? cuentasService.get(cuenta) as CuentasModel : null;
        }

        public CuentasModel Cuentaabono2(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            var cuentasService = new CuentasService(_context);
            var cuenta = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Select(f => f.cuentaabono2).SingleOrDefault() ?? "";
            return !String.IsNullOrEmpty(cuenta) ? cuentasService.get(cuenta) as CuentasModel : null;
        }

        public bool CodigoManual(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            var circuitoBD = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();
            var CM = "*CM*";
            bool codigo = false;
            
            if(!String.IsNullOrEmpty(circuitoBD.desccuentaabono1) && !codigo)
            {
                codigo = circuitoBD.desccuentaabono1.Contains(CM) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.desccuentaabono2) && !codigo)
            {
                codigo = circuitoBD.desccuentaabono2.Contains(CM) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.desccuentaabonorel) && !codigo)
            {
                codigo = circuitoBD.desccuentaabonorel.Contains(CM) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.desccuentacargo1) && !codigo)
            {
                codigo = circuitoBD.cuentacargo1.Contains(CM) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.desccuentacargo2) && !codigo)
            {
                codigo = circuitoBD.desccuentacargo2.Contains(CM) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.desccuentacargorel) && !codigo)
            {
                codigo = circuitoBD.desccuentacargorel.Contains(CM) ? true : false;
            }

            return codigo;
        }

        public bool ExisteCobrador(string circuito)
        {
            var idcircuito = Int32.Parse(circuito);
            var circuitoBD = _db.CircuitosTesoreriaCobros.Where(f => f.empresa == Empresa && f.id == idcircuito).Single();
            var C = "C";
            bool cobrador = false;

            if(circuitoBD.actualizarcobrador == true)
            {
                cobrador = true;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentaabono1) && !cobrador)
            {
                cobrador = circuitoBD.cuentaabono1.Contains(C) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentaabono2) && !cobrador)
            {
                cobrador = circuitoBD.cuentaabono2.Contains(C) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentaabonorel) && !cobrador)
            {
                cobrador = circuitoBD.cuentaabonorel.Contains(C) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentacargo1) && !cobrador)
            {
                cobrador = circuitoBD.cuentacargo1.Contains(C) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentacargo2) && !cobrador)
            {
                cobrador = circuitoBD.cuentacargo2.Contains(C) ? true : false;
            }

            if (!String.IsNullOrEmpty(circuitoBD.cuentacargorel) && !cobrador)
            {
                cobrador = circuitoBD.cuentacargorel.Contains(C) ? true : false;
            }

            return cobrador;
        }
    }
}
