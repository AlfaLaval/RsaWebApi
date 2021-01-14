CREATE TABLE [dbo].[CommonMasters]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [DisplayText] NVARCHAR(MAX) NOT NULL, 
    [DisplayValue] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [Active] BIT NOT NULL DEFAULT 0
)
