CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [DisplayName] NVARCHAR(100) NOT NULL, 
    [UserName] NVARCHAR(100) NOT NULL UNIQUE, 
    [Email] NVARCHAR(100) NULL, 
    [Active] BIT NOT NULL DEFAULT 0, 
    [Role] NVARCHAR(20) NOT NULL DEFAULT('Editor') 
)
