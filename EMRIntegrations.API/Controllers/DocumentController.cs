using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Models;
using EMRIntegrations.Data;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

namespace EMRIntegrations.Controllers
{
    public class DocumentController : ApiController
    {
        Log objLog = new Log();

        ConnectionModel oMasterConnection = new ConnectionModel();

        [AcceptVerbs("GET", "POST")]
        //public string PostDoctorReport(string EMRID, string FilePath, string EMRPatientID)
        public string PostDocument(dynamic data)
        {
            try
            {
                string UserID = string.Empty;
                //string DepartmentID = "1";
                string JSONString = string.Empty;
                string ModuleID = "0";

                oMasterConnection.EMRIntegrationMasterConStr = ConfigurationManager.ConnectionStrings["EMRIntegrationMaster"].ConnectionString;
                SqlConnection oConnMaster = new SqlConnection(oMasterConnection.EMRIntegrationMasterConStr);
                oMasterConnection.oConnEmrMaster = oConnMaster;

                string RequestID = objLog.LogRequest(oMasterConnection, Logs.Status.GotPushDoctorReportRequestFromIntelliH, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                string result = string.Empty;

                switch (((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString())
                {
                    case "1"://Athena
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        EMRIntegrations.Athena.AthenaAPI objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPushDoctorReportRequestSendingToEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        result = objAthenaAPI.PostClinicalDocuments(data.emrPatientid, data.DepartmentID, data.FilePath, RequestID);
                        return result;
                    //break;
                    case "6"://DrChrono
                        #region Temp code for doctorid
                        oMasterConnection.EmrStagingDBConStr = ConfigurationManager.ConnectionStrings["EMRStagingDB"].ConnectionString;
                        SqlConnection oConnStaging = new SqlConnection(oMasterConnection.EmrStagingDBConStr);
                        oMasterConnection.oConnEmrStagingDB = oConnStaging;
                        string doctoridfromstaging = string.Empty;
                        SqlDataReader doctoridreader = SqlHelper.ExecuteReader(oMasterConnection.oConnEmrStagingDB, System.Data.CommandType.Text, "select top 1 doctor as doctorid from [drchrono].[PatientDemographics] where id = '" + ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString() + "' order by requestid desc");
                        if (doctoridreader.HasRows)
                        {
                            while (doctoridreader.Read())
                            {
                                doctoridfromstaging = doctoridreader.GetString(0);
                            }
                        }
                        doctoridreader.Close();
                        #endregion

                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        EMRIntegrations.DrChrono.DrChronoAPI objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI(data, doctoridfromstaging);
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPushDoctorReportRequestSendingToEMR, RequestID, ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString(), ModuleID, ((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString());
                        result = objDrChronoAPI.PostDocument(((Newtonsoft.Json.Linq.JValue)data.emrPatientid).Value.ToString(), oMasterConnection.EmrStagingDBConStr, RequestID, ((Newtonsoft.Json.Linq.JValue)data.accessToken).Value.ToString());
                        return result;
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
