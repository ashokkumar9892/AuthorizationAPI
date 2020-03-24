CREATE TABLE [dbo].[Intitutions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ParentGuid] NVARCHAR(50) NULL, 
    [ObjectType] NVARCHAR(50) NULL, 
    [Website] NVARCHAR(50) NULL, 
    [Level] INT NULL
)
