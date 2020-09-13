CREATE TABLE [dbo].[VibrationAnalysisHeaders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [BsDryRunActive] BIT NOT NULL DEFAULT 0, 
    [BsProduction] BIT NOT NULL DEFAULT 0, 
    [AsDryRun] BIT NOT NULL DEFAULT 0, 
    [AsWaterTest] BIT NOT NULL DEFAULT 0, 
    [AsProduction] BIT NOT NULL DEFAULT 0, 
    [ReportHeaderId] INT NOT NULL, 
    [Remarks] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_VibrationAnalysisHeaders_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
