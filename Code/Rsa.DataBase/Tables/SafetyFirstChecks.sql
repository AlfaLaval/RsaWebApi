CREATE TABLE [dbo].[SafetyFirstChecks]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [ReportHeaderId] INT NOT NULL, 
    [EngineerName] NVARCHAR(200) NOT NULL, 
    [ProjectName] NVARCHAR(200) NULL, 
    [SiteSafetyContact] NVARCHAR(200) NULL, 
    [StartDate] DATETIME NOT NULL, 
    [JobOrderNumber] NVARCHAR(50) NOT NULL, 
    [ContactNUmber] NVARCHAR(50) NULL,
    [ReportGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CONSTRAINT [FK_SafetyFirstChecks_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)

	