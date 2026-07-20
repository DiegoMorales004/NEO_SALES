CREATE OR ALTER PROCEDURE sp_SALE_DETAIL
(
    @p_ACTION VARCHAR(16),

    @p_ID UNIQUEIDENTIFIER = NULL,
    @p_ID_SALE UNIQUEIDENTIFIER = NULL,
    @p_ID_PRODUCT UNIQUEIDENTIFIER = NULL,
    @p_QUANTITY INT = NULL,
    @p_PRICE_UNIT DECIMAL(18, 2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_ACTION = 'GET'
    BEGIN
        SELECT
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        FROM SALE_DETAIL
    END;


    IF @p_ACTION = 'GET_BY_ID'
    BEGIN
        SELECT
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        FROM SALE_DETAIL
        WHERE ID = @p_ID;
    END;

    IF @p_ACTION = 'GET_BY_ID_SALE'
    BEGIN
        SELECT
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        FROM SALE_DETAIL
        WHERE ID_SALE = @p_ID_SALE;
    END;

    IF @p_ACTION = 'GET_BY_ID_PRODUCT'
    BEGIN
        SELECT
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        FROM SALE_DETAIL
        WHERE ID_PRODUCT = @p_ID_PRODUCT;
    END;

    IF @p_ACTION = 'SEARCH'
    BEGIN
        SELECT
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        FROM SALE_DETAIL
        WHERE
            ID_SALE = @p_ID_SALE
            OR
            ID_PRODUCT = @p_ID_PRODUCT;
    END;


    IF @p_ACTION = 'INSERT'
    BEGIN
        INSERT INTO SALE_DETAIL
        (
            ID,
            ID_SALE,
            ID_PRODUCT,
            QUANTITY,
            PRICE_UNIT
        )
        OUTPUT
            INSERTED.ID,
            INSERTED.ID_SALE,
            INSERTED.ID_PRODUCT,
            INSERTED.QUANTITY,
            INSERTED.PRICE_UNIT
        VALUES
        (
            NEWID(),
            @p_ID_SALE,
            @p_ID_PRODUCT,
            @p_QUANTITY,
            @p_PRICE_UNIT
        );
    END;


    IF @p_ACTION = 'UPDATE'
    BEGIN
        UPDATE SALE_DETAIL
        SET
            ID_SALE = @p_ID_SALE,
            ID_PRODUCT = @p_ID_PRODUCT,
            QUANTITY = @p_QUANTITY,
            PRICE_UNIT = @p_PRICE_UNIT
        OUTPUT
            INSERTED.ID,
            INSERTED.ID_SALE,
            INSERTED.ID_PRODUCT,
            INSERTED.QUANTITY,
            INSERTED.PRICE_UNIT
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'DELETE'
    BEGIN
        DELETE FROM SALE_DETAIL
        WHERE ID = @p_ID;
    END;
END;
GO