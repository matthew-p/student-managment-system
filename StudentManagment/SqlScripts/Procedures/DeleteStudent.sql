USE School
GO

CREATE PROCEDURE dbo.DeleteStudentRecord
(
    @Id BIGINT
)
As
    SET NOCOUNT ON
    BEGIN  
    UPDATE Students
        SET Deleted = 1
    WHERE Id = @Id
    END
GO