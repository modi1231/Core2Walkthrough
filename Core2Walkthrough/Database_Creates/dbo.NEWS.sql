USE [DB_Core2Walkthrough]
GO

/****** Object: Table [dbo].[NEWS] Script Date: 12/25/2017 9:09:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NEWS] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [TEXT]         VARCHAR (100) NOT NULL,
    [DATE_ENTERED] DATETIME      NOT NULL
);


