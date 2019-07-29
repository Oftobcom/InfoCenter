USE [InfoCenterDB]
GO

/****** Object:  Table [dbo].[AsteriskCaller]    Script Date: 21.07.2019 7:06:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AsteriskCaller](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Caller] [nvarchar](20) NOT NULL,
	[Date_Time] [datetime] NULL,
 CONSTRAINT [PK_AsteriskCaller] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AsteriskCaller] ADD  DEFAULT (getdate()) FOR [Date_Time]
GO


