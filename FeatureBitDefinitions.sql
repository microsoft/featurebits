CREATE TABLE [dbo].[FeatureBitDefinitions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllowedUsers] [nvarchar](2048) NULL,
	[CreatedByUser] [nvarchar](100) NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[ExcludedEnvironments] [nvarchar](300) NULL,
	[LastModifiedByUser] [nvarchar](100) NOT NULL,
	[LastModifiedDateTime] [datetime2](7) NOT NULL,
	[MinimumAllowedPermissionLevel] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[OnOff] [bit] NOT NULL,
	[ExactAllowedPermissionLevel] [int] NULL,
	[Dependencies] [varchar](255) NULL,
 CONSTRAINT [PK_FeatureBitDefinitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FeatureBit_Name]    Script Date: 5/16/2019 4:19:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_FeatureBit_Name] ON [dbo].[FeatureBitDefinitions]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
