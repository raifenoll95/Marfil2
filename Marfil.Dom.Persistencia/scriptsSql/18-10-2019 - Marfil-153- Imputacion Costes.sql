--Publicado en azure

create table ImputacionCostes (
	empresa varchar(4) not null,
	id int IDENTITY(1,1) NOT NULL,
	fkejercicio int not null,
	fkseries varchar(3) not null,
	identificadorsegmento nvarchar(12),
	referencia nvarchar(30),
	fkoperarios varchar(15),
	fechadocumento datetime,
	notas ntext,
	fkestados nvarchar(10) not null,

	primary key(empresa, id)
	);



create table ImputacionCostesLin (
	empresa varchar(4) not null,
	fkimputacioncostes int not null,
	id int not null,
	fkarticulos nvarchar(15),
	descripcion nvarchar(120),
	lote nvarchar(12),
	tabla int,
	caja int,
	bundle nvarchar(2),
	cantidad float,
	largo float,
	ancho float,
	grueso float,
	metros float,
	precio float,
	porcentajedescuento float,
	importedescuento float,
	fktiposiva varchar(3),
	porcentajeiva float,
	cuotaiva float,
	fkunidades varchar(2),
	notas ntext,
	canal nvarchar(3),
	revision nvarchar(1),
	decimalesmonedas int,
	decimalesmedidas int,
	orden int,
	flagidentifier uniqueidentifier,
	costeadicionalmaterial float,
	costeadicionalportes float,
	costeadicionalotro float,
	costeadicionalvariable float,

	primary key(empresa,fkimputacioncostes,id),
	foreign key(empresa,fkimputacioncostes) references ImputacionCostes (empresa, id)
	)


create table ImputacionCostescostesadicionales (

	empresa varchar(4) not null,
	fkimputacioncostes int not null,
	id int not null,
	tipodocumento int not null,
	referenciadocumento nvarchar(30),
	importe float,
	porcentaje float,
	total float,
	tipocoste int not null,
	tiporeparto int not null,
	notas ntext,

	primary key (empresa,fkimputacioncostes,id),
	foreign key (empresa, fkimputacioncostes) references ImputacionCostes (empresa,id)
);