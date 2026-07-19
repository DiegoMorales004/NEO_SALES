
/*Limpiar antes de cargar scripts*/
DELETE FROM SALE_STATUS
DELETE FROM CUSTOMER
DELETE FROM [PRODUCT]
DELETE FROM SALE_DETAIL
DELETE FROM SALE

-- --------------------------------------------------

/*Insersar estados de ventas*/

INSERT INTO SALE_STATUS (ID, [NAME], [DESCRIPTION])
VALUES
    (1, 'PENDIENTE', 'La venta se encuentra en proceso de construcción'), -- SOLO SE AGREGO PRODUCTOS A CARRITO
    (2, 'CONFIRMADA', 'La venta fue confirmada correctamente'), -- LA VENTA SE HIZO
    (3, 'CANCELADA', 'La venta fue cancelada antes de completarse'), -- SE CANCELO EL CARRITO
    (4, 'EXPIRADA', 'La venta no fue completada dentro del tiempo permitido'); -- PASO EL TIEMPO DEL CARRITO

/*Insertar clientes*/
INSERT INTO CUSTOMER (ID, NIT, [NAME], EMAIL, USER_CREATE)
VALUES
    (NEWID(), '900123456', 'Juan Pérez', 'juan.perez@correo.com', 'SISTEMA'),
    (NEWID(), '900234567', 'María González', 'maria.gonzalez@correo.com', 'SISTEMA'),
    (NEWID(), '900345678', 'Carlos Rodríguez', 'carlos.rodriguez@correo.com', 'SISTEMA'),
    (NEWID(), '900456789', 'Ana Martínez', 'ana.martinez@correo.com', 'SISTEMA'),
    (NEWID(), '900567890', 'Luis Hernández', 'luis.hernandez@correo.com', 'SISTEMA');
GO

/*Productos iniciales*/

INSERT INTO [PRODUCT] (ID, [NAME], PRICE, STOCK, [STATUS], USER_CREATE)
VALUES
    (NEWID(), 'Laptop', 890.99, 15, 1, 'SISTEMA'),
    (NEWID(), 'Monitor', 243.99, 25, 1, 'SISTEMA'),
    (NEWID(), 'Teclado', 43.99, 50, 1, 'SISTEMA'),
    (NEWID(), 'Mouse', 22.99, 75, 1, 'SISTEMA'),
    (NEWID(), 'Audífonos', 79.99, 40, 1, 'SISTEMA'),
    (NEWID(), 'Hub USB', 12.99, 100, 1, 'SISTEMA'),
    (NEWID(), 'Cámara adaptable', 59.99, 30, 1, 'SISTEMA'),
    (NEWID(), 'Silla de escritorio', 199.99, 20, 1, 'SISTEMA'),
    (NEWID(), 'Mochila', 69.99, 35, 1, 'SISTEMA'),
    (NEWID(), 'Disco duro externo', 129.99, 18, 1, 'SISTEMA');
GO
