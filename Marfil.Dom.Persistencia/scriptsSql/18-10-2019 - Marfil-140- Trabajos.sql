--Publicado en azure

alter table Trabajos add precio float;

update Trabajos
set precio = 0


create table TrabajosLin (
	empresa varchar(4) not null,
	fktrabajos varchar(5) not null,
	id int not null,
	año smallint,
	precio float,

	primary key(empresa, fktrabajos, id),
	foreign key(empresa, fktrabajos) references Trabajos (empresa, id)
	);