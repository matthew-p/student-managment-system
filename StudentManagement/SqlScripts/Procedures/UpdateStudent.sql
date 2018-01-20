USE School
GO

Create PROCEDURE dbo.UpdateStudent
(
  @Id BIGINT,
  @FirstName VARCHAR(128),
  @LastName VARCHAR(128),
  @Gpa DECIMAL(8,6)
)
As

SET NOCOUNT ON
BEGIN
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
  BEGIN TRANSACTION
    UPDATE Students 
       SET FirstName = IsNull(@FirstName, FirstName),
           LastName = IsNull(@LastName, LastName),
           Gpa = IsNull(@Gpa, Gpa)
     WHERE Id = @Id
  COMMIT TRANSACTION
END