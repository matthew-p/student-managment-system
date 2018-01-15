USE School;
GO

Create PROCEDURE dbo.SelectAllStudents
AS
SET NOCOUNT ON
    BEGIN
        SELECT Id,FirstName,LastName,Gpa FROM Students 
        WHERE DELETED = 0
    END
GO