CREATE TABLE [dbo].[Recommendations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Remarks] NVARCHAR(100) NULL, 
    [ReportHeaderId] INT NOT NULL, 
    CONSTRAINT [FK_Recommendations_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])

)
