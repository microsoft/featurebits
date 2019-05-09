CREATE TABLE [dbo].[FeatureBitHeirarchy]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[ParentId] INT NOT NULL,
	[BitId] INT NOT NULL,
	[CreatedByUser] NVARCHAR(100) NOT NULL,
	[CreatedDateTime] DATETIME2 NOT NULL,
	[LastModifiedByUser] NVARCHAR(100) NOT NULL,
	[LastModifiedDateTime] DATETIME2 NOT NULL
    CONSTRAINT [PK_FeatureBitHeirarchy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FeatureBitHeirarchy_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[FeatureBitDefinitions] ([Id]),
    CONSTRAINT [FK_FeatureBitHeirarchy_BitId] FOREIGN KEY ([BitId]) REFERENCES [dbo].[FeatureBitDefinitions] ([Id]),
)
