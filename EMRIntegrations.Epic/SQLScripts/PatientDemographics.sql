USE [EMRStagingDB]
GO

/****** Object:  Table [Epic].[PatientDemographics]    Script Date: 02-Jan-20 12:46:08 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Epic].[PatientDemographics](
	[Title] [varchar](500) NULL,
	[Suffix] [varchar](500) NULL,
	[FirstName] [varchar](500) NULL,
	[MiddleName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[Email] [varchar](500) NULL,
	[DateOfBirth] [varchar](500) NULL,
	[Gender] [varchar](500) NULL,
	[Phone] [varchar](500) NULL,
	[EthnicityCode] [varchar](500) NULL,
	[RaceCode] [varchar](500) NULL,
	[emgphone] [varchar](500) NULL,
	[Address1] [varchar](500) NULL,
	[Address2] [varchar](500) NULL,
	[City] [varchar](500) NULL,
	[State] [varchar](500) NULL,
	[ZipCode] [varchar](500) NULL,
	[SSN] [varchar](500) NULL,
	[requestid] [varchar](500) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


