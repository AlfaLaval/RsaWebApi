CREATE TABLE [dbo].[ImageHouses]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[ReportHeaderId] INT NOT NULL, 
	[Entity] NVARCHAR(100) NOT NULL, 
    --[EntityId] INT NOT NULL,
	[EntityRefGuid] UNIQUEIDENTIFIER NOT NULL,
    [ImageFileGuid] UNIQUEIDENTIFIER NOT NULL,
	[ImageLabel] VARCHAR(100) NULL
)
