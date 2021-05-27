--Publicado en azure

create table ArticulosComponentes (
	empresa varchar(4) not null,
	fkarticulo varchar(15) not null,
	id int not null,
	idcomponente varchar(15),
	descripcioncomponente nvarchar(120),
	piezas int,
	largo float,
	ancho float,
	grueso float,
	merma int,

	primary key(empresa,fkarticulo,id),
	foreign key(empresa,fkarticulo) references Articulos(empresa,id)
	);