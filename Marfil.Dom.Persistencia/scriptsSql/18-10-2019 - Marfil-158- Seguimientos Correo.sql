--Publicado en azure

drop table Seguimientos;

create table Seguimientos (
		empresa varchar(4) not null,
		id int IDENTITY(1,1) not null,
		origen varchar(15) not null,
		tipo int,
		usuario varchar(15),
		asunto varchar(100),
		fechadocumento datetime,
		fkempresa varchar(15),
		fkcontacto varchar(15),
		fketapa varchar(10),
		fkaccion varchar(3),
		fkclavecoste varchar(3),
		coste int,
		notas varchar(max),
		fkdocumentorelacionado varchar(3),
		cerrado bit,
		fecharesolucion datetime,
		fkreaccion varchar(3),
		fkreferenciadocumentorelacionado varchar(30),
		fechaproximoseguimiento datetime,

		primary key(empresa,id,origen)
		);

create table SeguimientosCorreo (
	empresa varchar(4) not null,
	fkseguimientos int not null,
	fkorigen varchar(15) not null,
	id int not null,
	correo varchar(50),
	asunto varchar(100),
	fecha datetime,

	primary key(empresa,fkseguimientos,fkorigen,id),
	foreign key(empresa,fkseguimientos,fkorigen) references Seguimientos(empresa,id,origen)
	);