CREATE TABLE [dbo].[UserPermissions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserGuid] NVARCHAR(50) NULL, 
    [InstitutionGuid] NVARCHAR(50) NULL, 
    [Roles] VARCHAR(50) NULL, 
    [Permissions] NVARCHAR(50) NULL
)
