CREATE TABLE [dbo].[Students]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Guid] NVARCHAR(50) NULL, 
    [Name] NVARCHAR(50) NULL, 
    [EnrollmentDate] DATETIME NULL, 
    [DateOfBirth] DATETIME NULL, 
    [Address] NVARCHAR(50) NULL, 
    [ApplicationUserID] INT NULL
)
