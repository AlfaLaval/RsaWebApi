CREATE TABLE [dbo].[Miscs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FirmComments] NVARCHAR(MAX) NULL, 
    [CustomerComments] NVARCHAR(MAX) NULL, 
    [ReportHeaderId] INT NULL, 
    [FirmName] NVARCHAR(MAX) NULL, 
    [CustomerName] NVARCHAR(MAX) NULL, 
    [FirmDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CustomerDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [FK_Miscs_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
