CREATE TABLE [dbo].[ReportHeaders]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [CreatedBy] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [UpdatedBy] INT NULL, 
    [UpdatedOn] DATETIME NULL, 
    [IsSafetyFirstComplete] BIT NOT NULL DEFAULT 0, 
    [IsCustomerEquipmentComplete] BIT NOT NULL DEFAULT 0, 
    [IsVibrationAnalysisComplete] BIT NOT NULL DEFAULT 0, 
    [IsObservationComplete] BIT NOT NULL DEFAULT 0, 
    [IsRecommendationComplete] BIT NOT NULL DEFAULT 0, 
    [ApprovedBy] INT NULL , 
    [IsDocTrigger] BIT NOT NULL DEFAULT 0, 
    [DocTriggerFrom] NVARCHAR(20) NOT NULL DEFAULT 'FINAL', 
    CONSTRAINT [FK_ReportHeaders_Users_C] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_ReportHeaders_Users_U] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_ReportHeaders_Users_A] FOREIGN KEY ([ApprovedBy]) REFERENCES [Users]([Id])
)