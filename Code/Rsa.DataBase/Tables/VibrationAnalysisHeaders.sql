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
    [MdMotor] BIT NOT NULL DEFAULT 0, 
    [BdMotor] BIT NOT NULL DEFAULT 0, 
    MdDriveEndMain NVARCHAR(MAX) NULL,
    MdDriveEndBack NVARCHAR(MAX) NULL,
    MdNonDriveEndMain NVARCHAR(MAX) NULL,
    MdNonDriveEndBack NVARCHAR(MAX) NULL,
    BdDriveEndMain NVARCHAR(MAX) NULL,
    BdDriveEndBack NVARCHAR(MAX) NULL,
    BdNonDriveEndMain NVARCHAR(MAX) NULL,
    BdNonDriveEndBack NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_VibrationAnalysisHeaders_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
