USE [DB_Core2Walkthrough]
GO

/****** Object: Table [dbo].[ACTIVITY] Script Date: 12/25/2017 9:09:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ACTIVITY] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [TITLE]            VARCHAR (50)  NULL,
    [DESCRIPTION]      VARCHAR (100) NULL,
    [XP]               INT           NULL,
    [COOL_OFF_MINUTES] INT           NULL,
    [IS_ACTIVE]        BIT           NULL,
    [DATE_ENTERED]     DATETIME      NULL
);


