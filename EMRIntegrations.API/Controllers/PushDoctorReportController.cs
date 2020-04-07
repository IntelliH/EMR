using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Models;
using EMRIntegrations.Data;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

namespace EMRIntegrations.Controllers
{
    public class PushDoctorReportController : ApiController
    {
        Log objLog = new Log();

        ConnectionModel oMasterConnection = new ConnectionModel();

        [AcceptVerbs("GET", "POST")]
        public string PostDoctorReport(string EMRID, string FilePath, string EMRPatientID)
        {
            try
            {
                string UserID = string.Empty;
                string DepartmentID = "1";
                string JSONString = string.Empty;
                string ModuleID = "0";

                oMasterConnection.EMRIntegrationMasterConStr = ConfigurationManager.ConnectionStrings["EMRIntegrationMaster"].ConnectionString;
                SqlConnection oConnMaster = new SqlConnection(oMasterConnection.EMRIntegrationMasterConStr);
                oMasterConnection.oConnEmrMaster = oConnMaster;

                string RequestID = objLog.LogRequest(oMasterConnection, Logs.Status.GotPushDoctorReportRequestFromIntelliH, EMRID, ModuleID, EMRPatientID);

                switch (EMRID)
                {
                    case "1"://Athena
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                        EMRIntegrations.Athena.AthenaAPI objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, EMRID, ModuleID, EMRPatientID);

                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPushDoctorReportRequestSendingToEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                        string result = objAthenaAPI.PostClinicalDocuments(EMRPatientID, DepartmentID, FilePath, RequestID);

                        return result;
                        //break;
                    default:
                        return "{ \"EMRID is not Found\" }";
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("EMRID is not Found");
                return ex.Message + " " + ex.StackTrace;
            }
        }
    }
}
