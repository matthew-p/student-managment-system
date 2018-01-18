USE school
GO

CREATE PROCEDURE dbo.InsertStudent
(
    @FirstName VARCHAR(128),
    @LastName VARCHAR(128),
    @Gpa DECIMAL(8,6)
)
AS
SET NOCOUNT ON
    BEGIN
        INSERT INTO Students(FirstName,LastName,Gpa)
        OUTPUT Inserted.ID
        VALUES(@FirstName,@LastName,@Gpa)
    END
GO