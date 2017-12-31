USE [DB_Core2Walkthrough]
GO

/****** Object: Table [dbo].[ACTIVITY_LOG] Script Date: 12/25/2017 9:09:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ACTIVITY_LOG] (
    [ID]           INT      IDENTITY (1, 1) NOT NULL,
    [ID_USER]      INT      NULL,
    [ID_ACTIVITY]  INT      NULL,
    [DATE_ENTERED] DATETIME NULL
);


