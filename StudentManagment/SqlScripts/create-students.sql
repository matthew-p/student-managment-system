USE school
GO

CREATE TABLE dbo.Students
    (
        Id BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        FirstName VARCHAR(128),
        LastName VARCHAR(128),
        Gpa DECIMAL(8,6)
    )
GO