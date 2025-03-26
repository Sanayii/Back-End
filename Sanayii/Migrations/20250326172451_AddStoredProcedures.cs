using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanayii.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // ✅===== USER RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserById;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetUserById
                    @UserId VARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Users WHERE Id = @UserId;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveUsers;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllActiveUsers
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Users WHERE IsDeleted = 0;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SoftDeleteUser;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE SoftDeleteUser
                    @UserId UNIQUEIDENTIFIER
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        UPDATE Users SET IsDeleted = 1 WHERE Id = @UserId;
                        SELECT @@ROWCOUNT AS AffectedRows;
                    END TRY
                    BEGIN CATCH
                        SELECT -1 AS AffectedRows, ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS RestoreUser;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE RestoreUser
                    @UserId UNIQUEIDENTIFIER
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        UPDATE Users SET IsDeleted = 0 WHERE Id = @UserId;
                        SELECT @@ROWCOUNT AS AffectedRows;
                    END TRY
                    BEGIN CATCH
                        SELECT -1 AS AffectedRows, ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");




            // ✅===== ADMIN RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateAdmin;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateAdmin
                    @UserId UNIQUEIDENTIFIER,
                    @UserName NVARCHAR(256),
                    @Email NVARCHAR(256),
                    @FName NVARCHAR(50) = NULL,
                    @LName NVARCHAR(50) = NULL,
                    @Age INT = NULL,
                    @City NVARCHAR(50) = NULL,
                    @Street NVARCHAR(50) = NULL,
                    @Governate NVARCHAR(50) = NULL,
                    @Salary DECIMAL(10,2)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        UPDATE Users
                        SET UserName = @UserName,
                            Email = @Email,
                            FName = @FName,
                            LName = @LName,
                            Age = @Age,
                            City = @City,
                            Street = @Street,
                            Governate = @Governate
                        WHERE Id = @UserId;
                        
                        UPDATE Admins
                        SET Salary = @Salary
                        WHERE Id = @UserId;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAdminById;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAdminById
                    @UserId VARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*, A.Salary
                    FROM Users U
                    INNER JOIN Admins A ON U.Id = A.Id
                    WHERE U.Id = @UserId AND U.IsDeleted = 0;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveAdmins;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllActiveAdmins
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*, A.Salary
                    FROM Users U
                    INNER JOIN Admins A ON U.Id = A.Id
                    WHERE U.IsDeleted = 0;
                END;
            ");

            // ✅===== ARTISAN RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateArtisan;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateArtisan
                    @UserId UNIQUEIDENTIFIER,
                    @UserName NVARCHAR(256),
                    @Email NVARCHAR(256),
                    @FName NVARCHAR(50) = NULL,
                    @LName NVARCHAR(50) = NULL,
                    @Age INT = NULL,
                    @City NVARCHAR(50) = NULL,
                    @Street NVARCHAR(50) = NULL,
                    @Governate NVARCHAR(50) = NULL,
                    @NationalityID INT,
                    @Rating FLOAT,
                    @CategoryID INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        UPDATE Users
                        SET UserName = @UserName,
                            Email = @Email,
                            FName = @FName,
                            LName = @LName,
                            Age = @Age,
                            City = @City,
                            Street = @Street,
                            Governate = @Governate
                        WHERE Id = @UserId;
                        
                        UPDATE Artisans
                        SET NationalityID = @NationalityID,
                            Rating = @Rating,
                            CategoryID = @CategoryID
                        WHERE Id = @UserId;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 
                            ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END;
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetArtisanById;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetArtisanById
                    @UserId VARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*, A.NationalityID, A.Rating, A.CategoryID
                    FROM Users U
                    INNER JOIN Artisans A ON U.Id = A.Id
                    WHERE U.Id = @UserId AND U.IsDeleted = 0;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveArtisans;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllActiveArtisans
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*, A.NationalityID, A.Rating, A.CategoryID
                    FROM Users U
                    INNER JOIN Artisans A ON U.Id = A.Id
                    WHERE U.IsDeleted = 0;
                END;
            ");

            // ✅===== CUSTOMER RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateCustomer;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateCustomer
                    @UserId UNIQUEIDENTIFIER,
                    @UserName NVARCHAR(256),
                    @Email NVARCHAR(256),
                    @FName NVARCHAR(50) = NULL,
                    @LName NVARCHAR(50) = NULL,
                    @Age INT = NULL,
                    @City NVARCHAR(50) = NULL,
                    @Street NVARCHAR(50) = NULL,
                    @Governate NVARCHAR(50) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        UPDATE Users
                        SET UserName = @UserName,
                            Email = @Email,
                            FName = @FName,
                            LName = @LName,
                            Age = @Age,
                            City = @City,
                            Street = @Street,
                            Governate = @Governate
                        WHERE Id = @UserId;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 
                            ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetCustomerById;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetCustomerById
                    @UserId VARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*
                    FROM Users U
                    INNER JOIN Customer C ON U.Id = C.Id
                    WHERE U.Id = @UserId AND U.IsDeleted = 0;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetActiveCustomers;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetActiveCustomers
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT U.*
                    FROM Users U
                    INNER JOIN Customer C ON U.Id = C.Id
                    WHERE U.IsDeleted = 0;
                END;
            ");

            //✅ ===== SERVICE RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateService;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetServiceById
                    @ServiceId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Service s WHERE s.Id = @ServiceId;
                END;
            ");

            //✅ ===== REVIEW RELATED PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateReview");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateReview
                    @ReviewDate DATETIME,
                    @Rating INT,
                    @CustomerId VARCHAR(450),
                    @ArtisanId VARCHAR(450),
                    @ServiceId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        INSERT INTO Reviews (ReviewDate, Rating, CustomerId, ArtisanId, ServiceId)
                        VALUES (@ReviewDate, @Rating, @CustomerId, @ArtisanId, @ServiceId);

                        SELECT SCOPE_IDENTITY() AS ReviewId;
                    END TRY
                    BEGIN CATCH
                        THROW;
                    END CATCH
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetReviewById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetReviewById
                    @ReviewId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Reviews WHERE Id = @ReviewId;
                END;
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllReviews");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllReviews
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT R.*, 
                               C.FName + ' ' + C.LName AS CustomerName, 
                               A.FName + ' ' + A.LName AS ArtisanName, 
                               S.ServiceName
                        FROM Reviews R
                        INNER JOIN Users C ON R.CustomerId = C.Id
                        INNER JOIN Users A ON R.ArtisanId = A.Id
                        INNER JOIN Service S ON R.ServiceId = S.Id;
                    END TRY
                    BEGIN CATCH
                        THROW;
                    END CATCH
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateReview");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateReview
                    @ReviewId INT,
                    @ReviewDate DATETIME,
                    @Rating INT,
                    @CustomerId VARCHAR(450),
                    @ArtisanId VARCHAR(450),
                    @ServiceId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        UPDATE Reviews
                        SET ReviewDate = @ReviewDate,
                            Rating = @Rating,
                            CustomerId = @CustomerId,
                            ArtisanId = @ArtisanId,
                            ServiceId = @ServiceId
                        WHERE Id = @ReviewId;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteReview");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteReview
                    @ReviewId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        DELETE FROM Reviews WHERE Id = @ReviewId;
                        SELECT @@ROWCOUNT AS AffectedRows;
                    END TRY
                    BEGIN CATCH
                        SELECT -1 AS AffectedRows, ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");

            //✅ ===== USERPHONES CRUD PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserPhoneById");
            // Create UserPhone
            migrationBuilder.Sql(@"
                 CREATE PROCEDURE CreateUserPhone
                    @UserId VARCHAR(450),
                    @PhoneNumber VARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        INSERT INTO UserPhones (UserId, PhoneNumber)
                        VALUES (@UserId, @PhoneNumber);
                
                        SELECT PhoneNumber FROM UserPhones 
                        WHERE UserId = @UserId AND PhoneNumber = @PhoneNumber;
                    END TRY
                    BEGIN CATCH
                        SELECT 
                            ERROR_NUMBER() AS ErrorNumber,
                            ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");


            // Get all phones for a user
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserPhonesByUserId");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetUserPhonesByUserId
                  @UserId VARCHAR(450)
                 AS
              BEGIN
                SET NOCOUNT ON;
                    SELECT * FROM UserPhones 
                    WHERE UserId = @UserId;
             END;
                            ");
            //Get UserPhone

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserPhoneById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetUserPhoneById
                    @UserId VARCHAR(450),
                    @PhoneNumber VARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM UserPhones 
                    WHERE UserId = @UserId AND PhoneNumber = @PhoneNumber;
                END;
            ");
            // Update UserPhone
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateUserPhone");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateUserPhone
                    @UserId VARCHAR(450),
                    @OldPhoneNumber VARCHAR(20),
                    @NewPhoneNumber VARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION;
                
                        UPDATE UserPhones
                        SET PhoneNumber = @NewPhoneNumber
                        WHERE UserId = @UserId AND PhoneNumber = @OldPhoneNumber;
                
                        COMMIT TRANSACTION;
                
                        SELECT PhoneNumber FROM UserPhones 
                        WHERE UserId = @UserId AND PhoneNumber = @NewPhoneNumber;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;
                
                        SELECT 
                            ERROR_NUMBER() AS ErrorNumber,
                            ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");

            // Delete UserPhone
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteUserPhone");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteUserPhone
                    @UserId VARCHAR(450),
                    @PhoneNumber VARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        DELETE FROM UserPhones 
                        WHERE UserId = @UserId AND PhoneNumber = @PhoneNumber;
                
                        SELECT @@ROWCOUNT AS 'RowsAffected';
                    END TRY
                    BEGIN CATCH
                        SELECT 
                            ERROR_NUMBER() AS ErrorNumber,
                            ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");

            // Delete all phones for a user
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteAllUserPhones");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteAllUserPhones
                    @UserId VARCHAR(450)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        DELETE FROM UserPhones 
                        WHERE UserId = @UserId;
                
                        SELECT @@ROWCOUNT AS 'RowsAffected';
                    END TRY
                    BEGIN CATCH
                        SELECT 
                            ERROR_NUMBER() AS ErrorNumber,
                            ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH
                END;
            ");

            // ✅===== Service CRUD PROCEDURES =====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateService");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateService
                    @Description nvarchar(max),
                    @BasePrice float,
                    @AdditionalPrice float
                AS
                BEGIN
                    INSERT INTO Service (Description, BasePrice, AdditionalPrice)
                    VALUES (@Description, @BasePrice, @AdditionalPrice)
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllServices");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllServices
                    As
                    BEGIN
                        SELECT * FROM Service
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetServiceById");
            migrationBuilder.Sql(@"
                create procedure GetServiceById
                    @Id int
                as
                begin
                    select * from Service where Id = @Id
                end;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateService");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateService
                    @Id int,
                    @Description nvarchar(max),
                    @BasePrice float,
                    @AdditionalPrice float
                AS
                BEGIN
                    UPDATE Service
                    SET Description = @Description, BasePrice = @BasePrice, AdditionalPrice = @AdditionalPrice
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteService");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteService
                    @Id int
                AS
                BEGIN
                    DELETE FROM Service
                    WHERE Id = @Id
                END;
            ");
            //✅=== PAYMENT  CRUD PROCEDURES ====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreatePayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreatePayment
                    @Status nvarchar(max),
                    @Amount int
                AS
                BEGIN
                    INSERT INTO Payment (Status, Amount)
                    VALUES (@Status, @Amount)
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllPayments");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllPayments
                    As
                    BEGIN
                        SELECT * FROM Payment
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPaymentById");
            migrationBuilder.Sql(@"
                create procedure GetPaymentById
                    @Id int
                    as
                    begin
                        select * from Payment where Id = @Id
                    end;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdatePayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdatePayment
                    @Id int,
                    @Status nvarchar(max),
                    @Amount int
                AS
                BEGIN
                    UPDATE Payment
                    SET Status = @Status, Amount = @Amount
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeletePayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeletePayment
                    @Id int
                AS
                BEGIN
                    DELETE FROM Payment
                    WHERE Id = @Id
                END;
            ");

            // ✅==PAYMENT METHOD CRUD PROCEDURES==
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreatePaymentMethod");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreatePaymentMethod
                    @PaymentID INT,
                    @Method nvarchar(450)
                AS
                BEGIN
                    INSERT INTO PaymentMethods (PaymentId, Method) 
                    VALUES (@PaymentID, @Method);
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllPaymentMethods");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllPaymentMethods
                AS
                BEGIN
                    SELECT * FROM PaymentMethods;
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPaymentMethodById");
            migrationBuilder.Sql(@"
                create procedure GetPaymentMethodById
                    @PaymentID int
                    as
                    begin
                        select * from PaymentMethods where PaymentId = @PaymentID
                    end;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdatePaymentMethod");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdatePaymentMethod
                    @PaymentID INT,
                    @Method nvarchar(450)
                AS
                BEGIN
                    UPDATE PaymentMethods
                    SET Method = @Method
                    WHERE PaymentId = @PaymentID
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeletePaymentMethod");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeletePaymentMethod
                    @PaymentID INT
                AS
                BEGIN
                    DELETE FROM PaymentMethods
                    WHERE PaymentId = @PaymentID
                END;
            ");

            //ServiceRequestPayment crud
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateServiceRequestPayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateServiceRequestPayment
                    @CustomerId nvarchar(450),
                    @PaymentId int,
                    @ServiceId int,
                    @CreatedAt datetime
                    @ExecutionTime int
                AS
                BEGIN
                    INSERT INTO ServiceRequestPayment (CustomerId, PaymentId, ServiceId, CreatedAt, ExecutionTime)
                    VALUES (@CustomerId, @PaymentId, @ServiceId, @CreatedAt, @ExecutionTime)
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllServiceRequestPayments");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllServiceRequestPayments
                    As
                    BEGIN
                        SELECT * FROM ServiceRequestPayment
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetServiceRequestPaymentsById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetServiceRequestPaymentsById
                    @CustomerId nvarchar(450),
                    @PaymentId int
                    @CreatedAt datetime
                    As
                    BEGIN
                        SELECT * FROM ServiceRequestPayment WHERE CustomerId = @CustomerId AND PaymentId = @PaymentId AND CreatedAt=@CreatedAt
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateServiceRequestPayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateServiceRequestPayment
                    @CustomerId nvarchar(450),
                    @PaymentId int,
                    @ServiceId int,
                    @CreatedAt datetime
                    @ExecutionTime int
                AS
                BEGIN
                    UPDATE ServiceRequestPayment
                    SET CreatedAt = @CreatedAt, ExecutionTime = @ExecutionTime
                    WHERE CustomerId = @CustomerId AND PaymentId = @PaymentId AND ServiceId = @ServiceId
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteServiceRequestPayment");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteServiceRequestPayment
                    @CustomerId nvarchar(450),
                    @PaymentId int,
                    @ServiceId int
                AS
                BEGIN
                    DELETE FROM ServiceRequestPayment
                    WHERE CustomerId = @CustomerId AND PaymentId = @PaymentId AND ServiceId = @ServiceId
                END;
            ");

            //✅==Categories crud==
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateCategory");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateCategory
                    @Name nvarchar(max)
                AS
                BEGIN
                    INSERT INTO Categories (Name)
                    VALUES (@Name)
                END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS GetAllCategories");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllCategories
                    As
                    BEGIN
                        SELECT * FROM Categories
                    END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS GetCategoryById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetCategoryById
                    @Id int
                    As
                    BEGIN
                        SELECT * FROM Categories WHERE Id = @Id
                    END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS UpdateCategory");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateCategory
                    @Id int,
                    @Name nvarchar(max)
                AS
                BEGIN
                    UPDATE Categories
                    SET Name = @Name
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS DeleteCategory");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteCategory
                    @Id int
                AS
                BEGIN
                    DELETE FROM Categories
                    WHERE Id = @Id
                END;
            ");

            //✅==Contract crud==
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateContract");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateContract
                    @StartDate datetime,
                    @EndDate datetime,
                    @MaxViolationsAllowed int,
                    @Status nvarchar(max),
                    @ArtisanId nvarchar(450)
                AS
                BEGIN
                    INSERT INTO Contract (StartDate, EndDate, MaxViolationsAllowed, Status, ArtisanId)
                    VALUES (@StartDate, @EndDate, @MaxViolationsAllowed, @Status, @ArtisanId)
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllContracts");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllContracts
                    As
                    BEGIN
                        SELECT * FROM Contract
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetContractById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetContractById
                    @Id int
                    As
                    BEGIN
                        SELECT * FROM Contract WHERE Id = @Id
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateContract");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateContract
                    @Id int,
                    @StartDate datetime,
                    @EndDate datetime,
                    @MaxViolationsAllowed int,
                    @Status nvarchar(max),
                    @ArtisanId nvarchar(450)
                AS
                BEGIN
                    UPDATE Contract
                    SET StartDate = @StartDate, EndDate = @EndDate, MaxViolationsAllowed=@MaxViolationsAllowed, Status=@Status, ArtisanId=@ArtisanId
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS DeleteContract");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteContract
                    @Id int
                AS
                BEGIN
                    DELETE FROM Contract
                    WHERE Id = @Id
                END;
            ");

            //✅==DISCOUNT CRUD PROCEDURE==
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateDiscount");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateDiscount
                    @Name nvarchar(max),
                    @Amount int,
                    @MinRequiredRequests int,
                    @IsFixedAmount bit,
                    @IsPercentage bit,
                    @ExpireDate datetime,
                    @CustomerId nvarchar(450)
                AS
                BEGIN
                    INSERT INTO Discount (Name, Amount, MinRequiredRequests, IsFixedAmount, IsPercentage, ExpireDate, CustomerId)
                    VALUES (@Name, @Amount, @MinRequiredRequests, @IsFixedAmount, @IsPercentage, @ExpireDate, @CustomerId)
                END;
            ");
            migrationBuilder.Sql("Drop PROCEDURE IF EXISTS GetAllDiscounts");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllDiscounts
                    As
                    BEGIN
                        SELECT * FROM Discount
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetDiscountById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetDiscountById
                    @Id int
                    As
                    BEGIN
                        SELECT * FROM Discount WHERE Id = @Id
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateDiscount");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateDiscount
                    @Id int,
                    @Name nvarchar(max),
                    @Amount int,
                    @MinRequiredRequests int,
                    @IsFixedAmount bit,
                    @IsPercentage bit,
                    @ExpireDate datetime,
                    @CustomerId nvarchar(450)
                AS
                BEGIN
                    UPDATE Discount
                    SET Name = @Name, Amount = @Amount, MinRequiredRequests=@MinRequiredRequests, IsFixedAmount=@IsFixedAmount, IsPercentage=@IsPercentage, ExpireDate=@ExpireDate, CustomerId=@CustomerId
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteDiscount");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteDiscount
                    @Id int
                AS
                BEGIN
                    DELETE FROM Discount
                    WHERE Id = @Id
                END;
            ");

            //✅===Violation crud====
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateViolation");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateViolation
                    @Reason nvarchar(max),
                    @Status nvarchar(max),
                    @Date datetime,
                    @ContractId int
                AS
                BEGIN
                    INSERT INTO Violation (Reason, Status, Date, ContractId)
                    VALUES (@Reason, @Status, @Date, @ContractId)
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllViolations");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAllViolations
                    As
                    BEGIN
                        SELECT * FROM Violation
                    END;
            ");
            migrationBuilder.Sql("DRO PROCEDURE IF EXISTS GetViolationById");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetViolationById
                    @Id int
                    As
                    BEGIN
                        SELECT * FROM Violation WHERE Id = @Id
                    END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateViolation");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateViolation
                    @Id int,
                    @Reason nvarchar(max),
                    @Status nvarchar(max),
                    @Date datetime,
                    @ContractId int
                AS
                BEGIN
                    UPDATE Violation
                    SET Reason = @Reason, Status = @Status, Date=@Date, ContractId=@ContractId
                    WHERE Id = @Id
                END;
            ");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteViolation");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteViolation
                    @Id int
                AS
                BEGIN
                    DELETE FROM Violation
                    WHERE Id = @Id
                END;
            ");
        }

        

        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop procedures in reverse order

           

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetActiveCustomers");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetCustomerById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateCustomer");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveArtisans");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetArtisanById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateArtisan");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveAdmins");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAdminById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateAdmin");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS RestoreUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SoftDeleteUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllActiveUsers");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserById");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteReview");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateReview");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllReviews");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetReviewById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateReview");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteUserPhone");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteAllUserPhones");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateUserPhone");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateUserPhone");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserPhonesByUserId");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetUserPhoneById");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateService");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllServices");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetServiceById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateService");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteService");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreatePayment");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllPayments");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPaymentById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdatePayment");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeletePayment");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreatePaymentMethod");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllPaymentMethods");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPaymentMethodById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdatePaymentMethod");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeletePaymentMethod");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateServiceRequestPayment");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllServiceRequestPayments");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetServiceRequestPaymentsById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateServiceRequestPayment");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteServiceRequestPayment");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateCategory");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllCategories");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetCategoryById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateCategory");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteCategory");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateContract");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllContracts");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetContractById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateContract");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteContract");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateDiscount");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllDiscounts");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetDiscountById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateDiscount");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteDiscount");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateViolation");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetAllViolations");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateViolation");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteViolation");

        }
    }
}
