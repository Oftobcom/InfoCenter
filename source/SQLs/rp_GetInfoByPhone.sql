USE [InfoCenterDB]
GO

/****** Object:  StoredProcedure [dbo].[rp_GetInfoByPhone]    Script Date: 25.08.2019 19:37:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[rp_GetInfoByPhone]
	@Phone nvarchar(30)
AS
BEGIN
	SET NOCOUNT ON

	select *
	from dbo.CustomerInfo
	where PhoneNumber2 like '%' + @Phone + '%'
END
GO


