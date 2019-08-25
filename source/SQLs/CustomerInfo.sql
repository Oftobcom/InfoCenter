USE [InfoCenterDB]
GO

/****** Object:  Table [dbo].[CustomerInfo]    Script Date: 25.08.2019 19:36:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CustomerInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](30) NULL,
	[CustomerAddres] [nvarchar](30) NULL,
	[PhoneNumber2] [nvarchar](30) NULL,
	[CreditCount] [int] NOT NULL,
	[CreditCountActive] [int] NOT NULL,
	[DepositCount] [int] NOT NULL,
	[DepositCountActive] [int] NOT NULL,
 CONSTRAINT [PK_CustomerInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


