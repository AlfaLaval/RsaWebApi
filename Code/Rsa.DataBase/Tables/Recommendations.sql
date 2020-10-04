CREATE TABLE [dbo].[Recommendations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Remarks] NVARCHAR(100) NULL,
    [ImmediateAction] BIT NOT NULL DEFAULT 0,
    [MidTermAction] BIT NOT NULL DEFAULT 0,
    [Observation] BIT NOT NULL DEFAULT 0,
    [ReportHeaderId] INT NOT NULL, 
    [EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Recommendations_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])

)
