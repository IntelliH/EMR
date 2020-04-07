namespace EMRIntegrations.Core.Enums
{
    public class Logs
    {
        public enum Status
        {
            GotDataPullRequestFromIntelliH
            , GotPushDoctorReportRequestFromIntelliH
            , ConnectionRequestSentToEMR
            , ConnectionEstablishedWithEMR
            , DataPullRequestSendingToEMR
            , DataPushDoctorReportRequestSendingToEMR
            , DataPulledInMemoryFromEMR
            , DataSavedInStagingDatabase
            , DataMappingStarted
            , DataMappingDone
            , DataSentToIntelliH
            , DoctorReportSentToEMR
        }
    }
}

