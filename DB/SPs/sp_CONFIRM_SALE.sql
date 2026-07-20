/*
=========================================================
     1. Verificar que la venta exista y este en estado PENDIENTE
     2. Verificar que no haya expirado
     3. Validar que haya stock suficiente para todos los productos
     4. Si falta stock, informar cuáles productos no cumplen
     5. Si hay stock, descontarlo y marcar la venta como CONFIRMADA
========================================================= 
*/
CREATE OR ALTER PROCEDURE sp_CONFIRM_SALE
(
    @p_ID_SALE     UNIQUEIDENTIFIER,
    @p_USER_UPDATE VARCHAR(128) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        IF @p_ID_SALE IS NULL
        BEGIN
            SELECT CAST(0 AS BIT) AS SUCCESS, 'El ID de venta es requerido' AS MESSAGE;
            RETURN;
        END;

        DECLARE @STATUS_PENDIENTE  INT,
                @STATUS_CONFIRMADA INT;

        SELECT @STATUS_PENDIENTE  = ID FROM SALE_STATUS WHERE [NAME] = 'PENDIENTE';
        SELECT @STATUS_CONFIRMADA = ID FROM SALE_STATUS WHERE [NAME] = 'CONFIRMADA';

        IF @STATUS_PENDIENTE IS NULL OR @STATUS_CONFIRMADA IS NULL
        BEGIN
            THROW 50001, 'Catalogo SALE_STATUS incompleto', 1;
        END;

        BEGIN TRANSACTION;

        /* 
        ---------------------------------------------------
           1) VALIDAR VENTA
        --------------------------------------------------- 
        */
        DECLARE @l_CurrentStatus INT, @l_ExpireDate DATETIME;

        SELECT
            @l_CurrentStatus = STATUS_ID
        FROM SALE WITH (UPDLOCK, HOLDLOCK)
        WHERE ID = @p_ID_SALE;

        IF @l_CurrentStatus IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT CAST(0 AS BIT) AS SUCCESS, 'La venta indicada no existe' AS MESSAGE;
            RETURN;
        END;

        IF @l_CurrentStatus <> @STATUS_PENDIENTE
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT CAST(0 AS BIT) AS SUCCESS, 'La venta no está en estado PENDIENTE, no se puede confirmar' AS MESSAGE;
            RETURN;
        END;

        /* 
        ---------------------------------------------------
           2) AGRUPAR EL DETALLE POR PRODUCTO
        --------------------------------------------------- 
        */
        DECLARE @l_SaleItems TABLE
        (
            ID_PRODUCT     UNIQUEIDENTIFIER PRIMARY KEY,
            TOTAL_QUANTITY INT NOT NULL
        );

        INSERT INTO @l_SaleItems (ID_PRODUCT, TOTAL_QUANTITY)
        SELECT ID_PRODUCT, SUM(QUANTITY)
        FROM SALE_DETAIL
        WHERE ID_SALE = @p_ID_SALE
        GROUP BY ID_PRODUCT;

        IF NOT EXISTS (SELECT 1 FROM @l_SaleItems)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT CAST(0 AS BIT) AS SUCCESS, 'La venta no tiene productos asociados' AS MESSAGE;
            RETURN;
        END;

        /* 
        ---------------------------------------------------
           3) BLOQUEAR LOS PRODUCTOS INVOLUCRADOS
        --------------------------------------------------- 
        */
        DECLARE @l_LockedProducts TABLE
        (
            ID_PRODUCT UNIQUEIDENTIFIER PRIMARY KEY,
            STOCK      INT NOT NULL
        );

        INSERT INTO @l_LockedProducts (ID_PRODUCT, STOCK)
        SELECT P.ID, P.STOCK
        FROM [PRODUCT] P WITH (UPDLOCK, HOLDLOCK)
        WHERE P.ID IN (SELECT ID_PRODUCT FROM @l_SaleItems)
        ORDER BY P.ID;

        /* ---------------------------------------------------
           4) VALIDAR STOCK
        --------------------------------------------------- */
        IF EXISTS
        (
            SELECT 1
            FROM @l_SaleItems SI
            INNER JOIN @l_LockedProducts LP
                ON LP.ID_PRODUCT = SI.ID_PRODUCT
            WHERE LP.STOCK < SI.TOTAL_QUANTITY
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                CAST(0 AS BIT) AS SUCCESS,
                'No hay stock suficiente para uno o más productos' AS MESSAGE;

            SELECT
                P.ID                          AS ID_PRODUCT,
                P.[NAME],
                SI.TOTAL_QUANTITY             AS QUANTITY_REQUESTED,
                LP.STOCK                      AS STOCK_AVAILABLE,
                SI.TOTAL_QUANTITY - LP.STOCK  AS QUANTITY_MISSING
            FROM @l_SaleItems SI
            INNER JOIN @l_LockedProducts LP ON LP.ID_PRODUCT = SI.ID_PRODUCT
            INNER JOIN [PRODUCT] P        ON P.ID = SI.ID_PRODUCT
            WHERE LP.STOCK < SI.TOTAL_QUANTITY;

            RETURN;
        END;

        /* 
        ---------------------------------------------------
           5) DESCONTAR STOCK
        --------------------------------------------------- 
        */
        UPDATE P
        SET
            P.STOCK           = P.STOCK - SI.TOTAL_QUANTITY,
            P.DATETIME_UPDATE = GETDATE(),
            P.USER_UPDATE     = ISNULL(@p_USER_UPDATE, SYSTEM_USER)
        FROM [PRODUCT] P
        INNER JOIN @l_SaleItems SI ON SI.ID_PRODUCT = P.ID;

        /* ---------------------------------------------------
           6) CONFIRMAR VENTA
        --------------------------------------------------- */
        UPDATE SALE
        SET STATUS_ID = @STATUS_CONFIRMADA
        WHERE ID = @p_ID_SALE;

        COMMIT TRANSACTION;

        SELECT
            CAST(1 AS BIT) AS SUCCESS,
            'Venta confirmada correctamente' AS MESSAGE;

        SELECT
            P.ID               AS ID_PRODUCT,
            P.[NAME],
            SI.TOTAL_QUANTITY  AS QUANTITY_SOLD,
            P.STOCK            AS STOCK_REMAINING
        FROM @l_SaleItems SI
        INNER JOIN [PRODUCT] P ON P.ID = SI.ID_PRODUCT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
