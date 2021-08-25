﻿CREATE TABLE [dbo].[Observations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Title] NVARCHAR(100) NOT NULL, 
    [Remarks] NVARCHAR(MAX) NULL, 
    [ActionTaken] NVARCHAR(MAX) NULL, 
    [EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    [Status] NVARCHAR NOT NULL DEFAULT 'A',
    [ReportGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    [CreatedDateTime] DATETIME NOT NULL DEFAULT GETDATE()
)
