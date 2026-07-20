CREATE OR ALTER PROCEDURE sp_PRODUCT
(
    @p_ACTION VARCHAR(16),

    @p_ID UNIQUEIDENTIFIER = NULL,
    @p_NAME VARCHAR(256) = NULL,
    @p_PRICE DECIMAL(18, 2) = NULL,
    @p_STOCK INT = NULL,
    @p_STATUS BIT = NULL,
    @p_DATETIME_UPDATE DATETIME = NULL,
    @p_USER_CREATE VARCHAR(128) = NULL,
    @p_USER_UPDATE VARCHAR(128) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_ACTION = 'GET'
    BEGIN
        SELECT
            ID,
            [NAME],
            PRICE,
            STOCK,
            [STATUS],
            DATETIME_CREATE,
            DATETIME_UPDATE,
            USER_CREATE,
            USER_UPDATE
        FROM [PRODUCT]
    END;


    IF @p_ACTION = 'GET_BY_ID'
    BEGIN
        SELECT
            ID,
            [NAME],
            PRICE,
            STOCK,
            [STATUS],
            DATETIME_CREATE,
            DATETIME_UPDATE,
            USER_CREATE,
            USER_UPDATE
        FROM [PRODUCT]
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'SEARCH'
    BEGIN
        SELECT
            ID,
            [NAME],
            PRICE,
            STOCK,
            [STATUS],
            DATETIME_CREATE,
            DATETIME_UPDATE,
            USER_CREATE,
            USER_UPDATE
        FROM [PRODUCT]
        WHERE [NAME] LIKE '%' + @p_NAME + '%';
    END;


    IF @p_ACTION = 'INSERT'
    BEGIN
        INSERT INTO [PRODUCT]
        (
            ID,
            [NAME],
            PRICE,
            STOCK,
            [STATUS],
            USER_CREATE
        )
        OUTPUT
            INSERTED.ID,
            INSERTED.[NAME],
            INSERTED.PRICE,
            INSERTED.STOCK,
            INSERTED.[STATUS],
            INSERTED.DATETIME_CREATE,
            INSERTED.DATETIME_UPDATE,
            INSERTED.USER_CREATE,
            INSERTED.USER_UPDATE
        VALUES
        (
            NEWID(),
            @p_NAME,
            @p_PRICE,
            @p_STOCK,
            @p_STATUS,
            @p_USER_CREATE
        );
    END;


    IF @p_ACTION = 'UPDATE'
    BEGIN
        UPDATE [PRODUCT]
        SET
            [NAME] = @p_NAME,
            PRICE = @p_PRICE,
            STOCK = @p_STOCK,
            [STATUS] = @p_STATUS,
            DATETIME_UPDATE = @p_DATETIME_UPDATE,
            USER_UPDATE = @p_USER_UPDATE
        OUTPUT
            INSERTED.ID,
            INSERTED.[NAME],
            INSERTED.PRICE,
            INSERTED.STOCK,
            INSERTED.[STATUS],
            INSERTED.DATETIME_CREATE,
            INSERTED.DATETIME_UPDATE,
            INSERTED.USER_CREATE,
            INSERTED.USER_UPDATE
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'DELETE'
    BEGIN
        DELETE FROM [PRODUCT]
        WHERE ID = @p_ID;
    END;
END;
GO