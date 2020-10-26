CREATE TABLE [dbo].[Miscs]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FirmComments] NVARCHAR(MAX) NULL, 
    [CustomerComments] NVARCHAR(MAX) NULL, 
    [ReportHeaderId] INT NULL, 
    CONSTRAINT [FK_Miscs_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
