CREATE TABLE [dbo].[StudentAssociation]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StudentGuid] NVARCHAR(50) NULL, 
    [InstitutionGuid] NVARCHAR(50) NULL, 
    [Type] NVARCHAR(50) NULL
)
