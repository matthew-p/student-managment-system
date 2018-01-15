USE School
GO

CREATE PROCEDURE dbo.RemoveStudentRecord
(
    @Id BIGINT
)
As
SET NOCOUNT ON
    Begin
        DELETE from Students where Id=@Id
    End
GO