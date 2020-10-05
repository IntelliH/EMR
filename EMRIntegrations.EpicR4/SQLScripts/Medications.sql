USE [EMRStagingDB]
GO

/****** Object:  Table [Epic].[Medications]    Script Date: 02-Jan-20 12:47:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Epic].[Medications](
	[RequestID] [varchar](50) NULL,
	[Source] [varchar](5000) NULL,
	[Createdby] [varchar](5000) NULL,
	[Isstructuredsig] [varchar](5000) NULL,
	[Medicationid] [varchar](5000) NULL,
	[Issafetorenew] [varchar](5000) NULL,
	[Medicationentryid] [varchar](5000) NULL,
	[Structuredsig] [varchar](5000) NULL,
	[Events] [varchar](5000) NULL,
	[Medication] [varchar](5000) NULL,
	[Direction] [varchar](5000) NULL,
	[Dosagefrequencyvalue] [varchar](5000) NULL,
	[doseroute] [varchar](5000) NULL,
	[doseaction] [varchar](5000) NULL,
	[Dosageadditionalinstructions] [varchar](5000) NULL,
	[Dosagefrequencyunit] [varchar](5000) NULL,
	[DoseUnit] [varchar](5000) NULL,
	[Dosequantity] [varchar](5000) NULL,
	[Dosagefrequencydescription] [varchar](5000) NULL,
	[Dosagedurationunit] [varchar](5000) NULL,
	[Stopdate] [varchar](5000) NULL,
	[Startdate] [varchar](5000) NULL,
	[Enterdate] [varchar](5000) NULL,
	[Hidedate] [varchar](5000) NULL,
	[Type] [varchar](5000) NULL,
	[Providernote] [varchar](5000) NULL,
	[Patientnote] [varchar](5000) NULL,
	[Orderingmode] [varchar](5000) NULL,
	[Approvedby] [varchar](5000) NULL,
	[Route] [varchar](5000) NULL,
	[Encounterid] [varchar](5000) NULL,
	[Ndcoptions] [varchar](5000) NULL,
	[Rxnorm] [varchar](5000) NULL,
	[Ndcoptionsstr] [varchar](5000) NULL,
	[Rxnormstr] [varchar](5000) NULL,
	[Stopreason] [varchar](5000) NULL,
	[patientid] [varchar](5000) NULL,
	[daterecorded] [varchar](5000) NULL,
	[Active] [varchar](5000) NULL,
	[ndccode] [varchar](5000) NULL,
	[rxflag] [varchar](5000) NULL,
	[drugcode] [varchar](5000) NULL,
	[CodeSystemId] [varchar](5000) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


