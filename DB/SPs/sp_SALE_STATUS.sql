CREATE OR ALTER PROCEDURE sp_SALE_STATUS
(
    @p_ACTION VARCHAR(16),

    @p_ID INT = NULL,
    @p_NAME VARCHAR(32) = NULL,
    @p_DESCRIPTION VARCHAR(256) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @p_ACTION = 'GET'
    BEGIN
        SELECT
            ID,
            [NAME],
            [DESCRIPTION]
        FROM SALE_STATUS
    END;


    IF @p_ACTION = 'GET_BY_ID'
    BEGIN
        SELECT
            ID,
            [NAME],
            [DESCRIPTION]
        FROM SALE_STATUS
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'SEARCH'
    BEGIN
        SELECT
            ID,
            [NAME],
            [DESCRIPTION]
        FROM SALE_STATUS
        WHERE
            [NAME] LIKE '%' + @p_NAME + '%'
            OR
            [DESCRIPTION] LIKE '%' + @p_NAME + '%';
    END;


    IF @p_ACTION = 'INSERT'
    BEGIN
        INSERT INTO SALE_STATUS
        (
            ID,
            [NAME],
            [DESCRIPTION]
        )
        OUTPUT
            INSERTED.ID,
            INSERTED.[NAME],
            INSERTED.[DESCRIPTION]
        VALUES
        (
            @p_ID,
            @p_NAME,
            @p_DESCRIPTION
        );
    END;


    IF @p_ACTION = 'UPDATE'
    BEGIN
        UPDATE SALE_STATUS
        SET
            [NAME] = @p_NAME,
            [DESCRIPTION] = @p_DESCRIPTION
        OUTPUT
            INSERTED.ID,
            INSERTED.[NAME],
            INSERTED.[DESCRIPTION]
        WHERE ID = @p_ID;
    END;


    IF @p_ACTION = 'DELETE'
    BEGIN
        DELETE FROM SALE_STATUS
        WHERE ID = @p_ID;
    END;
END;
GO