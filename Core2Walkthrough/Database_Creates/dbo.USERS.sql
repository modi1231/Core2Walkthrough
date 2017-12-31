USE [DB_Core2Walkthrough]
GO

/****** Object: Table [dbo].[USERS] Script Date: 12/25/2017 9:09:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[USERS] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (10)  NULL,
    [Password]     VARCHAR (50)  NULL,
    [XP]           INT           NULL,
    [DATE_ENTERED] DATETIME      NOT NULL,
    [IS_ADMIN]     BIT           NOT NULL,
    [DESCRIPTION]  VARCHAR (100) NOT NULL
);


