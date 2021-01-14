﻿CREATE TABLE [dbo].[SpareParts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Description] NVARCHAR(100) NOT NULL,
    [PartNo] NVARCHAR(100) NULL,
    [Quantity] INT NOT NULL DEFAULT 0,
    [Type] NVARCHAR(5) NOT NULL,
    [Observation] BIT NOT NULL DEFAULT 0,
    [ReportHeaderId] INT NOT NULL, 
    [EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    [Status] CHAR(1) NOT NULL DEFAULT 'A',
    [ReportGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CONSTRAINT [FK_SpareParts_ReportHeaders] FOREIGN KEY ([ReportHeaderId]) REFERENCES [ReportHeaders]([Id])
)
