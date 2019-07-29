USE [InfoCenterDB]
GO

/****** Object:  Table [dbo].[ClientInfo]    Script Date: 21.07.2019 7:06:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Surname] [nvarchar](30) NULL,
	[Firstname] [nvarchar](30) NULL,
	[Patronymic] [nvarchar](30) NULL,
	[Phone] [nvarchar](30) NULL,
	[Address] [nvarchar](255) NULL,
	[NumberActiveCredits] [int] NOT NULL,
	[NumberActiveDeposits] [int] NOT NULL,
	[NumberTotalCredits] [int] NOT NULL,
	[NumberTotalDeposits] [int] NOT NULL,
	[NumberRemittances] [int] NOT NULL,
 CONSTRAINT [PK_ClientInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



INSERT INTO [dbo].[ClientInfo] ([Surname], [Phone], [Address], [Firstname], [NumberActiveCredits], [NumberActiveDeposits], [NumberRemittances], [NumberTotalCredits], [NumberTotalDeposits], [Patronymic]) 
VALUES (N'Хакимов', N'992927052100', N'г. Худжанд', N'Рахматджон', 0, 1, 3, 0, 1, N'Иномович');

INSERT INTO [dbo].[ClientInfo] ([Surname], [Phone], [Address], [Firstname], [NumberActiveCredits], [NumberActiveDeposits], [NumberRemittances], [NumberTotalCredits], [NumberTotalDeposits], [Patronymic]) 
VALUES (N'Хакимова', N'992927041881', N'г. Худжанд; 19-3-29', N'Мунаввар', 0, 1, 5, 0, 1, N'Тохировна');

INSERT INTO [dbo].[ClientInfo] ([Surname], [Phone], [Address], [Firstname], [NumberActiveCredits], [NumberActiveDeposits], [NumberRemittances], [NumberTotalCredits], [NumberTotalDeposits], [Patronymic]) 
VALUES (N'Восидов', N'992927771234', N'г. Худжанд', N'Анвар', 0, 1, 3, 0, 1, N'Акбарович');

INSERT INTO [dbo].[ClientInfo] ([Surname], [Phone], [Address], [Firstname], [NumberActiveCredits], [NumberActiveDeposits], [NumberRemittances], [NumberTotalCredits], [NumberTotalDeposits], [Patronymic]) 
VALUES (N'Алиева', N'992925552100', N'г. Душанбе', N'Бунафша', 3, 3, 5, 3, 3, N'Темуровна');

