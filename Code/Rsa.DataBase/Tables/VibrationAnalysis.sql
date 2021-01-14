CREATE TABLE [dbo].[VibrationAnalysis]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Parameter] NVARCHAR(200) NOT NULL, 
    [Units] NVARCHAR(20) NOT NULL, 
    [BsDryRun] NVARCHAR(50) NULL, 
    [BsProduction] NVARCHAR(50) NULL, 
    [AsDryRun] NVARCHAR(50) NULL, 
    [AsWaterTest] NVARCHAR(50) NULL, 
    [AsProduction] NVARCHAR(50) NULL, 
    [VibrationAnalysisHeaderId] INT NOT NULL,
    [ReportGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CONSTRAINT [FK_VibrationAnalysis_VibrationAnalysisHeaders] FOREIGN KEY ([VibrationAnalysisHeaderId]) REFERENCES [VibrationAnalysisHeaders]([Id])
)
