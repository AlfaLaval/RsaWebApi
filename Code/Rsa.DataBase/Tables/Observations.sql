﻿CREATE TABLE [dbo].[Observations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Title] NVARCHAR(100) NOT NULL, 
    [Remarks] NVARCHAR(100) NULL, 
    [ActionTaken] NVARCHAR(100) NULL, 
    [ReportHeaderId] INT NOT NULL, 
    [EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Observations_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
