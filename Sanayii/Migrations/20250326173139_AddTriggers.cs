using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanayii.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //✅==Add trigger to save tables Actions details===
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_AuditLogs");
            migrationBuilder.Sql(@"
                DECLARE @TableName NVARCHAR(100);
                DECLARE TableCursor CURSOR FOR
                SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME != 'AuditLogs';

                OPEN TableCursor;
                FETCH NEXT FROM TableCursor INTO @TableName;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DECLARE @SQL NVARCHAR(MAX) = '
                    CREATE TRIGGER trg_Audit_' + @TableName + '
                    ON ' + @TableName + '
                    AFTER INSERT, UPDATE, DELETE
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        BEGIN TRY
                            DECLARE @UserId NVARCHAR(450) = SYSTEM_USER;
                            DECLARE @Type NVARCHAR(50);
                            DECLARE @TableName NVARCHAR(100) = ''' + @TableName + ''';
                            DECLARE @Timestamp DATETIME2 = GETDATE();
                            DECLARE @OldValues NVARCHAR(MAX) = NULL;
                            DECLARE @NewValues NVARCHAR(MAX) = NULL;
                            DECLARE @AffectedColumns NVARCHAR(MAX) = NULL;
                            DECLARE @PrimaryKey NVARCHAR(MAX) = NULL;

                           
                            IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
                                SET @Type = ''UPDATE'';
                            ELSE IF EXISTS (SELECT * FROM inserted)
                                SET @Type = ''INSERT'';
                            ELSE
                                SET @Type = ''DELETE'';

                            
                            SELECT @PrimaryKey = STRING_AGG(c.name, '', '')
                            FROM sys.indexes i
                            JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                            JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                            WHERE i.is_primary_key = 1 AND OBJECT_NAME(i.object_id) = @TableName;

                            IF @Type = ''UPDATE'' OR @Type = ''DELETE''
                                SELECT @OldValues = (SELECT * FROM deleted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

                            IF @Type = ''UPDATE'' OR @Type = ''INSERT''
                                SELECT @NewValues = (SELECT * FROM inserted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

                            
                            IF @Type = ''UPDATE''
                                SELECT @AffectedColumns = (
                                    SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_NAME = ''' + @TableName + ''' FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                                );

                           
                            INSERT INTO AuditLogs (UserId, Type, TableName, Timestamp, OldValues, NewValues, AffectedColumns, PrimaryKey)
                            VALUES (@UserId, @Type, @TableName, @Timestamp, @OldValues, @NewValues, @AffectedColumns, @PrimaryKey);

                        END TRY
                        BEGIN CATCH
                            
                        END CATCH
                    END;';

                    EXEC sp_executesql @SQL;
                    FETCH NEXT FROM TableCursor INTO @TableName;
                END;

                CLOSE TableCursor;
                DEALLOCATE TableCursor;
            ");
            //✅ ===== TRIGGER TO CHECK VIOLATIONS =====
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_CheckViolations;");
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_CheckViolations;
                CREATE TRIGGER trg_CheckViolations
                ON Violations
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @ContractId INT;
                    DECLARE @MaxViolations INT;
                    DECLARE @AdminId NVARCHAR(450);
                    DECLARE @Message NVARCHAR(MAX);

                    SELECT TOP 1 @AdminId = AdminId FROM Admins ORDER BY AdminId;
                    SELECT @ContractId = ContractId FROM inserted;
                    SELECT @MaxViolations = MaxViolationsAllowed FROM Contracts WHERE Id = @ContractId;

                    IF (SELECT COUNT(*) FROM Violations WHERE ContractId = @ContractId) > @MaxViolations
                    BEGIN
                        UPDATE Contracts SET Status = 'Terminated' WHERE Id = @ContractId;
                        SET @Message = 'Contract ' + CAST(@ContractId AS NVARCHAR) + ' terminated due to excessive violations.';
                        INSERT INTO AdminNotifications (AdminId, Message, Timestamp) VALUES (@AdminId, @Message, GETDATE());
                        PRINT 'Contract Terminated Due to Excessive Violations';
                    END
                END;
            ");
            //✅ ===== TRIGGER TO UPDATE DISCOUNTS =====
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_UpdateDiscount;");
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_UpdateDiscount;
                CREATE TRIGGER trg_UpdateDiscount
                ON ServiceRequestPayments
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @CustomerId NVARCHAR(450);
                    SELECT @CustomerId = i.CustomerId FROM inserted i;

                    DECLARE @OrderCount INT;
                    SELECT @OrderCount = COUNT(*) FROM ServiceRequestPayments WHERE CustomerId = @CustomerId;

                    DECLARE @MinRequiredRequests INT;
                    SELECT @MinRequiredRequests = d.MinRequiredRequests FROM Discounts d WHERE d.CustomerId = @CustomerId;

                    IF @OrderCount >= @MinRequiredRequests
                    BEGIN
                        UPDATE Discounts
                        SET Amount = 
                            CASE 
                                WHEN IsFixedAmount = 1 THEN Amount
                                WHEN IsPercentage = 1 THEN (Amount * @OrderCount) / 100
                                ELSE Amount
                            END
                        WHERE CustomerId = @CustomerId;
                    END
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_CheckViolations");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_UpdateDiscount");
        }
    }
}
