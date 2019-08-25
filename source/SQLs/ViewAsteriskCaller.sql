USE [InfoCenterDB]
GO

/****** Object:  View [dbo].[ViewAsteriskCaller]    Script Date: 24.08.2019 18:17:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[ViewAsteriskCaller]
AS
SELECT Id, Caller, Date_Time 
FROM
(SELECT ROW_NUMBER() OVER (PARTITION BY convert(date,date_time) ORDER BY date_time DESC) AS ID2,*
FROM AsteriskCaller) t
where t.Date_Time >= convert(date, getdate()-1) and t.ID2 <= 10
GO


