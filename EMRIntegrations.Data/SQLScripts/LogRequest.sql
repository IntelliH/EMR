/****** Object:  StoredProcedure [dbo].[LogRequest]    Script Date: 24-Sep-19 3:46:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sahil Banker
-- Create date: 01 Aug 2019
-- Description:	To log the request and return the generated RequestID.
-- =============================================
CREATE PROCEDURE [dbo].[LogRequest]
	@PatientID varchar(100),
	@EMRID varchar(100),
	@ModuleID varchar(100),
	@Status varchar(100),
	@RequestID varchar(100)
AS
BEGIN
	SET NOCOUNT ON;
	
	if @RequestID = ''
	Begin
		insert into EMRDataRequestLogs (PatientUserID, EMRID, ModuleID, Status) values (@PatientID, @EMRID, @ModuleID, @Status)

		update EMRDataRequestLogs set RequestID = id where id = CAST(scope_identity() AS int)

		select CAST(scope_identity() AS int);
	End
	else
	Begin
		insert into EMRDataRequestLogs (PatientUserID, EMRID, ModuleID, Status, RequestID) values (@PatientID, @EMRID, @ModuleID, @Status, @RequestID)
	End
END

GO


