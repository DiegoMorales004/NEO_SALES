CREATE OR ALTER PROCEDURE sp_SALE
(
    @p_ACTION VARCHAR(16),

    @p_ID UNIQUEIDENTIFIER = NULL,
    @p_ID_CUSTOMER UNIQUEIDENTIFIER = NULL,
    @p_STATUS_ID INT = NULL,
    @p_DATE DATETIME = NULL,
    @p_DATETIME_EXPIRE DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_ACTION = 'GET'
    BEGIN
        SELECT
            ID,
            ID_CUSTOMER,
            STATUS_ID,
            [DATE],
            DATETIME_EXPIRE
        FROM SALE
    END;


    IF @p_ACTION = 'GET_BY_ID'
    BEGIN
        SELECT
            ID,
            ID_CUSTOMER,
            STATUS_ID,
            [DATE],
            DATETIME_EXPIRE
        FROM SALE
        WHERE ID = @p_ID;
    END;

    IF @p_ACTION = 'GET_BY_STATUS_ID'
    BEGIN
        SELECT
            ID,
            ID_CUSTOMER,
            STATUS_ID,
            [DATE],
            DATETIME_EXPIRE
        FROM SALE
        WHERE STATUS_ID = @p_STATUS_ID;
    END;


    IF @p_ACTION = 'GET_BY_ID_CUSTOMER'
    BEGIN
        SELECT
            ID,
            ID_CUSTOMER,
            STATUS_ID,
            [DATE],
            DATETIME_EXPIRE
        FROM SALE
        WHERE
            ID_CUSTOMER = @p_ID_CUSTOMER
    END;


    IF @p_ACTION = 'INSERT'
    BEGIN
        INSERT INTO SALE
        (
            ID,
            ID_CUSTOMER,
            STATUS_ID,
            [DATE],
            DATETIME_EXPIRE
        )
        OUTPUT
            INSERTED.ID,
            INSERTED.ID_CUSTOMER,
            INSERTED.STATUS_ID,
            INSERTED.[DATE],
            INSERTED.DATETIME_EXPIRE
        VALUES
        (
            NEWID(),
            @p_ID_CUSTOMER,
            @p_STATUS_ID,
            ISNULL(@p_DATE, GETDATE()),
            @p_DATETIME_EXPIRE
        );
    END;


    IF @p_ACTION = 'UPDATE'
    BEGIN
        UPDATE SALE
        SET
            ID_CUSTOMER = @p_ID_CUSTOMER,
            STATUS_ID = @p_STATUS_ID,
            [DATE] = @p_DATE,
            DATETIME_EXPIRE = @p_DATETIME_EXPIRE
        OUTPUT
            INSERTED.ID,
            INSERTED.ID_CUSTOMER,
            INSERTED.STATUS_ID,
            INSERTED.[DATE],
            INSERTED.DATETIME_EXPIRE
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'DELETE'
    BEGIN
        DELETE FROM SALE
        WHERE ID = @p_ID;
    END;
END;
GO