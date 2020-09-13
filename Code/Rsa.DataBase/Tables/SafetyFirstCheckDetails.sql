CREATE TABLE [dbo].[SafetyFirstCheckDetails]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [SafetyFirstCheckId] INT NOT NULL, 
    [CheckPointName] NVARCHAR(50) NOT NULL, 
    [IsSelected] BIT NOT NULL DEFAULT 0, 
    [Remarks] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_SafetyCheckDetails_SafetyFirstChecks] FOREIGN KEY ([SafetyFirstCheckId]) REFERENCES [SafetyFirstChecks]([Id])
)
