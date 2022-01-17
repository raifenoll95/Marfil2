﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IConfiguracionService
    {

    }

    public class ConfiguracionService : GestionService<ConfiguracionModel, Configuracion>, IConfiguracionService
    {
        #region Members

        public const int Id = 1;        

        #endregion

        #region CTR

        public ConfiguracionService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            
        }

        #endregion

        public void CreateOrUpdate(ConfiguracionModel model)
        {
            if (exists(model.Id.ToString()))
                edit(model);
            else 
                create(model);
        }

        public ConfiguracionModel GetModel()
        {
            if (exists(Id.ToString()))
                return get(Id.ToString()) as ConfiguracionModel;

            var fmodel =new FModel();
            return fmodel.GetModel<ConfiguracionModel>(_context);
        }

        public int GetCargaDatos()
        {
            return _db.Configuracion.FirstOrDefault()?.cargadatos ?? 0;
        }

        public void SetCargaDatos(int valor)
        {            
            var model = _db.Configuracion.FirstOrDefault() ?? _db.Configuracion.Create();
            model.id = 1;
            model.cargadatos = valor;
            _db.Configuracion.AddOrUpdate(model);
            _db.SaveChanges(); 
        }

        public int GetDiasDesvalorizacion()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("Cancelacionriesgoremesa");
            var dias = int.Parse(nodo[0].InnerText);
            return dias;
        }

        public DateTime GetFechaHastaEjercicio()
        {
            var idejerc = int.Parse(_context.Ejercicio);
            return (DateTime)_db.Ejercicios.Where(f => f.id == idejerc).FirstOrDefault().hasta;
        }

        public string GetSerieContable()
        {
            return _db.SeriesContables.Where(f => f.empresa == Empresa && f.tipodocumento == "AST").Select(f => f.id).SingleOrDefault() ?? "";
        }

        public string GetInvertirAsiento()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("Invertirasiento");
            var invertir = nodo[0].InnerText;
            return invertir;
        }

        public string GetComentarioIni()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("ComentarioExistenciasIniciales");
            var comentario = nodo[0].InnerText;
            return comentario;
        }

        public string GetComentarioFin()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("ComentarioExistenciasFinales");
            var comentario = nodo[0].InnerText;
            return comentario;
        }

        public string GetComentarioDetalle()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("ComentarioCuentasDetalle");
            var comentario = nodo[0].InnerText;
            return comentario;
        }

        public string GetComentarioHaber()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("ComentarioHaberPYG");
            var comentario = nodo[0].InnerText;
            return comentario;
        }

        public string GetComentarioDebe()
        {
            XmlDocument doc = new XmlDocument();
            var datos = _db.Configuracion.FirstOrDefault().xml;
            doc.LoadXml(datos);
            XmlElement datosParse = doc.DocumentElement;

            XmlNodeList nodo = datosParse.GetElementsByTagName("ComentarioDebePYG");
            var comentario = nodo[0].InnerText;
            return comentario;
        }
    }

}
