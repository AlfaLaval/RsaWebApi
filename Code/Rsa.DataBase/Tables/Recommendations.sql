CREATE TABLE [dbo].[Recommendations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Remarks] NVARCHAR(MAX) NULL,
    [ImmediateAction] BIT NOT NULL DEFAULT 0,
    [MidTermAction] BIT NOT NULL DEFAULT 0,
    [Observation] BIT NOT NULL DEFAULT 0,
    [EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    [Status] NVARCHAR NOT NULL DEFAULT 'A',
    [ReportGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
