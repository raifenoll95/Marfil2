--Publicado en azure

delete from TablasvariasLin where fktablasvarias = 21;
delete from Tablasvarias where id = 21;

create table GrupoMateriales (
	empresa varchar(4) not null,
	cod varchar(7) not null,
	descripcion varchar(20),
	inglesDescripcion varchar(20),
	fkcarpetas uniqueidentifier,

	primary key(empresa, cod));