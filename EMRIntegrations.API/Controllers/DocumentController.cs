using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Models;
using EMRIntegrations.Data;
using RestSharp;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
                //int i = 1;
                //var client = new RestClient("http://emr.intellih.com/api/document/PostDocument");
                //client.Timeout = -1;
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("application/json", "{\r\n  \"emrId\": \""+ i +"\",\r\n  \"accessToken\": \"fkAFfJzfLdpHoyjpDNnSYZpeZNOV6C\",\r\n  \"filePath\": \"C:\\\\intellihApps\\\\dev\\\\emr\\\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"87734739\",\r\n  \"doctorId\": \"123\",\r\n  \"documentDate\": \"2020-12-01\",\r\n  \"documentDescription\": \"Test Document 01Dec2020 1\"\r\n}", ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                //return response.Content;

                //request.AddParameter("application/json", " {\r\n  \"emrId\": \"" + param.emrId + "\" ,\r\n  \"accessToken\": \"" + accessToken + "\",\r\n  \"filePath\": \"C:\\intellihApps\\dev\\emr\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"" + emrPatient.EMRPatientId + "\",\r\n  \"doctorId\": \"128999\",\r\n  \"documentDate\": \"" +  DateTime.UtcNow.ToString() + "\" ,\r\n  \"documentDescription\": \"CB_2374$%#.;'_20201013_055038PM.pdf\"\r\n}", ParameterType.RequestBody);
                //request.AddParameter("application/json", " {\r\n  \"emrId\": \"1\",\r\n  \"accessToken\": \"1\",\r\n  \"filePath\": \"C:\\\\intellihApps\\\\dev\\\\emr\\\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"1\",\r\n  \"doctorId\": \"128999\",\r\n  \"documentDate\": \"01-Dec-20 12:54:12 PM\",\r\n  \"documentDescription\": \"CB_2374$%#.;'_20201013_055038PM.pdf\"\r\n}", ParameterType.RequestBody);
                //request.AddParameter("application/json", " {\r\n  \"emrId\": \"1\",\r\n  \"accessToken\": \"1\",\r\n  \"filePath\": \"C:\\\\intellihApps\\\\dev\\\\emr\\\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"1\",\r\n  \"doctorId\": \"128999\",\r\n  \"documentDate\": \"01-Dec-20 12:54:12 PM\",\r\n  \"documentDescription\": \"CB_2374$%#.;'_20201013_055038PM.pdf\"\r\n}", ParameterType.RequestBody);
                //request.AddParameter("application/json", " {\r\n  \"emrId\": \"" + param.emrId + "\",\r\n  \"accessToken\": \"" + accessToken + "\",\r\n  \"filePath\": \"C:\\\\intellihApps\\\\dev\\\\emr\\\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"" + emrPatient.EMRPatientId + "\",\r\n  \"doctorId\": \"128999\",\r\n  \"documentDate\": \"" + DateTime.UtcNow.ToString() + "\",\r\n  \"documentDescription\": \"CB_2374$%#.;'_20201013_055038PM.pdf\"\r\n}", ParameterType.RequestBody);
                //request.AddParameter("application/json", " {\r\n  \"emrId\": \"6\",\r\n  \"accessToken\": \"kmw6ylb7H4NQpgAAbTSZlavWceC0zB\",\r\n  \"filePath\": \"C:\\\\intellihApps\\\\dev\\\\emr\\\\TestPDF.pdf\",\r\n  \"emrPatientid\": \"87701984\",\r\n  \"doctorId\": \"128999\",\r\n  \"documentDate\": \"2013-02-24\",\r\n  \"documentDescription\": \"CB_011220206.06PM.pdf\"\r\n}", ParameterType.RequestBody);

                string UserID = string.Empty;
                //string DepartmentID = "1";
                string JSONString = string.Empty;
                StringBuilder JSONStringReturn = new StringBuilder();
                string ModuleID = "0";
                string EMRId = ((Newtonsoft.Json.Linq.JValue)data.emrId).Value.ToString();
                string EMRPatientId = ((Newtonsoft.Json.Linq.JValue)data.emrPatientId).Value.ToString();

                oMasterConnection.EMRIntegrationMasterConStr = ConfigurationManager.ConnectionStrings["EMRIntegrationMaster"].ConnectionString;
                SqlConnection oConnMaster = new SqlConnection(oMasterConnection.EMRIntegrationMasterConStr);
                oMasterConnection.oConnEmrMaster = oConnMaster;

                string RequestID = objLog.LogRequest(oMasterConnection, Logs.Status.GotPushDoctorReportRequestFromIntelliH, EMRId, ModuleID, EMRPatientId);
                string result = string.Empty;

                switch (EMRId)
                {
                    case "1"://Athena
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        EMRIntegrations.Athena.AthenaAPI objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPushDoctorReportRequestSendingToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        result = objAthenaAPI.PostClinicalDocuments(data.emrPatientid, data.DepartmentID, data.FilePath, RequestID);
                        objLog.LogRequest(oMasterConnection, Logs.Status.DoctorReportSentToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        return result;
                    //break;
                    case "6"://DrChrono

                        #region Temp code for doctorid
                        oMasterConnection.EmrStagingDBConStr = ConfigurationManager.ConnectionStrings["EMRStagingDB"].ConnectionString;
                        SqlConnection oConnStaging = new SqlConnection(oMasterConnection.EmrStagingDBConStr);
                        oMasterConnection.oConnEmrStagingDB = oConnStaging;
                        string doctoridfromstaging = string.Empty;
                        SqlDataReader doctoridreader = SqlHelper.ExecuteReader(oMasterConnection.oConnEmrStagingDB, System.Data.CommandType.Text, "select top 1 doctor as doctorid from [drchrono].[PatientDemographics] where id = '" + EMRPatientId + "' order by requestid desc");
                        if (doctoridreader.HasRows)
                        {
                            while (doctoridreader.Read())
                            {
                                doctoridfromstaging = doctoridreader.GetString(0);
                            }
                        }
                        doctoridreader.Close();
                        #endregion

                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        EMRIntegrations.DrChrono.DrChronoAPI objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI(data, doctoridfromstaging);
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPushDoctorReportRequestSendingToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        DataTable dtDocumentData = objDrChronoAPI.PostDocument(EMRPatientId, oMasterConnection.EmrStagingDBConStr, RequestID, ((Newtonsoft.Json.Linq.JValue)data.accessToken).Value.ToString());
                        objLog.LogRequest(oMasterConnection, Logs.Status.DoctorReportSentToEMR, RequestID, EMRId, ModuleID, EMRPatientId);
                        objDrChronoAPI.SaveDocumentUpload(dtDocumentData, oMasterConnection.EmrStagingDBConStr, RequestID);
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRId, ModuleID, EMRPatientId);
                        JSONString = objDrChronoAPI.GetJSONDocumentUpload(dtDocumentData, EMRPatientId, RequestID, EMRId, ModuleID, UserID);
                        return JSONString;
                    default:
                        JSONStringReturn.Append("{");
                        JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientId + "\",");
                        JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRId + "\",");
                        JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                        JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                        JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                        JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                        JSONStringReturn.Append("\"EMRID is not Found\"");
                        JSONStringReturn.Append("}");
                        return JSONStringReturn.ToString();
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
