Para la correcta ejecucion de los scripts, deben de ejecutarse en el orden de numeracion que llevan como nombre.

Por ejemplo, ejecutar en este orden:
	1 - ....
	2 - ...


Los scripts de insert, en la seccion de SALE_STATUS no modificar el orden de insercion, puede provocar errores en el estado que se tomo en cuenta durante el desarrollo. 
Debe de ser este el orden 
	Pendiente = 1
	Confirmada = 2
	Cancelada = 3
	Expirada = 4


Para los procedimientos almacenados, basta con que se haya creado ya la base de datos para poder ejecutarlos. Estos se encuentran en la carpeta "SPs" de 
esta misma ruta. 



