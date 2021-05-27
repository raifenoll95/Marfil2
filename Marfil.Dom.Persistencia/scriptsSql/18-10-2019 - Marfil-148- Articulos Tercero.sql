--Publicado en azure

create table ArticulosTercero (
	empresa varchar(4) not null,
	codarticulo varchar(15) not null,
	id int not null,
	codtercero varchar(15) not null,
	descripciontercero nvarchar(120),
	codarticulotercero varchar(15) not null,
	descripcionarticulotercero nvarchar(120),

	primary key(empresa,codarticulo,id),
	foreign key(empresa,codarticulo) references Articulos(empresa,id)
	);