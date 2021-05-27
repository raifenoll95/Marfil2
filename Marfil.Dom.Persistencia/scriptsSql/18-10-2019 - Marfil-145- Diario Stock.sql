--Publicado en azure

alter view Diariostock as
SELECT        SUBSTRING(m.fkarticulos, 3, 3) AS Expr1, m.empresa, m.id, m.fecha, m.fkalmacenes, m.fkarticulos, m.referenciaproveedor, m.lote, m.loteid, m.tag, m.fkunidadesmedida, m.cantidad, m.largo, m.ancho, m.grueso, 
							 s.metros, m.documentomovimiento, m.integridadreferencialflag, m.fkcontadorlote, m.fkusuarios, m.tipooperacion, m.fkalmaceneszona, m.fkcalificacioncomercial, m.fktipograno, m.fktonomaterial, m.fkincidenciasmaterial, m.fkvariedades, 
							 m.categoriamovimiento, a.descripcion AS Almacendescripcion, ISNULL(u.usuario, 'admin') AS Usuario, un.codigounidad AS Um, az.descripcion AS Almaceneszonas, cc.Descripcion AS Calificacioncomercial, 
							 tg.Descripcion AS Tipograno, tn.Descripcion AS Tonomaterial, inc.descripcion AS Incidencia, ml.descripcion AS Variedad, CAST(m.fecha AS time) AS Hora, art.descripcion AS Descripcionarticulos, 
							 ISNULL(m.documentomovimiento.value('(/*/Referencia/text())[1]', 'varchar(40)'), '') AS Documentoreferencia, m.tipoalmacenlote
	FROM            dbo.Movimientosstock AS m INNER JOIN
							 dbo.Almacenes AS a ON a.empresa = m.empresa AND a.id = m.fkalmacenes INNER JOIN
							 dbo.Unidades AS un ON un.empresa = m.empresa AND un.id = m.fkunidadesmedida INNER JOIN
							 dbo.Articulos AS art ON art.empresa = m.empresa AND art.id = m.fkarticulos LEFT OUTER JOIN
							 dbo.Usuarios AS u ON m.fkusuarios = u.id LEFT OUTER JOIN
							 dbo.AlmacenesZona AS az ON az.empresa = m.empresa AND az.fkalmacenes = m.fkalmacenes AND az.id = m.fkalmaceneszona LEFT OUTER JOIN
							 dbo.Calificacioncomercial AS cc ON cc.Valor = m.fkcalificacioncomercial LEFT OUTER JOIN
							 dbo.Tipograno AS tg ON tg.Valor = m.fktipograno LEFT OUTER JOIN
							 dbo.Tonomaterial AS tn ON tn.Valor = m.fktonomaterial LEFT OUTER JOIN
							 dbo.Incidencias AS inc ON inc.empresa = m.empresa AND inc.id = m.fkincidenciasmaterial LEFT OUTER JOIN
							 dbo.Stockactual AS s ON s.fkarticulos = m.fkarticulos AND s.empresa = m.empresa AND s.lote = m.lote AND s.loteid = m.loteid LEFT OUTER JOIN
							 dbo.MaterialesLin AS ml ON ml.empresa = m.empresa AND ml.codigovariedad = m.fkvariedades AND ml.fkmateriales = SUBSTRING(m.fkarticulos, 3, 3)