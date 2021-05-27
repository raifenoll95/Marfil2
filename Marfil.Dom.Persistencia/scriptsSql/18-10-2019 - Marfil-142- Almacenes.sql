--Publicado en azure

alter table almacenes add privado bit
update Almacenes set privado = 0 where privado is null